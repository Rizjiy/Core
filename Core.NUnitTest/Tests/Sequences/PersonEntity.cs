using System;
using Core.Domain;
using Core.Domain.Attributes;
using LinqToDB.Mapping;

namespace Core.NUnitTest.Tests.Sequences
{
    /// <summary>
    /// Фейковая сущность для тестирования получения Уникального Межтабличного Идентификатора(УМИ).
    /// Здесь атрибуты заполнены верно
    /// </summary>
    [Table(Name = "Person", Schema = "dbo", Database = "CoreTest")]
    public class PersonEntity : EntityBase
    {
        [Column]
        [PrimaryKey]
        [Euid]
        public override int Id
        {
            get;
            set;
        }


    }

    /// <summary>
    /// Фейковая сущность для тестирования получения Уникального Межтабличного Идентификатора(УМИ).
    /// Здесь атрибуты заполнены неверно - сочетание двух несовместимых атрибутов Euid и Identity
    /// </summary>
    [Table(Name = "Person", Schema = "dbo", Database = "CoreTest")]
    public class PersonEuidIdentityEntity : EntityBase
    {
        [Column]
        [PrimaryKey]
        [Euid]
        [Identity]
        public override int Id
        {
            get; set;
        }
    }

    /// <summary>
    /// Фейковая сущность для тестирования получения Уникального Межтабличного Идентификатора(УМИ).
    /// Здесь атрибуты заполнены неверно - атрибут EuidAttribute должен соседствовать с атрибутом PrimaryKeyAttribute
    /// </summary>

    [Table(Name = "Person", Schema = "dbo", Database = "CoreTest")]
    public class PersonOnlyEuidEntity : EntityBase
    {
        [Column]
        [Euid]
        public override int Id
        {
            get; set;
        }
    }

    [Table(Name = "Person", Schema = "dbo", Database = "CoreTest")]
    public class PersonOnlyIdentityEntity : EntityBase
    {
        [Identity]
        [Column]
        public override int Id { get; set; }
    }

}
