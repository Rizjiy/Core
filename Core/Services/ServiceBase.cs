using AutoMapper;
using Core.Interfaces;
using Core.Log;
using Core.Services.Other;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Core.Services
{
    /// <summary>
    /// Базовый сервис. Происходит определение контекста БД
    /// </summary>
    public abstract class ServiceBase : IDisposable, IDependency
    {
        /// <summary>
        /// Режим для авто-тестов
        /// </summary>
        public static volatile bool TestMode = false;

        public DateTimeService DateTimeService { get; set; }

        public Logger<ServiceBase> Logger { get; set; }
        public IMapper Mapper { get; set; }


        public virtual void Dispose()
        {
           
        }


        /// <summary>
        /// Фабрика соединенй БД
        /// </summary>
        public IDataConnectionFactory ConnectionFactory { get; set; }

        private IDataConnection _dataContext;
        /// <summary>
        /// Соединение БД
        /// </summary>
        public IDataConnection DataContext
        {
            get
            {
                if (_dataContext == null)
                {
                    _dataContext = ConnectionFactory.GetDataConnection(GetConnectionName());
                }
                return _dataContext;
            }
            set
            {
                _dataContext = value;
            }
        }

        /// <summary>
        /// Словарь соответствия неймспейсов и названий коннекшенов
        /// </summary>
        internal static readonly IDictionary<string, string> NamespaceConnectionDict = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Получаю название коннекшена по словарю соответствия неймспейсов и типу текущего экземпляра сервиса
        /// При множествоенном попадании отбирается наиболее полное соответствие
        /// </summary>
        public string GetConnectionName()
        {
            var ns = GetType().Namespace + ".";
            var key = NamespaceConnectionDict
                .Keys
                .OrderByDescending(k => k.Length)
                .FirstOrDefault(k => ns.StartsWith(k + "."));
            var value = string.IsNullOrWhiteSpace(key) ? "default" : NamespaceConnectionDict[key];
            return value;
        }
    }
}
