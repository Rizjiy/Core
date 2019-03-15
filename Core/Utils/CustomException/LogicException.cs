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
    public class LogicException : System.Exception
    {
        public LogicException(string message)
            : base(message)
        {

        }

        public LogicException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }
    }

  

}
