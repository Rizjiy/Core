using Core.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Core.Web.Controllers.Demo
{
    [IgnoreServiceGen]
    public class NonGeneratingController: ApiController
    {
        [HttpGet]
        public void GetMethod1(int a)
        {

        }

        [HttpPost]
        public void PostMethod1(object b)
        {

        }
    }
}