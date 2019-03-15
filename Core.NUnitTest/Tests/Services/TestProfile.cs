using AutoMapper;
using Core.Interfaces;
using Core.Internal.Dependency;

namespace Core.NUnitTest.Tests.Services
{
    public class TestProfile : Profile, IDependency
    {
        public TestProfile()
        {
            CreateMap<TestEntity, TestDto>();
        }
    }
}
