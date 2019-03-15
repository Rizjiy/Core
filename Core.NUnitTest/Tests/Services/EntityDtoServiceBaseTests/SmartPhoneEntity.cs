using Core.Domain;
using LinqToDB.Mapping;

namespace Core.NUnitTest.Tests.Services.EntityDtoServiceBaseTests
{
    [Table(Name = "SmartPhone", Schema = "dbo", Database = "CoreTest")]
    public class SmartPhoneEntity : EntityBase
    {
        [Column]
        [Identity]
        [PrimaryKey]
        public override int Id { get; set; }

        [Column]
        public string Model { get; set; }

        [Column]
        public string Make { get; set; }

        [Column]
        public int Year { get; set; }
    }
}
