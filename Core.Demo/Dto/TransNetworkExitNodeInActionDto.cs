using Core.Dto;
using Newtonsoft.Json;

namespace Core.Demo.Dto
{
    /// <summary>
    /// Dto-класс: События сетевых выходов.
    /// </summary>
    public class TransNetworkExitNodeInActionDto : EntityDto
    {
        /// <summary>
        /// Наименование интерфейса.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Mac-адрес
        /// </summary>
        [JsonProperty(PropertyName = "mac")]
        public string Mac { get; set; }

        /// <summary>
        /// Ip-адерс
        /// </summary>
        [JsonProperty(PropertyName = "ip")]
        public string IP { get; set; }

        /// <summary>
        /// Маска
        /// </summary>
        [JsonProperty(PropertyName = "mask")]
        public string Mask { get; set; }

        /// <summary>
        ///  Ip-адрес шлюза (для данного интерфейса) 
        /// </summary>
        [JsonProperty(PropertyName = "ipGW")]
        public string IpGW { get; set; }
    }
}
