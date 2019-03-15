using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Core.Caching.Implementation;
using Core.Interfaces;

namespace Core.Web.Cache
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Nick { get; set; }
        public DateTime DateCached { get; set; }
    }

    public class UserCache : CacheBase<string, UserDto>, IDependency
    {
        //переопреляем получение строкового ключа таким образом что в провайдер передается строка в нижнем регистре. 
        protected override string GetTransformedKey(string key)
        {
            return key.ToString().ToLower();
        }

        private IEnumerable<UserDto> GetFakeData()
        {
            return new List<UserDto>
            {
                new UserDto { UserId = 1, Nick = "UserTheFirst", DateCached = DateTime.Now },
                new UserDto { UserId = 2, Nick = "UserTheSecond", DateCached = DateTime.Now },
                new UserDto { UserId = 3, Nick = "UserTheThird", DateCached = DateTime.Now },
            };
        }

        public override IDictionary<string, object> LoadAll()
        {
            Debug.WriteLine($"{nameof(UserCache)}.LoadAll() method is called");

            return GetFakeData().ToDictionary(s => s.Nick.ToLower(), s => (object)s);
            
        }

        protected override UserDto LoadValue(string key)
        {
            Debug.WriteLine($"{nameof(UserCache)}.LoadValue(string key) method is called");

            return GetFakeData().FirstOrDefault(u => u.Nick.ToLower() == key.ToLower());
        }
    }
}