using NLog;
using System;

namespace Core.Log
{
    public class Logger<T> 
    {
        private readonly ILogger _log;

        public Logger()
        {
            _log = LogManager.GetLogger(typeof(T).Name);
        }

        #region Методы логирования

        /// <summary>
        /// Расширенное логирование
        /// </summary>
        /// <param name="logEventInfo"></param>
        public void Log(LogEventInfo logEventInfo)
        {
            _log.Log(logEventInfo);
        }

        /// <summary>
        /// логирование критических ошибок
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(string message)
        {
            _log.Fatal(message);
        }

        /// <summary>
        /// логирование критических ошибок
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(string message, Exception exception)
        {
            _log.Fatal(exception, message);
        }

        /// <summary>
        /// Логирование ошибок
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            _log.Error(message);
        }

        /// <summary>
        /// Логирование ошибок
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception)
        {
            _log.Error(exception, message);
        }

        /// <summary>
        /// Логирование прочей информации
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            _log.Info(message);
        }
        public void Info(string message, Exception exception)
        {
            _log.Info(exception, message);
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            _log.Debug(exception, message);
        }

        /// <summary>
        /// логирование всего подряд
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message)
        {
            _log.Trace(message);
        }

        /// <summary>
        /// логирование всего подряд
        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message, Exception exception)
        {
            _log.Trace(exception, message);
        }

        #endregion

    }

}
