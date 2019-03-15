using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGenConsole.Models
{
    /// <summary>
    /// влияет на генерируемую функцию
    /// </summary>
    public enum FunctionType
    {
        /// <summary>
        /// core.dsRead(url, options, filter)
        /// </summary>
        DsRead = 1,
        /// <summary>
        /// core.dsDownload(url, options, filter)
        /// </summary>
        DsDownload = 2
    }
}
