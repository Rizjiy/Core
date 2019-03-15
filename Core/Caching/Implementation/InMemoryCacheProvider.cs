using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Core.Caching.Interface;
using Core.Interfaces;
using Core.Log;

namespace Core.Caching.Implementation
{
    /// <summary>
    /// Реализация провайдера кэша с хранением в MemoryCache.
    /// </summary>
    public class InMemoryCacheProvider : ICacheProvider
    {
        public Logger<InMemoryCacheProvider> Logger { get; set; }

        private readonly ObjectCache _cache = new MemoryCache(nameof(InMemoryCacheProvider));

        /// <summary>
        /// Вовзращает полный ключ, построенный на базе ключа кэша и наименования самого кэша. Полученный ключ уникален среди значений всех кэшей.
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <param name="key">Значение кэша</param>
        /// <returns></returns>
        private string GetFullKey(ICache cache, string key)
        {
            return $"{GetCachNameKeyPrefix(cache)}{key}";
        }

        /// <summary>
        /// Для Взаданного кэша возвращает префикс, исплользуемый для построения полного ключа
        /// </summary>
        private string GetCachNameKeyPrefix(ICache cache)
        {
            return $"{cache.GetType().Name}.";
        }

        /// <summary>
        /// Вовзращает наименование главного ключа, иплюзуемого для регулярных автоматических обновлений кэша.
        /// </summary>
        private string GetIntervalKey(ICache cache)
        {
            return $"[intervalkey].{cache.GetType()}.[intervalkey]";
        }

        public void Set(ICache cache, string key, object value)
        {
	        if (value == null)
	        {
		        this.Remove(cache, key);
		        return;
	        }

	        var fullKey = GetFullKey(cache, key);

            _cache.Set(fullKey, value,
                new CacheItemPolicy
                {
                    AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration
                });
        }

        public object Get(ICache cache, string key)
        {
            var fullKey = GetFullKey(cache, key);

            return _cache[fullKey];
        }

        public void Clear(ICache cache)
        {
            var items = _cache.Where(i => i.Key.StartsWith(GetCachNameKeyPrefix(cache))).ToList();

            foreach (var item in items)
            {
                _cache.Remove(item.Key);
            }
        }

        public void Remove(ICache cache, string key)
        {
            var fullKey = GetFullKey(cache, key);

            _cache.Remove(fullKey);
        }

        /// <summary>
        /// Метод запускает механизм регулярного автоматического полного обновления кэша.
        /// ps. Автообновление всего кэша работает через коллбеки устаревания MemoryCache, которые срабатывают не чаще чем ~20c 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="timeSpan">Временной интервал, задающий промежуток времени между соседними обновлениями</param>
        /// <param name="runImmediately">Если true (как по дефолту), то указывает что в момент вызова метода нужно выполнить первоначальную инициализацию актуальными данными из бд</param>
        public void SetIntervalUpdate(ICache cache, TimeSpan timeSpan, bool runImmediately = true)
        {
			//первноначальное полоное обновление кэша
            if(runImmediately)
			    InvalidateAll(cache);

			//устанавливаем intervalKey для регулярных полных обновлений кэша
            var intervalKey = GetIntervalKey(cache);

            _cache.Set(intervalKey, intervalKey,
                new CacheItemPolicy
                {
                    UpdateCallback = GetIntervalCallback(cache, timeSpan),
                    AbsoluteExpiration = DateTimeOffset.Now.Add(timeSpan),
                });
        }

        /// <summary>
        ///  Восвращает коллекцию ключей значений в формате $"{cache.GetType().Name}.{key}" для заданного кэша.
        /// ps. public по причине удобства в тестировании. А так вполне мог бы быть private
        /// </summary>
        /// <param name="cache">Кэш</param>
        /// <returns></returns>
        public IReadOnlyCollection<string> GetAllKeys(ICache cache)
        {
            return _cache
                //отбираем ключи переданного кеша
                .Where(i => i.Key.StartsWith(GetCachNameKeyPrefix(cache)))
                .Select(i => i.Key)
                .ToList();
        }

        /// <summary>
		/// Возвращает делегат для калбека. Делегат скрыт в данный метод с целью возможности прогидывать в него дополнительные параметры через механизм замыкания.
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		private CacheEntryUpdateCallback GetIntervalCallback(ICache cache, TimeSpan timeSpan)
        {
            var cacheClosured = cache;
            var timeSpanClosured = timeSpan;

            return args =>
            {
                if (args.RemovedReason == CacheEntryRemovedReason.Expired || args.RemovedReason == CacheEntryRemovedReason.Removed)
                {
                    try
                    {
                        //полное обновление кэша
                        InvalidateAll(cache);
                    }
                    catch (Exception ex)
                    {
                        Logger.Fatal($"Ошибка автоматического полного обновления кэша {cache.GetType().Name}", ex);
                    }

                    //====---- Прописываем вызов следующей итерации автоматического обновления

                    //пересоздаем intervalKey
                    args.UpdatedCacheItem = new CacheItem(args.Key, args.Key);

                    //настраивам следующий вызов каллбека
                    args.UpdatedCacheItemPolicy = new CacheItemPolicy
                    {
                        AbsoluteExpiration = DateTimeOffset.Now.Add(timeSpanClosured),
                        UpdateCallback = GetIntervalCallback(cacheClosured, timeSpan),
                    };
                }


            };
        }

        /// <summary>
        /// Полностью обновляет кэш актуальным данными. Механизм получения актуальных данных предоставляет объект кэша через метод ICache.LoadAll.
        /// </summary>
        /// <param name="cache">Кэш, который нужно обновить.</param>
        protected virtual void InvalidateAll(ICache cache)
        {
            Logger.Debug($"Обновление кэша {cache.GetType().Name}");

            //получаем актульные данные
            var actualData = cache.LoadAll();

            // получим все ключи, которые есть сейчас в кеше
            var allKeys = this.GetAllKeys(cache);

            // запишем все актуальные данные в кэш
            actualData.AsParallel().ForAll(kv => Set(cache, kv.Key, kv.Value));


            //=======----------- все ключи, которых уже нет в источнике, грохнем из провайдера

            //наименования актуальных ключей приводим к нативному формату провайлера
            var actualDataNativeKeys = actualData.Select(x => GetFullKey(cache, x.Key)).ToArray();

            //ключи, которые нужно удалить
            var notExistingKeys = allKeys.AsParallel().Where(x => !actualDataNativeKeys.Contains(x));

            //собственно удаление
            notExistingKeys.AsParallel().ForAll(k => _cache.Remove(k));
        }
    }
}

