using AutoMapper;
using Core.Interfaces;

namespace Core.NUnitTest.Tests.Sequences
{
    public class PersonProfile : Profile, IDependency
    {
        public PersonProfile()
        {
            CreateMap<PersonDto, PersonEntity>()
                .ForMember(entity => entity.Id, option => option.Ignore());
        }
    }
}
