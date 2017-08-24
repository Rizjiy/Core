using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Caching;
using Core.Caching.Redis;
using NUnit.Framework;

namespace Core.MSTest.Caching.Redis
{
	[TestFixture]
	class RedisCacheProviderTests
	{

		private ICacheProvider _provider;

		//private const string RedisConnectionString = "localhost:6379";
		private const string RedisConnectionString = "g-devweb-01.srv.int:6379";

		[OneTimeSetUp]
		public void Init()
		{
			_provider = new RedisCacheProvider(RedisConnectionString);
		}

		[OneTimeTearDown]
		public void Dispose()
		{
			((IDisposable)_provider).Dispose();
		}

		[Test]
		public void SetAdd_NotNullValue_ValueInCache()
		{
			//подготовительная очистка
			_provider.AddSet<string, string>("test", null);
			Assert.IsTrue(_provider.AddSet("test", "value1"));

			var valueInCache = _provider.Get<string, string>("test");
			Assert.AreEqual(valueInCache, "value1");
		}

		[Test]
		public void SetAdd_SameKey_NullValues_FirstValueInCache()
		{
			//подготовительная очистка
			_provider.AddSet<string, string>("test", null);

			Assert.IsTrue(_provider.AddSet("test", "value1"));
			Assert.IsFalse(_provider.AddSet("test", "value2"));

			var valueInCache = _provider.Get<string, string>("test");
			Assert.AreEqual(valueInCache, "value1");

		}

		[Test]
		public void SetAdd_NullValue()
		{
			//заносим что то в кэш (на случай если ничего не было)
			_provider.AddSet("test", "value1");

			//очищяем
			Assert.IsTrue(_provider.AddSet("test", (string)null));
				
			//проверям что значение в кэше обнуллилось
			var valueInCache = _provider.Get<string, string>("test");
			Assert.IsNull(valueInCache);

			//повторная очистка вернет false, т.к. значение не меняется
			Assert.IsFalse(_provider.AddSet("test", (string)null));
		}

		[Test]
		public void SetAdd_Concurrently()
		{
			var threadsCount = 20;

			ManualResetEvent ev = new ManualResetEvent(false);
			var tasks = new Task<bool>[threadsCount];

			for (int i = 0; i < threadsCount; i++)
			{
				var value = i.ToString();
				tasks[i] = Task.Run<bool>(() =>
				{
					ev.WaitOne();
					return _provider.AddSet("test", value);
				});
			}

			_provider.AddSet("test", (string)null);
			ev.Set();
				
			Task.WaitAll(tasks);

			//в результате значение в кэш должно быть занесено ровно один раз
			Assert.AreEqual(tasks.Where(t => t.Result).Count(), 1);

			//проверяем что в кэше на самом деле что то есть
			var valueInCache = _provider.Get<string, string>("test");
			Assert.IsNotNull(valueInCache);

			//и это что то должно соответствовать значению из успешного вызова
			for (int i = 0; i < threadsCount; i++)
			{
				if (tasks[i].Result)
				{
					Assert.AreEqual(valueInCache, i.ToString());
					break;
				}
			}
		}
	}
}