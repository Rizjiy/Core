using Core.Dto;
using Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.NUnitTest.Tests.Services
{
    public class TestEntityService : ReadonlyEntityServiceBase<TestEntity, BaseListDto, EntityDto, TestDto>
    {
    }
}
