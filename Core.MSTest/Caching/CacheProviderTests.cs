using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Core.Caching.Implementation;
using Core.Caching.Interface;
using Core.Internal.Dependency;
using LightInject;
using NUnit.Framework;

namespace Core.MSTest.Caching
{
    /// <summary>
    ///Кэш-затычка. Нужен поскольку практически любая операция провайдера кэша требует инстанс кэша.
    /// </summary>
    public class CacheStub : ICache
    {
        public IDictionary<string, object> LoadAll()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    ///еще одна затычка
    /// </summary>
    public class CacheStub2 : ICache
    {
        public IDictionary<string, object> LoadAll()
        {
            throw new System.NotImplementedException();
        }
    }

    /// <summary>
    /// Тестирует InMemoryCacheProvider на основные операции (добавить элемент в кэш, удалить из кэша и .т.п)
    /// </summary>
    [TestFixture]
    public class CacheProviderTests
    {
        public InMemoryCacheProvider Provider { get; set; }

        private ICache _cacheStub = new CacheStub();
        private ICache _cacheStub2 = new CacheStub2();

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    container.Register<InMemoryCacheProvider>(new PerContainerLifetime());

                    //логгер работает вне скопа
                    //_scope = container.BeginScope();

                    container.InjectProperties(this);
                });
        }

        /// <summary>
        /// добавление элементов в кэш
        /// </summary>
        [Test, Order(1)]
        public void AddItems()
        {
            //action---------===========
            Provider.Set(_cacheStub, "1", new TestPersonDto { Name = "Developer" });
            Provider.Set(_cacheStub, "2", new TestPersonDto { Name = "DevOps" });

            //assertion---------===========

            //в кэше должны появиться элементы
            Assert.AreEqual("Developer", ((TestPersonDto)Provider.Get(_cacheStub, "1")).Name);
            Assert.AreEqual("DevOps", ((TestPersonDto)Provider.Get(_cacheStub, "2")).Name);

            //всего элементов в _cacheStub
            Assert.AreEqual(2, Provider.GetAllKeys(_cacheStub).Count);
        }

        /// <summary>
        /// добавление в кэш уже существующего элемента
        /// </summary>
        [Test, Order(2)]
        public void ReplaceExistingItem()
        {
            //action---------===========
            Provider.Set(_cacheStub, "2", new TestPersonDto { Name = "Architect" });

            //assertion---------===========

            //не изменилось
            Assert.AreEqual("Developer", ((TestPersonDto)Provider.Get(_cacheStub, "1")).Name);

            //поменялось с "DevOps" на "Architect"
            Assert.AreEqual("Architect", ((TestPersonDto)Provider.Get(_cacheStub, "2")).Name);

            //всего элементов в _cacheStub
            Assert.AreEqual(2, Provider.GetAllKeys(_cacheStub).Count);
        }

        /// <summary>
        /// Занесение значений во 2-ой кэш. Проверка что всё ок и кэши не конфилктуют.
        /// </summary>
        [Test, Order(3)]
        public void SetValueForAnotherCache()
        {
            //action---------===========

            //Занесение значений во 2-ой кэш.
            Provider.Set(_cacheStub2, "2", new TestPersonDto { Name = "SysAdmin" });
            Provider.Set(_cacheStub2, "3", new TestPersonDto { Name = "Analyst" });

            //assertion---------===========

            //Проверка значений во 2-ом кэше.
            Assert.AreEqual("SysAdmin", ((TestPersonDto)Provider.Get(_cacheStub2, "2")).Name);
            Assert.AreEqual("Analyst", ((TestPersonDto)Provider.Get(_cacheStub2, "3")).Name);

            //у другого кэша все по прежнему
            Assert.AreEqual("Developer", ((TestPersonDto)Provider.Get(_cacheStub, "1")).Name);
            Assert.AreEqual("Architect", ((TestPersonDto)Provider.Get(_cacheStub, "2")).Name);

            //всего элементов в _cacheStub
            Assert.AreEqual(2, Provider.GetAllKeys(_cacheStub).Count);

            //всего элементов в _cacheStub2
            Assert.AreEqual(2, Provider.GetAllKeys(_cacheStub2).Count);
        }

        /// <summary>
        /// очистка кэша
        /// </summary>
        [Test, Order(4)]
        public void ClearAll()
        {
            //action---------===========

            Provider.Clear(_cacheStub);

            //assertion---------===========

            //кэш пуст
            Assert.Null(Provider.Get(_cacheStub, "1"));
            Assert.Null(Provider.Get(_cacheStub, "2"));

            //2-ой кэш не затронут очисткой
            Assert.AreEqual("SysAdmin", ((TestPersonDto)Provider.Get(_cacheStub2, "2")).Name);
            Assert.AreEqual("Analyst", ((TestPersonDto)Provider.Get(_cacheStub2, "3")).Name);

            //в _cacheStub после очистки должно быть 0 элементов
            Assert.AreEqual(0, Provider.GetAllKeys(_cacheStub).Count);

            //всего элементов в _cacheStub2
            Assert.AreEqual(2, Provider.GetAllKeys(_cacheStub2).Count);
        }
    }
}