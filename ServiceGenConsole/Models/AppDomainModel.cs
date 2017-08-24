using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace ServiceGenConsole.Models
{
    public class AppDomainModel
    {
        /// <summary>
        /// Модуль приложения Angular js
        /// </summary>
        public string AppModule { get; set; }

        /// <summary>
        /// Наименование контроллера и коллекция методов
        /// </summary>
        public List<Tuple<string, List<ApiMethodModel>>> Data { get; set; }
    }
}