using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.NUnitTest.Tests.Sequences
{
    [Table(Name = "EntityUNID", Schema = "dbo", Database = "CoreTest")]
    public class EntityUnIdEnity
    {
        [Column(Name = "UNID")]
        [Identity]
        [PrimaryKey]
        public int Id { get; set; }

        [Column]
        public int TableId { get; set; }

        [Association(ThisKey = nameof(TableId), OtherKey = nameof(TableEntity.Id), CanBeNull = false)]
        public TableEntity Table { get; set; }
    }
}
