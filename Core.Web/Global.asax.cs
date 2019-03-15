using System;
using System.Web;
using System.Web.Http;
using Core.Caching.Implementation;
using Core.Caching.Interface;
using LightInject;
using Core.Internal.Dependency;
using Core.Web.Cache;

namespace Core.Web
{
    public class Global : HttpApplication
    {
        private void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(config =>
            {
                // Подключил атрибуты Route
                config.MapHttpAttributeRoutes();

                // Конвенция роутинга "по умолчанию", - если нет атрибута Route
                config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}", new { id = RouteParameter.Optional });

            });

            // Инициализация сервисной инфраструктуры: DataConnection и Dependency Injections
            new DependencyInitializer()
                .ForAssembly(typeof(Core.AspxUtils).Assembly)
                .Init((dbConfig, container) =>
            {
                container.RegisterApiControllers();
                container.EnableWebApi(GlobalConfiguration.Configuration);

                container.Register<ICacheProvider, InMemoryCacheProvider>(new PerContainerLifetime());

                //запуск постоянного автообновления кеша
                container.GetInstance<UserCache>().SetIntervalUpdate(TimeSpan.FromSeconds(1));

            });
        }
    }
}