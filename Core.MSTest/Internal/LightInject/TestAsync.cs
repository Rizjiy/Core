using Core.Internal.Dependency;
using Core.Internal.LinqToDB;
using LightInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.MSTest.Internal.LightInject
{
    [TestClass]
    public class TestAsync
    {
        [TestMethod]
        public void GetInstanceAsync()
        {
            var container = new ServiceContainer();
            container.Register<A>(new PerScopeLifetime());
            container.Register<B>(new PerScopeLifetime());

            Task task;

            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<A>();

                task = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning).StartNew(() =>
                {
                    using(container.BeginScope())
                    {
                        Thread.Sleep(10);
                        var instance2 = container.GetInstance<B>();
                    }
                });

            }

            task.Wait();

        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void GetInstanceAsync_Exeption()
        {
            var container = new ServiceContainer();
            container.Register<A>(new PerScopeLifetime());

            Task task;

            using (container.BeginScope())
            {
                var instance1 = container.GetInstance<A>();

                task = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning).StartNew(() =>
                 {
                     {
                         Thread.Sleep(100);
                         //Скоп уничтожился в основном треде и объект мы не получим
                         var instance2 = container.GetInstance<A>();
                     }
                 });

            }

            task.Wait();

        }

        [TestMethod]
        public void GetInstanceWebFormsAsync()
        {
            var container = new ServiceContainer();
            container.Register<WebFormsClass>(new PerScopeLifetime());
            container.Register<B>(new PerScopeLifetime());

            Task task;

            var scope1 = container.BeginScope();
            {
                var instance1 = container.GetInstance<WebFormsClass>();
                instance1.Scope = scope1;

                task = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning).StartNew(() =>
                {
                    using (var scope2 = container.BeginScope())
                    {
                        Thread.Sleep(1000);
                        var instance2 = container.GetInstance<B>();
                    }
                });

                //scope1 должен уничтожится с объектом instance1
                instance1.Dispose();
            }

            task.Wait();

        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void GetInstanceWebFormsAsync_Exception()
        {
            var container = new ServiceContainer();
            container.Register<WebFormsClass>(new PerScopeLifetime());
            container.Register<B>(new PerScopeLifetime());

            Task task;

            var scope1 = container.BeginScope();
            {
                var instance1 = container.GetInstance<WebFormsClass>();
                instance1.Scope = scope1;

                //думаю, что scope2 будет дочерним
                var scope2 = container.BeginScope();

                task = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning).StartNew(() =>
                {
                    Thread.Sleep(1000);
                    var instance2 = container.GetInstance<B>();
                });

                //Здесь должна быть ошибка, потому что нельзя уничтожать объект с дочерним скопом
                instance1.Dispose();
                scope2.Dispose();
            }

            task.Wait();

        }



        public class A: IDisposable
        {
            public void Dispose()
            {
                Console.WriteLine($"Dispose {this.GetType().Name}");
            }
        }

        public class B : IDisposable
        {
            public void Dispose()
            {
                Console.WriteLine($"Dispose {this.GetType().Name}");
            }
        }

        public class WebFormsClass : IAspNetDependency
        {
            public IServiceContainer ContainerDi { get; set; }
            public Scope Scope { get; set; }
            public DataConnectionFactory ConnectionFactory { get; set; }

            public void Dispose()
            {
                this.FreeScope();
                Console.WriteLine($"Dispose {this.GetType().Name}");
            }
        }


    }
}
