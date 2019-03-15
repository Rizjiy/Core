using Newtonsoft.Json;

namespace Core.Dto
{
    public class EntityDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [AutoMapper.IgnoreMap]
        public int LogUser_Id { get; set; }
    }
}
