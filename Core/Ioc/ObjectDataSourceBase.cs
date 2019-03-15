using Core.Internal.Dependency;
using Core.Internal.LinqToDB;
using LightInject;
using System;

namespace Core.Ioc
{
    public abstract class ObjectDataSourceBase : IAspNetDependency, IDisposable
    {
        public IServiceContainer ContainerDi { get; set; }
        public Scope Scope { get; set; }

        public ObjectDataSourceBase()
        {
            this.InitContainer();
        }

        public void Dispose()
        {
            this.FreeScope();
        }
    }
}
