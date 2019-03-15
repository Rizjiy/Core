using System;
using System.ComponentModel;
using System.Web.Services;
using Core.Internal.Dependency;
using Core.Internal.LinqToDB;
using LightInject;

namespace Core.Ioc
{
    [ToolboxItem(false)]
    public abstract class WebServiceBase : WebService, IAspNetDependency
    {
        public IServiceContainer ContainerDi { get; set; }
        public Scope Scope { get; set; }

        protected WebServiceBase()
        {
            this.InitContainer();
            Disposed += WebServiceBase_Disposed;
        }

        private void WebServiceBase_Disposed(object sender, EventArgs e)
        {
            this.FreeScope();
        }

    }
}
