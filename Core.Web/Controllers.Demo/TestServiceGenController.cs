using Core.Web.Controllers.Demo.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace Core.Web.Controllers.Demo
{
    /// <summary>
    /// Контроллер для тестирования методов утилиты ServiceGenConsole
    /// </summary>
    public class TestServiceGenController: ApiController
    {
        [HttpGet]
        public HttpResponseMessage ExcelExport(DateTime startDatePeriod, DateTime endDatePeriod)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            return result;
        }

        [HttpPost]
        public void Save(TimeoutDto dto)
        {
            Thread.Sleep(dto.Timeout * 1000);

        }



    }
}