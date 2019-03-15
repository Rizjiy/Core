using AutoMapper;
using Core.Domain;
using Core.Dto;
using Core.Interfaces;
using Core.Services;
using LinqToDB.Mapping;

namespace Core.NUnitTest.Tests.GetQueryResultDtoTests
{
    [Table(Name = "Car", Schema = "dbo", Database = "CoreTest")]
    public class CarEntity : EntityBase
    {
        [Column]
        public override int Id { get; set; }

        [Column]
        public string Model { get; set; }

        [Column]
        public string Make { get; set; }

        [Column]
        public int Year { get; set; }
    }

    public class CarListDto : BaseListDto
    {
        public string Model { get; set; }

        public string Make { get; set; }

        public int Year { get; set; }
    }

    public class CarEntityProfile : Profile, IDependency
    {
        public CarEntityProfile()
        {
            CreateMap<CarEntity, CarListDto>();
        }
    }

    public class CarEntityService : ReadonlyServiceBase<CarEntity, CarListDto, object>
    {

    }
}
