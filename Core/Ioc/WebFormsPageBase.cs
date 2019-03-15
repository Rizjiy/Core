using System;
using Core.Internal.Dependency;
using Core.Internal.LinqToDB;
using LightInject;

namespace Core.Ioc
{
    public abstract class WebFormsPageBase : System.Web.UI.Page, IAspNetDependency
    {
        public IServiceContainer ContainerDi { get; set; }
        public Scope Scope { get; set; }

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

    }
}
