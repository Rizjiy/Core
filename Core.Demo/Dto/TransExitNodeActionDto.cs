using System;
using Core.Dto;
using Core.Internal.Newtsoft.Json.Converters;
using Newtonsoft.Json;

namespace Core.Demo.Dto
{
    public class TransExitNodeActionDto : EntityDto
    {
        /// <summary>
        /// Даты и время выхода(Время начала подключения)
        /// </summary>
        [JsonProperty(PropertyName = "date")]
        [JsonConverter(typeof(ddMMyyyy_hhmmss))]
        public DateTime Date { get; set; }

        /// <summary>
        /// Имя хоста.
        /// </summary>
        [JsonProperty(PropertyName = "hostName")]
        public string HostName { get; set; }

        /// <summary>
        /// Идентификатор сессии - guid.
        /// </summary>
        [JsonProperty(PropertyName = "sessionId")]
        public string SessionId { get; set; }

        /// <summary>
        /// Событие (init,connected, error, disconnected) 
        /// </summary>
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }

        /// <summary>
        ///Тип CMAK профиля vpn подключения (шаблон использованный для генерации профиля) 
        /// </summary>
        [JsonProperty(PropertyName = "profileName")]
        public string ProfileName { get; set; }

        /// <summary>
        /// Наименование vpn-подключения.
        /// </summary>
        [JsonProperty(PropertyName = "vpnName")]
        public string VpnName { get; set; }

        /// <summary>
        /// Ip-адрес шлюза по умолчанию/mac адрес шлюза по умолчанию
        /// </summary>
        [JsonProperty(PropertyName = "defaultGW")]
        public string DefaultGW { get; set; }

        /// <summary>
        /// Ip - адрес точки выхода.
        /// </summary>
        [JsonProperty(PropertyName = "externalIP")]
        public string ExternalIP { get; set; }

        /// <summary>
        /// События сетевых выходов.
        /// </summary>
        [JsonConverter(typeof(ArrayOrSinglePropertyConverter<TransNetworkExitNodeInActionDto>))]
        [JsonProperty(PropertyName = "network")]
        public TransNetworkExitNodeInActionDto[] Network { get; set; }
    }
}
