using Core.Services;

namespace Core.NUnitTest.Tests.Sequences
{
    /// <summary>
    /// Сервис с сущностью в которой свойство Id помечено валидными атрибутом
    /// </summary>
    public class PersonEntityService : EntityServiceBase<PersonEntity>
    {
    }

    /// <summary>
    /// Сервис с сущностью в которой свойстов Id помечено невалидными атрибутом: сочетание двух несовместимых атрибутов Euid и Identity
    /// </summary>
    public class PersonEuidIdentityEntityService : EntityServiceBase<PersonEuidIdentityEntity>
    {

    }

    /// <summary>
    /// Сервис с сущностью в которой свойстов Id помечено невалидными атрибутом:
    /// атрибут EuidAttribute должен соседствовать с атрибутом PrimaryKeyAttribute
    /// </summary>
    public class PersonOnlyEuidEntityService : EntityServiceBase<PersonOnlyEuidEntity>
    {
    }

    /// <summary>
    /// Сервис с сущностью в которой свойстов Id помечено валидным атрибутом:
    /// атрибут Identity 
    /// </summary>
    public class PersonOnlyIdentityEntityService : EntityServiceBase<PersonOnlyIdentityEntity>
    {
    }
}
