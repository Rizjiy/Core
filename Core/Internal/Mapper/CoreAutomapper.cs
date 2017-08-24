using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Internal.Mapper
{
    public class CoreAutomapper : IMapperCore
    {
        private IMapper mapper;

        public CoreAutomapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TDestination Map<TDestination>(object source)
        {
            return mapper.Map<TDestination>(source);
        }

    }
}
