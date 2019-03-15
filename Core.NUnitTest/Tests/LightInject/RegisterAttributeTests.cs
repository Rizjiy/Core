using Core.Interfaces;
using Core.Internal.Dependency;
using Core.Ioc;
using LightInject;
using NUnit.Framework;
using System;

namespace Core.NUnitTest.Tests.LightInject
{
    [TestFixture]
    public class RegisterAttributeTests
    {

        private IServiceContainer _container;

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    _container = container;

                });

            //_container.InjectProperties(this);

        }

        [OneTimeTearDown]
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Тест проверяет работу атрибута регистрации типов
        /// </summary>
        [Test]
        public void RegisterServiceTest_InScope()
        {
            using (_container.BeginScope())
            {
                var instance1 = _container.GetInstance<IClass1>();
                var instance2 = _container.GetInstance<PerScopeClass1>();

                Assert.NotNull(instance1);
                Assert.NotNull(instance2);
            }

            //Должен взорваться, т.к. нет скопа
            Assert.Throws<InvalidOperationException>(() => 
            _container.GetInstance<IClass1>()
            );

            //Должен взорваться, т.к. по дефолту время жизни PerScope
            Assert.Throws<InvalidOperationException>(() =>
            _container.GetInstance<IClass3>()
            );


        }

        /// <summary>
        /// Создание объекта Синглтон
        /// </summary>
        [Test]
        public void RegisterServiceTest_Singlton()
        {
            var instance1 = _container.GetInstance<PerContainerClass>();
            var instance2 = _container.GetInstance<PerContainerClass>();

            Assert.NotNull(instance1);
            Assert.AreEqual(instance1, instance2);

        }

        /// <summary>
        /// Создание разных экземпляров одного объекта
        /// </summary>
        [Test]
        public void RegisterServiceTest_Transient()
        {
            var instance1 = _container.GetInstance<TransientClass>();
            var instance2 = _container.GetInstance<TransientClass>();

            Assert.NotNull(instance1);
            Assert.NotNull(instance2);
            Assert.AreNotEqual(instance1, instance2);

        }


        #region Тестовые классы

        /// <summary>
        /// Регистрируем от интерфейса
        /// </summary>
        [Register(typeof(IClass1), Lifetime.PerScope)]
        public class PerScopeClass1 : IClass1, IDependency
        {
            public void Dispose()
            {
                Console.WriteLine($"Dispose {this.GetType().Name}");
            }
        }

        /// <summary>
        /// Регистрируем от интерфейса с дефолтовым лайфтаймом
        /// </summary>
        [Register(typeof(IClass3))]
        public class PerScopeClass2 : IClass3, IDependency
        {
            public void Dispose()
            {
                Console.WriteLine($"Dispose {this.GetType().Name}");
            }
        }


        /// <summary>
        /// Региистрируем как Singleton
        /// </summary>
        [Register(Lifetime=Lifetime.PerContainer)]
        public class PerContainerClass : IDependency
        {
            public void Dispose()
            {
                Console.WriteLine($"Dispose {this.GetType().Name}");
            }
        }

        /// <summary>
        /// Региистрируем как Singleton
        /// </summary>
        [Register(Lifetime = Lifetime.Transient)]
        public class TransientClass : IDependency
        {
            public void Dispose()
            {
                Console.WriteLine($"Dispose {this.GetType().Name}");
            }
        }

        public interface IClass1
        {

        }

        public interface IClass3
        {

        }

        #endregion
    }
}
