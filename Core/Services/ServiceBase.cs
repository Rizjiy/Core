using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Core.Internal.Dependency;
using Core.Internal.LinqToDB;
using LinqToDB.Data;
using Core.Domain;
using System.Linq.Expressions;
using Core.Dto;
using System.Linq.Dynamic;
using Core.LinqToDB.Interfaces;

namespace Core.Services
{
    public abstract class ServiceBase : IDisposable, IDependency
	{
        /// <summary>
        /// Режим для авто-тестов
        /// </summary>
        public static volatile bool TestMode = false;

        /// <summary>
        /// Фабрика соединенй БД
        /// </summary>
        public IDataConnectionFactory ConnectionFactory { get; set; }

        private IDataConnection _dataContext;
        /// <summary>
        /// Соединение БД
        /// </summary>
        public IDataConnection DataContext
        {
            get
            {
                if(_dataContext == null)
                {
                    _dataContext = ConnectionFactory.GetDataConnection(GetConnectionName());
                }
                return _dataContext;
            }
            set
            {
                _dataContext = value;
            }
        }




        /// <summary>
        /// Получаю название коннекшена по словарю соответствия неймспейсов и типу текущего экземпляра сервиса
        /// При множествоенном попадании отбирается наиболее полное соответствие
        /// </summary>
        public string GetConnectionName()
		{
			var ns = GetType().Namespace + ".";
			var key = NamespaceConnectionDict
				.Keys
				.OrderByDescending(k => k.Length)
				.FirstOrDefault(k => ns.StartsWith(k + "."));
			var value = string.IsNullOrWhiteSpace(key) ? "default" : NamespaceConnectionDict[key];
			return value;
		}

		protected ServiceBase()
		{
            
        }

        public virtual void Dispose()
        {

        }

        /// <summary>
        /// Словарь соответствия неймспейсов и названий коннекшенов
        /// </summary>
        internal static readonly IDictionary<string, string> NamespaceConnectionDict = new ConcurrentDictionary<string, string>();


        /// <summary>
        /// Загружает сущность по экземпляру dto.
        /// </summary>
        /// <typeparam name="T">Тип загружаемой сущности</typeparam>
        /// <param name="dto">dto-объект</param>
        /// <returns>Сущность. Если dto - null или нет записей с dto.Id, то возвращается null</returns>
        public virtual T LoadEntityOrNull<T>(EntityDto dto)
            where T : EntityBase
        {
            if (dto == null)
                return null;

            return DataContext.GetTable<T>().FirstOrDefault(item => item.Id == dto.Id);
        }

        /// <summary>
        /// Загружает сущность по частному условию
        /// </summary>
        /// <typeparam name="T">Тип загружаемой сущности</typeparam>
        /// <param name="dto">dto-объект</param>
        /// <param name="predicate">Частное условие</param>
        /// <returns></returns>
        public virtual T LoadEntityOrNull<T>(EntityDto dto, Func<IQueryable<T>, EntityDto, IQueryable<T>> predicate) where T : EntityBase
        {
            if (dto == null)
                return null;

            if (predicate == null)
                return DataContext.GetTable<T>().FirstOrDefault(item => item.Id == dto.Id);

            IQueryable<T> query = DataContext.GetTable<T>();
            query = predicate(query, dto);
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Загружает сущность по экземпляру dto.
        /// </summary>
        /// <typeparam name="T">Тип загружаемой сущности</typeparam>
        /// <param name="dto">dto-объект</param>
        /// <param name="expressionEntityKey">Выражение предоставляющие альтернативный ключ для загрузки сущности, который будет сравнивать с dto.Id</param>
        /// <returns></returns>
        public virtual T LoadEntityOrNull<T>(EntityDto dto,
            Expression<Func<T, int>> expressionEntityKey)
            where T : EntityBase
        {
            if (dto == null)
                return null;

            if (expressionEntityKey == null)
            {
                return DataContext.GetTable<T>().FirstOrDefault(item => item.Id == dto.Id);
            }
            else
            {
                MemberExpression body = expressionEntityKey.Body as MemberExpression;

                if (body == null)
                {
                    UnaryExpression ubody = (UnaryExpression)expressionEntityKey.Body;
                    body = ubody.Operand as MemberExpression;
                }

                string paramName = body.Member.Name;
                string predicate = string.Concat(paramName, "=", dto.Id);
                IQueryable<T> query = DataContext.GetTable<T>().Where(predicate);

                return query.FirstOrDefault();
            }
        }



        /// <summary>
        /// Загружает заполненую сущность или создает новую с сгенерированным идентификатором.
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <param name="dto">Дто-объект для которого загружается сущность.</param>
        /// <returns>Загруженная или созданная сущность.</returns>
        public virtual T LoadEntityOrCreate<T>(EntityDto dto)
            where T : EntityBase, new()
        {

            // Загружаем сущность из бд.
            var entity = LoadEntityOrNull<T>(dto);

            if (entity != null)
                return entity;


            // Сущность в бд не нашлась, создаю новую
            return CreateEntity<T>();
        }


        /// <summary>
        /// Создает новую сущность
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <returns>Созданная сущность</returns>
        public virtual T CreateEntity<T>() where T : EntityBase, new()
        {
            // Получаем наименование таблицы.
            var tableName = typeof(T).GetTableName();

            // Очередной идентификатор для сущности
            var id = GetSequenceNextValue(tableName);

            // Новая сущность
            return new T { Id = id };
        }

        /// <summary>
        /// Получение следующего порядкового номера
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <returns></returns>
        public int GetSequenceNextValue(string sequenceName)
        {
            // Запрос на генерацию и получения нового идентификатора из бд.
            var query = $"UPDATE CustomSequence SET idvalue += 1 OUTPUT inserted.idvalue AS Id WHERE tablename = '{sequenceName}'";

            EntityDto result;

            if (TestMode)
            {
                result = DataContext.Query<EntityDto>(query).First();
            }
            else
            {
                using (var context = new DataConnection(GetConnectionName()))
                {
                    result = context.Query<EntityDto>(query).First();
                }
            }

            return result.Id;
        }


    }
}
