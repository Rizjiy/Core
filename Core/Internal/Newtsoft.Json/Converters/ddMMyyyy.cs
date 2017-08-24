using Newtonsoft.Json.Converters;

namespace Core.Internal.Newtsoft.Json.Converters
{
    public class ddMMyyyy : IsoDateTimeConverter
    {
        public ddMMyyyy()
        {
            DateTimeFormat = "dd.MM.yyyy";
        }
    }
}
