using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils.CustomeException
{
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
