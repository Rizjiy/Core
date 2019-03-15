using Core.Domain;
using Core.Services;
using LinqToDB.Mapping;
using System;

namespace Core.NUnitTest.Tests.CRUD
{
    [Table(Name = "Crud", Database = "CoreTest", Schema = "dbo")]
    public class CrudEntity : EntityBase
    {
        [Column]
        [PrimaryKey]
        public override int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public int Year { get; set; }

        [Column]
        public DateTime BirthDate { get; set; }
    }

    public class CrudEntityService : LegacyEntityServiceBase<CrudEntity> { }
}
