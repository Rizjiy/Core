using System;
using Newtonsoft.Json;

namespace Core.Demo.Dto
{
    /// <summary>
    /// Фильтр по периоду
    /// </summary>
    public class TransExitNodeActionFilterDto
    {
        /// <summary>
        /// Начало периода.
        /// </summary>
        [JsonProperty(PropertyName = "startDate")]
        public DateTime? StartPeriodDateTime { get; set; }

        /// <summary>
        /// Конец периода.
        /// </summary>
        [JsonProperty(PropertyName = "endDate")]
        public DateTime? EndPeriodDateTime { get; set; }
        
    }
}
