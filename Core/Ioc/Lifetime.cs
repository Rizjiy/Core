using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Ioc
{
    /// <summary>
    /// 
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// каждый раз новый объект
        /// </summary>
        Transient = 0,

        /// <summary>
        /// Один в рамках скопа
        /// </summary>
        PerScope = 1,

        /// <summary>
        /// Один в рамках контейнера
        /// </summary>
        PerContainer = 2,
    }
}
