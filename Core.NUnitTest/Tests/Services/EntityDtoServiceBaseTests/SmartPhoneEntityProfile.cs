using AutoMapper;
using Core.Interfaces;

namespace Core.NUnitTest.Tests.Services.EntityDtoServiceBaseTests
{
    public class SmartPhoneEntityProfile : Profile, IDependency
    {
        public SmartPhoneEntityProfile()
        {
            // Мэппинг и сущности в дто
            CreateMap<SmartPhoneEntity, SmartPhoneDto>();

            // Мэппинг из дто в сущность
            CreateMap<SmartPhoneDto, SmartPhoneEntity>();
        }
    }
}
