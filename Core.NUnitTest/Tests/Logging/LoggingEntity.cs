using Core.Domain;
using Core.Domain.Attributes;
using LinqToDB.Mapping;
using System;

namespace Core.NUnitTest.Tests.Logging
{
    /// <summary>
    /// Сущность для тестирования логирования
    /// </summary>
    [Table(Name = "Logging", Schema = "dbo", Database = "CoreTest")]
    [LogTable]
    public class LoggingEntity : EntityBase
    {
        [PrimaryKey]
        [Column]
        public override int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public int Year { get; set; }

        [Column(Name = "some_amount")]
        public decimal Amount { get; set; }

        [Column]
        public DateTime BirthDate { get; set; }

        [Column]
        public string NullableName { get; set; }
    }

    /// <summary>
    /// Сущность таблицы Лога
    /// </summary>
    /// 
    [Table(Name = "Logging_log", Schema = "dbo", Database = "CoreTest")]
    public class LoggingLogEntity
    {

        /// <summary>
        /// Свойства по умолчанию для таблицы логирования
        /// </summary>
        #region Свойства по умолчанию для таблицы логирования

        /// <summary>
        /// Идентификатор лога
        /// </summary>
        [PrimaryKey]
        [Column]
        [Identity]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор логируемой записи
        /// </summary>
        [Column]
        public int IdLog { get; set; }

        /// <summary>
        /// Идентификатор пользователя, внесшего изменения
        /// </summary>
        [Column]
        public int? UserIdLog { get; set; }

        /// <summary>
        /// Дат логирования со временем
        /// </summary>
        [Column]
        public DateTime DateLog { get; set; }

        /// <summary>
        /// Действие - Обновление, Инсерт или удаление
        /// </summary>
        [Column]
        public string ActionLog { get; set; }

        #endregion


        /// <summary>
        /// Свойства для логирования
        /// </summary>
        #region Свойства для логирования

        [Column]
        public string Name { get; set; }

        [Column]
        public int? Year { get; set; }

        [Column(Name = "some_amount")]
        public decimal? Amount { get; set; }

        [Column]
        public DateTime? BirthDate { get; set; }

        [Column]
        public string NullableName { get; set; }

        #endregion

    }
}
