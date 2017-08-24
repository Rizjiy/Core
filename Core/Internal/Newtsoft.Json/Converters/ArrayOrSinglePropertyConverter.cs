using System;
using Newtonsoft.Json;

namespace Core.Internal.Newtsoft.Json.Converters
{
    public class ArrayOrSinglePropertyConverter<TDto> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize(reader, objectType);
            }

            return new TDto[]
                {
                     serializer.Deserialize<TDto>(reader)
                };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
