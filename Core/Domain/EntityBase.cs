using LinqToDB.Mapping;

namespace Core.Domain
{
    /// <summary>
    /// Базовый класс для сущностей из бд.
    /// </summary>
    public abstract class EntityBase
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public abstract int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, внесшего изменения.
        /// </summary>
        [NotColumn]
        public int LogUser_Id { get; set; }
    }
}
