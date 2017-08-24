using System;

namespace Core.Domain
{
    /// <summary>
    /// Сущеность для генерации идентификаторов таблиц
    /// </summary>
    public class CustomSequenceEntity : EntityBase
    {
        /// <summary>
        /// Идентификатор(не использовать)
        /// </summary>
        public override int Id
        {
            get
            {
                throw new NotImplementedException("CustomSequenceEntity.GetId()");
            }

            set
            {
                throw new NotImplementedException("CustomSequenceEntity.SetId()");
            }
        }

        /// <summary>
        /// Имя таблицы
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Значение идентификатора
        /// </summary>
        public int IdValue { get; set; }
    }
}
