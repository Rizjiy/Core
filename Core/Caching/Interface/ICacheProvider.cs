using System;

namespace Core.Caching.Interface
{
    /// <summary>
    /// Интерфейс провайдера кэша
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
		/// Метод инвалидации элемента данных
		/// </summary>
		/// <param name="key">Ключ.</param>
		/// <returns> Значение </returns>
		void Set(ICache cache, string key, object value);

        /// <summary>
        /// Получение значения из кеша
        /// </summary>
        /// <param name="key"> Ключ </param>
        /// <returns> Значение </returns>
        object Get(ICache cache, string key);

        /// <summary>
		/// Удаление всех данных из кеша
		/// </summary>
		void Clear(ICache cache);

        /// <summary>
		/// Удаление значения из кеша.
		/// </summary>
		/// <param name="key">Ключ.</param>
		void Remove(ICache cache, string key);

        //ICache<string, TValue>

        /// <summary>
        /// Метод запускает механизм регулярного автоматического полного обновления кэша.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="timeSpan">Временной интервал, задающий промежуток времени между соседними обновлениями</param>
        /// <param name="runImmediately"></param>
        void SetIntervalUpdate(ICache cache, TimeSpan timeSpan, bool runImmediately = true);
    }
}
