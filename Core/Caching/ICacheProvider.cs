using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Caching
{
	/// <summary>
	/// Провайдер, реализующий функционал кеша
	/// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// Указывает на наличие соединения с серверном
		/// </summary>
		bool IsConnected { get; }

		/// <summary>
		/// Установить значение в кеш
		/// </summary>
		/// <typeparam name="TKey"> Тип ключа </typeparam>
		/// <typeparam name="TValue"> Тип значения - класс </typeparam>
		/// <param name="key"> Ключ </param>
		/// <returns> Значение, может быть null </returns>
		TValue Get<TKey, TValue>(TKey key) where TValue : class;

		/// <summary>
		/// Установить значение в кеш
		/// </summary>
		/// <typeparam name="TKey"> Тип ключа </typeparam>
		/// <typeparam name="TValue"> Тип значения - класс </typeparam>
		/// <param name="key"> Ключ </param>
		/// <param name="value"> Значение, может быть null </param>
		/// <param name="expiry"> Время жизни значения в кеше </param>
		void Set<TKey, TValue>(TKey key, TValue value, TimeSpan? expiry = null) where TValue : class;


		/// <summary>
		/// Установить значение в кэш (если значение с таким ключом уже есть, то остается старое значение).
		/// </summary>
		/// <typeparam name="TKey"> Тип ключа </typeparam>
		/// <typeparam name="TValue"> Тип значения - класс </typeparam>
		/// <param name="key"> Ключ </param>
		/// <param name="value"> Значение, может быть null </param>
		/// <param name="expiry"> Время жизни значения в кеше </param>
		/// <returns></returns>
		bool AddSet<TKey, TValue>(TKey key, TValue value, TimeSpan? expiry = null) where TValue : class;

		/// <summary>
		/// Удаляет значения из кеша, ориентируясь на селектор
		/// </summary>
		/// <param name="selector"> Строчный селектор </param>
		void Clear(string selector);
	}
}
