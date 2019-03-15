using Core.Domain;
using Core.Dto;
using Core.Services;
using LinqToDB.Mapping;

namespace Core.NUnitTest.Tests.LoadTests
{
    // Тестовые классы для тестирования мэпинга при загрузки дто ServiceBase.LoadDtoOrNull.

    /// <summary>
    /// Сущность человека, отсюда будут мэппится значния из свойств Firstname И Surname
    /// </summary>
    [Table(Name = "PersonTest", Schema = "dbo", Database = "CoreTest")]
    public class PersonTestEntity : EntityBase
    {
        [Column, PrimaryKey]
        public override int Id { get; set; }

        [Column]
        public string Firstname { get; set; }

        [Column]
        public string Surname { get; set; }
    }

    /// <summary>
    /// Сущность карты из нее будет мэппится ДТО карты
    /// </summary>
    [Table(Name = "BankCardTest", Schema = "dbo", Database = "CoreTest")] 
    public class BankCardTestEntity : EntityBase
    {
        [Column, PrimaryKey]
        public override int Id { get; set; }

        [Column]
        public int Year { get; set; }

        [Column]
        public int Month { get; set; }

        [Column]
        public int PersonId { get; set; }

     

        [Association(ThisKey = nameof(PersonId), OtherKey = nameof(PersonTestEntity.Id))]
        public PersonTestEntity Owner { get; set; }
    }

   /// <summary>
   /// Дто в которую будет мэппится
   /// </summary>
    public class BankCardTestDto: EntityDto
    {
        public int Year { get; set; }
        
        public int Month { get; set; }

        public string OwnerFirstname { get; set; }

        public string OwnerSurname { get; set; }

     

    }

    public enum BankCardType
    {
        MasterCard = 1,
        Visa = 2,
        MIR = 3
    }

    // Реадонли сервис для тестрования метода LoadDtoOrNull
    public class BankCardTestEntityService : ReadonlyEntityServiceBase<BankCardTestEntity, BaseListDto, object, BankCardTestDto> { }

    // Сервис для инсерта тестовых данных по человеку.
    public class PersonTestEntityService : LegacyEntityServiceBase<PersonTestEntity> { }

}
