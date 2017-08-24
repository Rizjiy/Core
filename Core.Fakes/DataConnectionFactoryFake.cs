using Core.Internal.LinqToDB;
using Core.LinqToDB.Interfaces;
using LightInject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Fakes
{
    /// <summary>
    /// Этим классом нужно подменить интерфейс IDataConnectionFactory
    /// </summary>
    public class DataConnectionFactoryFake: IDataConnectionFactory
    {
        private readonly IServiceContainer _container;

        public DataConnectionFactoryFake(IServiceContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Возвращает фейковый контекст
        /// </summary>
        /// <param name="configurationString">Не используется</param>
        /// <returns></returns>
        public IDataConnection GetDataConnection(string configurationString)
        {
            return new DataContextFake(_container);
        }
    }
}
