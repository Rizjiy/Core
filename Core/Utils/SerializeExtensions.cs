using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils
{
    public static class SerializeExtensions
    {
        /// <summary>
        /// Сериализация любого объекта в JSON
        /// </summary>
        /// <param name="obj"> Объект </param>
        /// <returns> JSON </returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// Сериализация любого объекта в JSON
        /// </summary>
        /// <param name="obj"> Объект </param>
        /// <returns> JSON </returns>
        public static string SerializeToJson(this object obj)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }

        /// <summary>
        /// Десериализация JSON в объект указанного типа
        /// </summary>
        /// <typeparam name="T"> Тип объекта</typeparam>
        /// <param name="json"> JSON </param>
        /// <returns> Объект </returns>
        public static T DeserializeJson<T>(this string json)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }




    }
}
