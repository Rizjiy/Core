using System;
using Core.Internal.LinqToDB;
using LightInject;

namespace Core.Internal.Dependency
{
    public abstract class WebFormsPageBase : System.Web.UI.Page, IAspNetDependency
    {
        #region DI
        public IServiceContainer ContainerDi { get; set; }
        public Scope Scope { get; set; }
        public DataConnectionFactory ConnectionFactory { get; set; }

	    protected override void OnPreInit(EventArgs e)
	    {
			this.InitContainer();
			base.OnPreInit(e);
	    }

        public override void Dispose()
        {
            this.FreeScope();
            base.Dispose();
        }

        #endregion
    }
}
