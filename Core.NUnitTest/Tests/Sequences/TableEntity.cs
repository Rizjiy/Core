using LinqToDB.Mapping;

namespace Core.NUnitTest.Tests.Sequences
{
    [Table(Name = "Table", Schema = "dbo", Database = "CoreTest")]
    public class TableEntity
    {
        [Column(Name = "TableId")]
        [PrimaryKey]
        [Identity]
        public int Id { get; set; }

        [Column]
        public string TableName { get; set; }
    }
}
