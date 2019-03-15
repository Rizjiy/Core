using System;

namespace Core.Domain.Attributes
{
    /// <summary>
    /// Атрибут для логирования изменений в сущностях(таблицах в бд).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LogTableAttribute : Attribute
    {
        public string LogTableName { get; set; }

        public LogTableAttribute() { }
        public LogTableAttribute(string logTableName)
        {
            LogTableName = logTableName;
        }
    }
}
