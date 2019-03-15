using AutoMapper;
using Core.Interfaces;

namespace Core.NUnitTest.Tests.LoadTests
{
    /// <summary>
    ///  Мэппинг тестовой сущности в тестовую ДТО
    /// </summary>
    public class BankCardTestEntityProfile : Profile, IDependency
    {
        public BankCardTestEntityProfile()
        {
            CreateMap<BankCardTestEntity, BankCardTestDto>()
                .ForMember(dto => dto.OwnerFirstname,
                    src => src.MapFrom(entity => entity.Owner.Firstname))
                .ForMember(dto => dto.OwnerSurname,
                    src => src.MapFrom(entity => entity.Owner.Surname));
        }
    }
}
