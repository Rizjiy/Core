using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Caching.Implementation;
using Core.Caching.Interface;
using Core.Internal.Dependency;
using LightInject;
using NUnit.Framework;
using System.Diagnostics;
using System.Runtime.Caching;

namespace Core.MSTest.Caching
{
    /// <summary>
    ///Тестовая реализация провайдера, позволяющая остлеживать момент обновления кэша
    /// </summary>
    public class FakeInMemoryCacheProvider : InMemoryCacheProvider
    {
        protected override void InvalidateAll(ICache cache)
        {
            base.InvalidateAll(cache);

            _afterInvalidateAllHandler?.Invoke();
        }

        private Action _afterInvalidateAllHandler;

        //хз возможно было бы с точки зрения надежности тестов кошернее передавать делегат с ассертами, который бы на время своего выполнения блокировал бы цикл обновлений 
        public TimeSpan WaitForAfterNextUpdate()
        {
            var promise = new TaskCompletionSource<bool>();

            this._afterInvalidateAllHandler = () => 
            {
                _afterInvalidateAllHandler = null;
                promise.SetResult(true);
            };

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            promise.Task.Wait();

            stopwatch.Stop();

            Console.WriteLine($"Время ожидания полного обновления {stopwatch.Elapsed.TotalSeconds} секунд.");

            return stopwatch.Elapsed;
        }
    }

    [TestFixture]
    public class CacheIntervalUpdateTests
    {
        // Зарегистрируется автоматически в виде синглтона, т.к. реализует ICache и IDependency
        public CacheBaseImpl Cache { get; set; }

        //для хранения ссылки на провайдер удобной приведенностью к FakeInMemoryCacheProvider 
        private FakeInMemoryCacheProvider _provider;


        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    container.Register<ICacheProvider, FakeInMemoryCacheProvider>(new PerContainerLifetime());
                    _provider = (FakeInMemoryCacheProvider)container.GetInstance<ICacheProvider>();




                    container.InjectProperties(this);
                });
        }

        /// <summary>
        /// Включается автоматическое обновление. Проверяется первоначальное заполнение актуальными данными.
        /// </summary>
	    [Test, Order(1)]
	    public void SetIntervalUpdate()
	    {
	        //arrange-----======

            //типа в бд такое
            Cache.FakeData = new List<TestPersonDto>
		    {
			    new TestPersonDto { Id = 1, Name = "Developer"},
			    new TestPersonDto { Id = 2, Name = "Analyst"},
		    };

	        //action-----======
            //включается автоматическое обновление с интервалом 1 с (На практике memorycache обновляется не чаще ~20 сек)
            Cache.SetIntervalUpdate(TimeSpan.FromSeconds(1));


	        //assertion-----======

            //по дефолту должно уже отработать первоначальное заполние кэша актуальными данными
            Assert.AreEqual("Developer", Cache.Get(1).Name);
            Assert.AreEqual("Analyst", Cache.Get(2).Name);

            //очищаем кэш, чтобы заоодно убедиться что очистка не ломает автоматические обновления
            Cache.Clear();
        }

        /// <summary>
        /// Изменились фейковые данные. Автоматическое обновление должно их подтянуть.
        /// </summary>
        [Test, Order(2)]
        public void ChangeInFakeData()
        {
            //action-----======

            Cache.FakeData = new List<TestPersonDto>
            {
                new TestPersonDto { Id = 1, Name = "Developer"},
                new TestPersonDto { Id = 2, Name = "Robot"},
                new TestPersonDto { Id = 3, Name = "Admin1F"},
            };

            //ждем следующей полной отработки цикла обновления кэша
            this._provider.WaitForAfterNextUpdate();

            //assertion-----======

            Assert.AreEqual("Developer", Cache.Get(1).Name);
            Assert.AreEqual("Robot", Cache.Get(2).Name);
            Assert.AreEqual("Admin1F", Cache.Get(3).Name);
        }

        /// <summary>
        /// Изменились фейковые данные сразу после предыдущего автообновления, но до (!!!!) обращения к кэшу. Для уже занесенных в кэш ключей должны остаться прежние значения.
        /// </summary>
        [Test, Order(3)]
        public void ChangeInFakeDataJustAfterUpdate()
        {
            //action-----======

            Cache.FakeData = new List<TestPersonDto>
            {
                new TestPersonDto { Id = 1, Name = "Developer"},
                new TestPersonDto { Id = 2, Name = "Robot"},
                new TestPersonDto { Id = 3, Name = "Admin1F"},
            };

            //ждем следующей полной отработки цикла обновления кэша
            this._provider.WaitForAfterNextUpdate();

            Cache.FakeData = new List<TestPersonDto>
            {
                new TestPersonDto { Id = 1, Name = "Analyst"},
                new TestPersonDto { Id = 5, Name = "ScrumMuster"},
            };

            //assertion-----======
            //должно остаться прежними, т.к. уже были в кэше
            Assert.AreEqual("Developer", Cache.Get(1).Name);
            Assert.AreEqual("Robot", Cache.Get(2).Name);
            Assert.AreEqual("Admin1F", Cache.Get(3).Name);

            //а это должно подтянутся из т.к. такого ключа еще нет в кэше 
            Assert.AreEqual("ScrumMuster", Cache.Get(5).Name);
        }


        /// <summary>
        /// Изменились фейковые данные (в бд пусто). По результатам автоматического обновления Get должен возвращать null для любого ключа.
        /// </summary>
        [Test, Order(4)]
        public void ChangeInFakeDataEmpty()
        {
            //action-----======

            //в фейковых данный пусто
            Cache.FakeData = new List<TestPersonDto> { };

            //assertion-----======

            //ждем следующей полной отработки цикла обновления кэша
            this._provider.WaitForAfterNextUpdate();

            Assert.Null(Cache.Get(1));
            Assert.Null(Cache.Get(2));
            Assert.Null(Cache.Get(3));
        }
    }
}
