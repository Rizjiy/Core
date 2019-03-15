using Core.Interfaces;
using Core.Internal.Dependency;
using LightInject;
using NUnit.Framework;

namespace Core.NUnitTest.Tests.LightInject
{
    /// <summary>
    /// Тестируем регистрацию класса для интерфейса:
    /// По равенству наименований интерфейса и класса наслединка
    /// Пример - Интерфейс: ISomeLogic, Имя класса: SomeLogic
    /// </summary>
    [TestFixture]
    public class InterfaceRegistrationTests
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
        }

        [Test]
        public void GetImplementionOnInterface()
        {
            using (_container.BeginScope())
            {
                var car = _container.GetInstance<ICar>();


                Assert.AreEqual(typeof(Car), car.GetType());
              
            }
        }
    }

    public interface ICar
    {
        int GetSpeed();
    }


    /// <summary>
    ///  Должен зарезолвится именно этот класс
    /// </summary>
    public class Car : ICar, IDependency
    {
        int ICar.GetSpeed()
        {
            return 100;
        }
    }

    /// <summary>
    /// Этот класс зарезолвится не должен
    /// </summary>
    public class Car2 : ICar, IDependency
    {
        int ICar.GetSpeed()
        {
            return 2000;
        }
    }


}
