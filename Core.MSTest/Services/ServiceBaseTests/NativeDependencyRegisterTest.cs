using Core.Internal.Dependency;
using LightInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Core.MSTest.Services.ServiceBaseTests
{
    [TestClass]
    public class NativeDependencyRegisterTest
    {
        private DependencyInitializer.DependencyRegister _dependencyRegister;
        private ServiceContainer _container;

        public class DependencyStubSimple : IDependency
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
            var notFound = _container.CanGetInstance(typeof(DependencyStubSimple), string.Empty);
            Assert.IsFalse(notFound);

            _dependencyRegister.RegisterAssemblyDependencies(_container, new List<Assembly> { GetType().Assembly });

            var found = _container.CanGetInstance(typeof(DependencyStubSimple), string.Empty);
            Assert.IsTrue(found);

            using (_container.BeginScope())
            {
                var stub = _container.GetInstance<DependencyStubSimple>();
                Assert.IsNotNull(stub);
                Assert.IsInstanceOfType(stub, typeof(DependencyStubSimple));
            }
        }

        [TestCleanup]
        public void Clean()
        {
            _dependencyRegister = null;
            _container.Dispose();
        }
    }
}
