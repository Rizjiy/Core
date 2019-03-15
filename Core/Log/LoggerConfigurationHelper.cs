using NLog.Common;
using System.Web;

namespace Core.Log
{
    /// <summary>
    /// Статический класс содержит вспомогательные методы для конфигурации логгера
    /// </summary>
    public static class LoggerConfigurationHelper
    {
        /// <summary>
        /// Включает логирование внутренних ошибок логгера. 
        /// Внутренние ошибки логируются в файл "[Папка сайта]/Temp/nlog-internal.log"
        /// </summary>
        public static void SetInternalLogging()
        {
            //путь к файлу лога. ps. Только так, в коде. Сослаться на папку сайта через конфиг NLog способ не найден.
            InternalLogger.LogFile = HttpContext.Current.Server.MapPath("Temp/nlog-internal.log"); 

            //Во внутренний лог будет писатьс информация только о внутренних ошибках логгера. (уровень логирования подобран эксперементально, т.к. нет четкой документации по данному аспекту NLog)
            InternalLogger.LogLevel = NLog.LogLevel.Error; 
        }
    }
}
