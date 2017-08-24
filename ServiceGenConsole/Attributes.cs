using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Core.Web.Core
{
    /// <summary>
    /// Контроллер, помеченный этим классом не будет генерировать js-сервис
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreServiceGenAttribute: Attribute
    {
    }
}