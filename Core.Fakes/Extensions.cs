using Core.Internal.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightInject;
using Core.LinqToDB.Interfaces;

namespace Core.Fakes
{
    public static class Extensions
    {

        /// <summary>
        /// Зарегистрировать коллекцию значений
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">Коллекция значений</param>
        public static void SetTable<T>(this IServiceContainer container, IList<T> table)
        {
            container.RegisterInstance(table);
        }

        /// <summary>
        /// Метод устанавливает вместо БД фейковый контекст
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        public static void SetFakeContext(this IServiceContainer container)
        {
            //Регистрируем DataConnectionFactory
            container.Register<IDataConnectionFactory, DataConnectionFactoryFake>(new PerContainerLifetime());

        }
    }
}
