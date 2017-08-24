using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    /// <summary>
    /// Ошибка логики
    /// </summary>
    public class LogicException : Exception
    {
        public LogicException(string message)
            : base(message)
        {

        }

        public LogicException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }

    /// <summary>
    /// Данные не найдены
    /// </summary>
    public class NotFoundException : System.Exception
    {
        public NotFoundException()
        {

        }

        public NotFoundException(string message) : base(message)
        {

        }
    }

}
