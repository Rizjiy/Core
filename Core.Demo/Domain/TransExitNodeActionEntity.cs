using System;
using System.Collections.Generic;
using Core.Domain;
using LinqToDB.Mapping;

namespace Core.Demo.Domain
{
    /// <summary>
    /// Entity-класс: Точка выхода ДОР в сеть.
    /// </summary>
    [Table(Name = "TransExitNodeAction")]
    public class TransExitNodeActionEntity : EntityBase
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        [PrimaryKey]
        [Column(CanBeNull = false)]
        public override int Id { get; set; }

        /// <summary>
        /// Даты и время выхода.
        /// </summary>
        [Column(CanBeNull = false)]
        public DateTime Date { get; set; }

        /// <summary>
        /// Имя хоста.
        /// </summary>
        [Column(CanBeNull = false)]
        public string HostName { get; set; }

        /// <summary>
        /// Идентификатор сессии - guid.
        /// </summary>
        [Column(CanBeNull = false)]
        public string SessionId { get; set; }

        /// <summary>
        /// Событие (init,connected, error, disconnected) 
        /// </summary>
        [Column(CanBeNull = false)]
        public string Action { get; set; }

        /// <summary>
        ///Тип CMAK профиля vpn подключения (шаблон использованный для генерации профиля) 
        /// </summary>
        [Column(CanBeNull = false)]
        public string ProfileName { get; set; }

        /// <summary>
        /// Наименование vpn-подключения.
        /// </summary>
        [Column(CanBeNull = false)]
        public string VpnName { get; set; }

        /// <summary>
        /// Ip-адрес шлюза по умолчанию/mac адрес шлюза по умолчанию
        /// </summary>
        [Column(CanBeNull = true)]
        public string DefaultGW { get; set; }

        /// <summary>
        /// Ip - адрес точки выхода.
        /// </summary>
        [Column(CanBeNull = true)]
        public string ExternalIP { get; set; }

        [Association(OtherKey = nameof(TransNetworkExitNodeInActionEntity.TransExitNodeActionId), ThisKey = nameof(Id))]
        public ICollection<TransNetworkExitNodeInActionEntity> Networks { get; set; }
    }
}
