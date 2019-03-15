using System;
using System.Collections.Generic;

namespace Core.Caching.Interface
{
    public interface ICache
    {
        /// <summary>
        /// Загрузка данных из БД для полного обновления кэша
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> LoadAll();
    }

    public interface ICache<in TKey, TValue> : ICache
        where TKey : IEquatable<TKey>
        where TValue : class, new()
    {
        /// <summary>
        /// Метод инвалидации элемента данных
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <param name="value"></param>
        /// <returns> Значение </returns>
        void Set(TKey key, TValue value);

	    /// <summary>
	    /// Получение значения из кеша
	    /// </summary>
	    /// <param name="key"> Ключ </param>
	    /// <returns> Значение </returns>
	    TValue Get(TKey key);

	    /// <summary>
	    /// Удаление всех данных из кеша
	    /// </summary>
	    void Clear();

	    /// <summary>
	    /// Удаление значения из кеша.
	    /// </summary>
	    /// <param name="key">Ключ.</param>
	    void Remove(TKey key);

        /// <summary>
        /// Метод инвалидации элемента данных
        /// </summary>
        /// <param name="key">Ключ.</param>
        /// <returns> Значение </returns>
        TValue Invalidate(TKey key);

        ICacheProvider Provider { get; }

        /// <summary>
        /// Метод запускает механизм регулярного автоматического полного обновления кэша.
        /// </summary>
        /// <param name="timeSpan">Временной интервал, задающий промежуток времени между соседними обновлениями</param>
        /// <param name="runImmediately"></param>
        void SetIntervalUpdate(TimeSpan timeSpan, bool runImmediately = true);
	}

    
}