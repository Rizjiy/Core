using System.Collections.Generic;
using LightInject;
using Core.Services;

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
        /// Получить зарегистрированную коллекцию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <returns></returns>
        public static IList<T> GetTable<T>(this IServiceContainer container)
        {
            return container.GetInstance<IList<T>>();
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
