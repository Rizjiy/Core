using System;
using Core.Validation;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Core.Internal.Dependency;
using Core.Log;
using LightInject;
using System.IO;
using NLog;
using System.Web;
using System.Linq;
using Core.Utils;

namespace Core.Internal.ExceptionHandling
{
    public class GlobalExceptionHandler : ExceptionHandler, IDependency
    {
        public static void DependencyRegister(IServiceContainer container)
        {
            container.Register(typeof(GlobalExceptionHandler), new PerContainerLifetime());
        }

        public Logger<GlobalExceptionHandler> Logger { get; set; }

        public override void Handle(ExceptionHandlerContext context)
        {
            var result = new HttpResponseMessage();

            var ex = context.Exception;

            var request = HttpContext.Current.Request;
            string fiddlerString = request.ToFiddlerString();

            //Собираем логирование
            LogEventInfo logEventInfo = new LogEventInfo();
            logEventInfo.LoggerName = this.GetType().Name;
            logEventInfo.Properties["Request"] = fiddlerString;
            logEventInfo.Exception = ex;

            var failureEx = ex as ValidationFailureException;

            if (failureEx != null)
            {
                //Ошибка валидации
                logEventInfo.Level = NLog.LogLevel.Info;

                var sb = new StringBuilder();
                failureEx.FailureList.Violations.ForEach(e => 
                {
                    sb.AppendLine(e.Message);
                });

                logEventInfo.Message = sb.ToString();

                result.StatusCode = HttpStatusCode.Conflict; //409
                result.Content = new StringContent(failureEx.FailureList.ToJson());
            }
            else
            {
                var guid = Guid.NewGuid();
                logEventInfo.Level = NLog.LogLevel.Error;
                logEventInfo.Message = ($"Id ошибки: {guid}");

                var customEx = new Exception($"{ex.Message}<br />Id ошибки: <b>{guid}</b>", ex);

                result.StatusCode = HttpStatusCode.InternalServerError; //500
                result.Content = new StringContent(customEx.ToJson());
            }

            context.Result = new ExceptionResult(result);

            Logger?.Log(logEventInfo);
        }

        public class ExceptionResult : IHttpActionResult
        {
            private readonly HttpResponseMessage _httpResponseMessage;

            public ExceptionResult(HttpResponseMessage httpResponseMessage)
            {
                _httpResponseMessage = httpResponseMessage;
            }

            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(_httpResponseMessage);
            }
        }

    }
}
