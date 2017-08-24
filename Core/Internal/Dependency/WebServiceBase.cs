using System;
using System.ComponentModel;
using System.Web.Services;
using Core.Internal.LinqToDB;
using LightInject;

namespace Core.Internal.Dependency
{
    [ToolboxItem(false)]
    public abstract class WebServiceBase : WebService, IAspNetDependency
    {
        #region DI
        public IServiceContainer ContainerDi { get; set; }
        public Scope Scope { get; set; }
        public DataConnectionFactory ConnectionFactory { get; set; }

        protected WebServiceBase()
        {
            this.InitContainer();
            Disposed += WebServiceBase_Disposed;
        }

        private void WebServiceBase_Disposed(object sender, EventArgs e)
        {
            this.FreeScope();
        }

        public void Register(IServiceContainer container)
        {
        }
        #endregion
    }
}
