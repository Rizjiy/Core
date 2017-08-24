using System.Web.Http;
using LightInject;
using System;
using System.Diagnostics;
using Core.Log;

namespace Core.Internal.Dependency
{
	public static class DependencyExtentions
	{
		public static void InitContainer(this IAspNetDependency dependency)
		{
			dependency.ContainerDi = GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IServiceContainer)) as IServiceContainer;
            try
            {
                dependency.Scope = dependency.ContainerDi.BeginScope();
                dependency.ContainerDi.InjectProperties(dependency);
            }
            catch(Exception ex)
            {
				Debug.WriteLine($"{typeof(DependencyExtentions).Namespace}\r\n{ex}");
            }

        }

		public static void FreeScope(this IAspNetDependency dependency)
		{
			if (dependency.Scope != null && dependency.Scope.ChildScope == null)
				dependency.Scope.Dispose();
            else
            {
                var log = dependency.ContainerDi?.GetInstance<Logger<IAspNetDependency>>();
                log?.Fatal($"Метод FreeScope не уничтожил объект! Type: {dependency.GetType()} ChildScope: {dependency.Scope?.ChildScope}");
            }
		}
	}
}
