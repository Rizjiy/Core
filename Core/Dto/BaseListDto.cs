using Newtonsoft.Json;

namespace Core.Dto
{
    public class BaseListDto
    {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}
