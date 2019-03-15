using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public FunctionType FunctionType { get; set; }

        public ApiMethodModel(ApiDescription apiDescription)
        {
            Method = apiDescription.HttpMethod.Method;
            MethodName = (apiDescription.ActionDescriptor as ReflectedHttpActionDescriptor)?.MethodInfo.Name;

            int urlLenth = apiDescription.RelativePath.IndexOf('?') == -1 ? apiDescription.RelativePath.Length : apiDescription.RelativePath.IndexOf('?');
            Url = apiDescription.RelativePath.Substring(0, urlLenth); //отсекли параметры

            ControllerName = apiDescription.ActionDescriptor.ControllerDescriptor.ControllerName;
            ActionName = apiDescription.ActionDescriptor.ActionName;
            ParameterDescriptions = apiDescription.ParameterDescriptions;

            var parList = new List<string>();
            foreach (var parameter in ParameterDescriptions.Where(p => p.Source == ApiParameterSource.FromUri))
            {
                parList.Add($"{parameter.Name}: {parameter.Name}");
            }
            if (parList.Any())
                Params = "{" + $"{string.Join(",", parList)}" + "}";
            ParametersString = string.Join(", ", ParameterDescriptions.Select(p => p.Name));

            var nonUriParam = ParameterDescriptions.FirstOrDefault(p => p.Source != ApiParameterSource.FromUri);
            if (nonUriParam != null)
            {
                //Определяем тип параметра, и если  это DataSourceRequest, то используем функцию dsRead(url, options, filter)
                if (typeof(IDataSourceRequest).IsAssignableFrom(nonUriParam.ParameterDescriptor.ParameterType))
                {
                    FunctionType = FunctionType.DsRead;
                }

                //Если тип возвращаемого параметра IHttpResponseFile, то загрузка файла методом core.dsDownload
                if (typeof(IHttpResponseFile).IsAssignableFrom(apiDescription.ResponseDescription.DeclaredType))
                {
                    FunctionType = FunctionType.DsDownload;
                }

                NonUriParam = nonUriParam.Name;
            }
        }
    }

}