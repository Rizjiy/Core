using System.Collections.Generic;
using Core.Caching.Implementation;
using System.Linq;
using Core.Interfaces;

namespace Core.MSTest.Caching
{
    /// <summary>
    /// Тестовая реализация кэша на фейковых данных в источнике (где обычно будет БД). 
    /// </summary>
    public class CacheBaseImpl : CacheBase<int, TestPersonDto>, IDependency
    {
        //todo: уточнить на предмет потокобезопасности
        public List<TestPersonDto> FakeData = new List<TestPersonDto>();

        protected override TestPersonDto LoadValue(int key)
        {
            return FakeData.FirstOrDefault(s => s.Id == key);
        }

        public override IDictionary<string, object> LoadAll()
        {
            return FakeData.ToDictionary(s => s.Id.ToString(), s => (object)s);
        }
    }
}
