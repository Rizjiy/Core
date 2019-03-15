using System;
using System.Collections.Generic;
using Core.Caching.Interface;

namespace Core.Caching.Implementation
{
    /// <summary>
    /// Базовый класс кэша, от которого будут реализовываться конкретные кэши со своими типами пар ключ/значение и логикой получения актуальных данных из бд.
    /// </summary>
    /// <typeparam name="TKey">Ключ</typeparam>
    /// <typeparam name="TValue">Значение</typeparam>
    public abstract class CacheBase<TKey, TValue> : ICache<TKey, TValue> where TValue : class, new() where TKey : IEquatable<TKey>
	{
        /// <summary>
        /// Метод преобразует указанный ключ в строку (путем вызова ToString() на ключе) 
        /// для того, чтобы полученную строку в последующем можно было передать в провайдер.   
        /// Переопределение метода позволяет управлять этим процессом.
        /// Это может портебоваться, например, чтобы сделать кэш нечувствительным к регистру. (провайдер чувствителен к регистру) 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
	    protected virtual string GetTransformedKey(TKey key)
	    {
	        return key.ToString();
	    }

        public ICacheProvider Provider { get; set; }

        public void Set(TKey key, TValue value)
        {
			Provider.Set(this, GetTransformedKey(key), value);
        }

        public TValue Get(TKey key)   
        {
            var res = (TValue)Provider.Get(this, GetTransformedKey(key));

	        return res ?? Invalidate(key);
		}

	    /// <summary>
	    /// Удаление всех данных из кеша
	    /// </summary>
        public void Clear()
        {
            Provider.Clear(this);
        }

        public void Remove(TKey key)
        {
            Provider.Remove(this, GetTransformedKey(key));
        }

	    public TValue Invalidate(TKey key)
        {
            //получение актуального значения
            TValue originalValue = LoadValue(key);

            //сохранение в кэш
            Provider.Set(this, GetTransformedKey(key), originalValue);

            return originalValue;
        }

	    /// <summary>
	    /// Метод запускает механизм регулярного автоматического полного обновления кэша.
	    /// </summary>
	    /// <param name="timeSpan">Временной интервал, задающий промежуток времени между соседними обновлениями</param>
	    /// <param name="runImmediately"></param>
	    public void SetIntervalUpdate(TimeSpan timeSpan, bool runImmediately = true)
	    {
	        Provider.SetIntervalUpdate(this, timeSpan);
		}

	    /// <summary>
        /// Получает оригинальное значение по ключу из БД или другого изначального источника.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value</returns>
        protected abstract TValue LoadValue(TKey key);

        /// <summary>
        /// Вовзращает полный набор актуальных данных для полного обновления кэша.
        /// </summary>
        public abstract IDictionary<string, object> LoadAll();
    }  
}