using System;
using Core.Internal.Dependency;
using Core.Internal.LinqToDB;
using LightInject;

namespace Core.Ioc
{
    public abstract class MasterPageBase : System.Web.UI.MasterPage, IAspNetDependency
    {
        public IServiceContainer ContainerDi { get; set; }
        public Scope Scope { get; set; }



        public MasterPageBase()
            : base()
        {
            this.InitContainer();
        }

        public override void Dispose()
        {
            this.FreeScope();
            base.Dispose();
        }



    }
}
