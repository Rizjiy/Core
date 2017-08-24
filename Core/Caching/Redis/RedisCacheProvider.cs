using System;
using StackExchange.Redis;
using System.Linq;

namespace Core.Caching.Redis
{
	public class RedisCacheProvider : IDisposable, ICacheProvider
	{
		#region DI

		// Необязательный логгер ошибок
		//public ILogger Logger { get; set; }

		#endregion

		#region IDisposable

		private readonly ConnectionMultiplexer _redis;
		private readonly IDatabase _data;
		private readonly IServer _server;

		public RedisCacheProvider(string connection)
		{
			if (string.IsNullOrWhiteSpace(connection))
				return; // Не указан коннекшен: режим заглушки - ни ошибок, ни реальных действий

			try
			{
				var config = ConfigurationOptions.Parse(connection);
				config.KeepAlive = 5;
				config.SyncTimeout = 1000;
				config.AbortOnConnectFail = false;
				config.AllowAdmin = true;

				_redis = ConnectionMultiplexer.Connect(config);
				_data = _redis.GetDatabase();

				_server = _redis.GetServer(config.EndPoints.First());
			}
			catch (Exception ex)
			{
				// Логгер - необязательная фича кеша
				//Logger?.Log(ex, ExceptionSource.Cache);
			}
		}

		public void Dispose()
		{
			_redis?.Dispose();
		}

		#endregion

		#region ICacheProvider

		public bool IsConnected => (_redis?.IsConnected ?? false) && _data != null && (_server?.IsConnected ?? false);

		public TValue Get<TKey, TValue>(TKey key) where TValue : class
		{
			if (key == null)
				return null;

			if (!IsConnected)
				return null;

			// Строчный ключ
			var stringKey = key.ToString();

			// Извлекаю значение в терминах StackExchange
			var redisValue = _data.StringGet(stringKey);

			// Строчное значение
			var stringValue = redisValue.ToString();

			if (string.IsNullOrEmpty(stringValue))
				return null;

			// Приведение типов
			var value = (TValue)Convert.ChangeType(stringValue, typeof(TValue));

			return value;
		}

		public void Set<TKey, TValue>(TKey key, TValue value, TimeSpan? expiry = null) where TValue : class
		{
			if (key == null)
				throw new NullReferenceException("Key is not be null.");

			if (!IsConnected)
				return;

			// Строчный ключ
			var stringKey = key.ToString();

			if (value == null)
			{
				// Удаляю значение из кеша
				_data.KeyDelete(stringKey);
			}
			else
			{
				// Строчное значение
				var stringValue = value.ToString();

				// Устанавливаю значение в кеш
				_data.StringSet(stringKey, stringValue, expiry);
			}
		}

		public bool AddSet<TKey, TValue>(TKey key, TValue value, TimeSpan? expiry = null) where TValue : class
		{
			if (key == null)
				throw new NullReferenceException("Key is not be null.");

			if (!IsConnected)
				return false;

			// Строчный ключ
			var stringKey = key.ToString();

			if (value == null)
			{
				// Удаляю значение из кеша
				return _data.KeyDelete(stringKey);
			}
			else
			{
				// Строчное значение
				var stringValue = value.ToString();
				// Устанавливаю значение в кеш
				return _data.StringSet(stringKey, stringValue, expiry, When.NotExists);
			}
		}

		public void Clear(string selector)
		{
			if (!IsConnected)
				return;

			foreach (var redisKeySet in _server.Keys(pattern: selector))
			{
				_data.KeyDelete(redisKeySet);
			}
		}

		#endregion
	}
}
