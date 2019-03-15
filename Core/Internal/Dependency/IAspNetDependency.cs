using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Internal.LinqToDB;
using LightInject;

namespace Core.Internal.Dependency
{
    /// <summary>
    /// Интерфейс имплементируется старыми классами WebForms, 
    /// </summary>
	public interface IAspNetDependency
	{
		IServiceContainer ContainerDi { get; set; }
		Scope Scope { get; set; }
	}
}
