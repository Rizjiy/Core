using Core.Domain;
using LinqToDB.Mapping;

namespace Core.NUnitTest.Tests.Services
{
    [Table(Name = "Test", Schema = "dbo", Database = "CoreTest")]
    public class TestEntity : EntityBase
    {
        [Column]
        public override int Id
        {
            get; set;
        }

        [Column]
        public string Inn { get; set; }
    }
}
