using Core.Internal.Dependency;
using LightInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Reflection;

namespace Core.MSTest.Services.ServiceBaseTests
{
    [TestClass]
    public class CustomDependencyRegisterTest
    {
        private DependencyInitializer.DependencyRegister _dependencyRegister;
        private ServiceContainer _container;

        public class DependencyStubRegister : IDependency
        {
            public static void DependencyRegister(IServiceContainer container)
            {
                container.Register<NotDependencyStub>();
            }
        }

        public class NotDependencyStub
        {
        }

        [TestInitialize]
        public void Init()
        {
            _container = new ServiceContainer();
            _dependencyRegister = new DependencyInitializer.DependencyRegister();
        }


        /// <summary>
        /// Проверяю автоматическую регистрацию зависимостей в контейнер
        /// </summary>
        [TestMethod]
        public void Test()
        {
            var notFound = _container.CanGetInstance(typeof(NotDependencyStub), string.Empty);
            Assert.IsFalse(notFound);

            _dependencyRegister.RegisterAssemblyDependencies(_container, new List<Assembly> { GetType().Assembly });

            var found = _container.CanGetInstance(typeof(NotDependencyStub), string.Empty);
            Assert.IsTrue(found);

            var stub =_container.GetInstance<NotDependencyStub>();
            Assert.IsNotNull(stub);
            Assert.IsInstanceOfType(stub, typeof(NotDependencyStub));
        }

        [TestCleanup]
        public void Clean()
        {
            _dependencyRegister = null;
            _container.Dispose();
        }
    }
}
