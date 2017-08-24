using Newtonsoft.Json;

namespace Core.Dto
{
    public class EntityDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}
