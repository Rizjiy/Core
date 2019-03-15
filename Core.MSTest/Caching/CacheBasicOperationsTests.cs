using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Core.Caching.Interface;
using Core.Internal.Dependency;
using LightInject;
using NUnit.Framework;

namespace Core.MSTest.Caching
{
    [TestFixture]
    public class CacheBasicOperationsTests
    {
        // Зарегистрируется автоматически в виде синглтона, т.к. реализует ICache и IDependency
        public CacheBaseImpl Cache { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    //реистрируется провайдер
                    container.Register<ICacheProvider, FakeInMemoryCacheProvider>(new PerContainerLifetime());


                    container.InjectProperties(this);
                });
        }

        /// <summary>
        /// в источнике (бд) нет данных
        /// </summary>
        [Test, Order(1)]
        public void Get_SourceHasNoData()
        {
            //assertion---------===========
            //в кэш возвращает null
            Assert.Null(Cache.Get(1));
            Assert.Null(Cache.Get(2));
            Assert.Null(Cache.Get(3));
        }


        /// <summary>
        /// в источнике появились кое какие данные
        /// </summary>
        [Test, Order(2)]
        public void Get_SourceHasData()
        {
            //action---------===========
            //фейковые данные для источника
            Cache.FakeData = new List<TestPersonDto>
            {
                new TestPersonDto {Id = 1, Name = "Developer"},
                new TestPersonDto {Id = 2, Name = "DevOps"},
            };

            //assertion---------===========
            //в кэше должны появиться элементы
            Assert.AreEqual("Developer", Cache.Get(1).Name);
            Assert.AreEqual("DevOps", Cache.Get(2).Name);
            Assert.Null(Cache.Get(3));
        }

        /// <summary>
        /// в источнике поменялись данные. 
        /// </summary>
        [Test, Order(3)]
        public void Get_SourceHasChangedData()
        {
            //action---------===========
            //изменение в фейковых даных источника
            Cache.FakeData = new List<TestPersonDto>
            {
                //добавлено
                new TestPersonDto {Id = 3, Name = "Analyst"},

                new TestPersonDto {Id = 1, Name = "Robot" /*изменилось!!*/},

                //исчезло!!
                //new TestPersonDto { Id = 2, Name = "DevOps"},
            };

            //assertion---------===========
            //всезначения прежние, закешированые (автоапдейт кэша не включен).
            Assert.AreEqual("Developer", Cache.Get(1).Name);
            Assert.AreEqual("DevOps", Cache.Get(2).Name);

            //должно появиться, т.к. раньше отсутсвовало, тобишь было null, а null значения не кэшируются 
            Assert.AreEqual("Analyst", Cache.Get(3).Name);
        }

        /// <summary>
        /// Очистка кэша методом Clear. Метод Get должен вернуть актуальные данные из источника. 
        /// </summary>
        [Test, Order(4)]
        public void Clear()
        {
            //action---------===========
            Cache.Clear();

            //assertion---------===========
            Assert.AreEqual("Robot", Cache.Get(1).Name);
            Assert.Null(Cache.Get(2));
            Assert.AreEqual("Analyst", Cache.Get(3).Name);
        }
    }
}
