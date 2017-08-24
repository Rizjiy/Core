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
    }
}
