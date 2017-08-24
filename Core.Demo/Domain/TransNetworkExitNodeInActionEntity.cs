using Core.Domain;
using LinqToDB.Mapping;

namespace Core.Demo.Domain
{
    [Table(Name = "TransNetworkExitNodeInAction")]
    public class TransNetworkExitNodeInActionEntity : EntityBase
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        [PrimaryKey]
        [Column(CanBeNull = false)]
        public override int Id { get; set; }

        /// <summary>
        /// Наименование интерфейса.
        /// </summary>
        [Column(CanBeNull = true)]
        public string Name { get; set; }

        /// <summary>
        /// Mac-адрес
        /// </summary>
        [Column(CanBeNull = true)]
        public string Mac { get; set; }

        /// <summary>
        /// Ip-адерс
        /// </summary>
        [Column(CanBeNull = true)]
        public string Ip { get; set; }

        /// <summary>
        /// Маска
        /// </summary>
        [Column(CanBeNull = true)]
        public string Mask { get; set; }

        /// <summary>
        ///  Ip-адрес шлюза (для данного интерфейса) 
        /// </summary>
        [Column(CanBeNull = true)]
        // ReSharper disable once InconsistentNaming
        public string IpGW { get; set; }

        [Column(CanBeNull = false)]
        public int TransExitNodeActionId { get; set; }

        /// <summary>
        /// Точка выхода ДОР в сеть.
        /// </summary>
        [Association(OtherKey = nameof(TransExitNodeActionEntity.Id), ThisKey = nameof(TransExitNodeActionId), CanBeNull = false), NotNull]
        public TransExitNodeActionEntity TransExitNodeAction { get; set; }
    }
}
