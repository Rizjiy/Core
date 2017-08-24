using Newtonsoft.Json.Converters;

namespace Core.Internal.Newtsoft.Json.Converters
{
    public class ddMMyyyy_hhmmss : IsoDateTimeConverter
    {
        public ddMMyyyy_hhmmss()
        {
            DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        }
    }
}
