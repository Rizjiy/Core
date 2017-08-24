using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Internal.Mapper
{
    public interface IMapperCore
    {
        TDestination Map<TDestination>(object source);
    }
}
