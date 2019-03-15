using Core.Domain;
using LinqToDB.Mapping;

namespace Core.NUnitTest.Tests.Services.EntityServiceBaseTests
{
    [Table(Name = "Address", Schema = "dbo", Database = "CoreTest")]
    public class AddressEntity : EntityBase
    {
        [Column]
        [PrimaryKey]
        [Identity]
        public override int Id { get; set; }

        [Column]
        public int BuildingNumber { get; set; }

        [Column]
        public string Street { get; set; }

        [Column]
        public string City { get; set; }
    }
}
