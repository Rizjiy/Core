using System;
using System.Threading.Tasks;
using Core.Interfaces;
using Core.Internal.Dependency;
using Core.Services;
using LightInject;

namespace Core.Services.Other
{
    /// <summary>
    /// Сервис для выполнения работы в фоновом потоке. Создается новый скоп
    /// </summary>
    public class AsyncService : IDependency
    {
        public IServiceContainer Container { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TService">Сервис, к инстансу которого нужно обратиться асинхронно по принципу "запустил и забыл".</typeparam>
        /// <param name="action">Код, собственно который и будет вызван асинхронно</param>
        public Task RunAsync<TService>(Action<TService> action)
        {
            return Task.Factory.StartNew(() => { 
                //Нужно открыть свой скоп, т.к. код выполняется в потоке, который вероятно переживет родительский поток.
                using (Container.BeginIndependentScope())
                {
                    var service = Container.GetInstance<TService>();
                    action(service);

                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">Код, собственно который и будет вызван асинхронно. Создается новый скоп</param>
        public Task RunAsync(Action<IServiceContainer> action)
        {
            return Task.Factory.StartNew(() => {
                //Нужно открыть свой скоп, т.к. код выполняется в потоке, который вероятно переживет родительский поток.
                using (Container.BeginIndependentScope())
                {
                    action(Container);
                }
            });
        }

    }
}