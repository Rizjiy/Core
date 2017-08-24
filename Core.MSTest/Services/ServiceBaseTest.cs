using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Internal.Dependency;

namespace Core.MSTest.Services
{
    [TestClass]
    public class ServiceBaseTest
    {
        [TestInitialize]
        public void Init()
        {
            
        }

        public void SingleServiceBaseInit()
        {
            var initWorks = false;
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    initWorks = true;
                Assert.IsNotNull(dbConfig);
                Assert.IsNotNull(container);
                Console.WriteLine($"{typeof(Core.Services.ServiceBase).Namespace}{nameof(DependencyInitializer.Init)}(); ManagedThreadId=={Thread.CurrentThread.ManagedThreadId}");
            });
            Assert.IsTrue(initWorks);
        }

        [TestMethod]
        public void TestInit()
        {
            Parallel.ForEach(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, i =>
               {
                   Console.WriteLine($"{GetType().Namespace}{nameof(SingleServiceBaseInit)}(); Id=={i}, ManagedThreadId=={Thread.CurrentThread.ManagedThreadId}");
                   SingleServiceBaseInit();
                   Thread.Sleep(1);
               });

        }
    }
}
