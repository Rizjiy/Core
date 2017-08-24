using ServiceGen.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace ServiceGenConsole.Models
{
    public class ApiMethodModel
    {
        
        public string Method { get; set; }
        public string MethodName { get; set; }
        public string Url { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public IEnumerable<ApiParameterDescription> ParameterDescriptions { get; set; }

        public string NonUriParam { get; set; }

        public string ParametersString { get; set; }

        public string Params { get; set; }

        public bool IsDsRead { get; set; }

        public ApiMethodModel(ApiDescription apiDescription)
        {
            Method = apiDescription.HttpMethod.Method;
            MethodName = (apiDescription.ActionDescriptor as ReflectedHttpActionDescriptor)?.MethodInfo.Name;
            Url = apiDescription.RelativePath;
            ControllerName = apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName;
            ActionName = apiDescription.ActionDescriptor.ActionName;
            ParameterDescriptions = apiDescription.ParameterDescriptions;

            var parList = new List<string>();
            foreach (var parameter in ParameterDescriptions.Where(p => p.Source == ApiParameterSource.FromUri))
            {

                if (Url.Contains("{" + parameter.Name + "}"))
                    Url = Url.Replace("{" + parameter.Name + "}", "");

                parList.Add($"{parameter.Name}: {parameter.Name}");
            }
            if (parList.Any())
                Params = "{" + $"{string.Join(",", parList)}" + "}";
            ParametersString = string.Join(", ", ParameterDescriptions.Select(p => p.Name));

            //Определяем тип параметра, и если  это DataSourceRequest, то используем функцию dsRead(url, options, filter)
            var nonUriParam = ParameterDescriptions.FirstOrDefault(p => p.Source != ApiParameterSource.FromUri);
            if (nonUriParam != null)
            {
                IsDsRead = typeof(IDataSourceRequest).IsAssignableFrom(nonUriParam.ParameterDescriptor.ParameterType);
                //IsDsRead = nonUriParam.ParameterDescriptor.ParameterType.Name.Contains(Constants.DsReadParameterTypeName);
                NonUriParam = nonUriParam.Name;
            }
        }
    }

}