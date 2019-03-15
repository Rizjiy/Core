using Core.Fakes;
using Core.Internal.Dependency;
using LightInject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Core.NUnitTest.Tests.Services
{
    [TestFixture]
    public class LoadDtoOrNullTests
    {
        private Scope _scope;
        private IServiceContainer _container;
        public TestEntityService Service { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    container.SetFakeContext();
                    _scope = container.BeginScope();
                    _container = container;
                });

            // Регистриуруем тестируемые сервисы
            _container.Register<TestEntityService>();
            _container.InjectProperties(this);

        
        }

        [Test]
        public void LoadDtoOrNullTest()
        {
            // Подготовка
            int id = 10;
            string inn = "0987654321";
            var testTable = new List<TestEntity>()
            {
                new TestEntity { Id = id, Inn = inn}
            };

            _container.SetTable(testTable);

         
            // Действие - получение данных
            var dto = Service.LoadDtoOrNull(id);

            // Проверка
            Assert.IsNotNull(dto);
            Assert.AreEqual(id, dto.Id);
            Assert.AreEqual(inn, dto.Inn);

        }


        [OneTimeTearDown]
        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
