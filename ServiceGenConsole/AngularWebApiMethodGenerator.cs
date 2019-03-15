using Core.Web.Core;
using ServiceGenConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Routing;

namespace ServiceGenConsole
{
    public class AngularWebApiMethodGenerator
    {
        public List<Tuple<string, List<ApiMethodModel>>> GetData()
        {

            //var apiExplorer = GlobalConfiguration.Configuration.Services.GetApiExplorer();
            var conf = new HttpConfiguration();
            WebApiConfig.Register(conf);
            conf.EnsureInitialized();
            var apiExplorer = conf.Services.GetApiExplorer();
            var groupDescriptions =
                apiExplorer.ApiDescriptions.
                Where(ad=>ad.ActionDescriptor.ControllerDescriptor.ControllerType.CustomAttributes.All(a => a.AttributeType != typeof(IgnoreServiceGenAttribute))).
                Select(ad => new ApiMethodModel(ad))
                    .OrderBy(m => m.ControllerName)
                    .GroupBy(m => m.ControllerName)
                    .Select(g => Tuple.Create(g.Key, g.GroupBy(e => e.MethodName).Select(grp => grp.Select(
                (gg, index) =>
                {
                    gg.MethodName += index == 0 ? "" : (index - 1).ToString();
                    return gg;
                }
            )).SelectMany(m => m).OrderBy(m => m.MethodName).ToList())).ToList();
            return groupDescriptions;
        }
    }
}