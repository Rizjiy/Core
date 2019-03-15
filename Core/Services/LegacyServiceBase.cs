using Core.Domain;
using Core.Domain.Attributes;
using Core.Dto;
using Core.Internal.LinqToDB;
using Core.Services.Constants;
using LinqToDB.Data;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace Core.Services
{
    public abstract class LegacyServiceBase : ServiceBase
	{
       
		protected LegacyServiceBase()
		{
            
        }

       
        /// <summary>
        /// Показывает существует ли сущность с указанным идентификатором
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>True - сущность с таким идентификатором существует; False - сущность с таким идентификатором не существует</returns>
        public virtual bool Exists<T>(int id)
            where T : EntityBase
        {
            bool result = DataContext.GetTable<T>().Any(entity => entity.Id == id);

            return result;
        }


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
        /// Загрузка сущности по идентфикатору
        /// </summary>
        /// <typeparam name="T">Тип загружаемой сущности</typeparam>
        /// <param name="id">Идентификатор</param>
        /// <returns>Экземпляр сущности или null</returns>
        public virtual T LoadEntityOrNull<T>(int id)
            where T : EntityBase
        {
            return DataContext.GetTable<T>().FirstOrDefault(entity => entity.Id == id);
        }

        public virtual TDto LoadDtoOrNull<TDto, TEntity>(int id)
            where TDto : EntityDto
            where TEntity: EntityBase
        {

            var query = Mapper.ProjectTo<TDto>(DataContext.GetTable<TEntity>());
            
            var resultDto = query.FirstOrDefault(dto=>dto.Id == id);
            if (resultDto == null)
                return default(TDto);
            else
                return resultDto;
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

            var type = typeof(T);
            // Получаем наименование таблицы.
            var tableName = type.GetTableName();

            var idProperty = type.GetProperty("Id");

            var isEuidAttribute = idProperty.IsDefined(typeof(EuidAttribute));

            var isIdentityAttribute = idProperty.IsDefined(typeof(IdentityAttribute));
          
            if(isEuidAttribute && isIdentityAttribute)
                throw new CustomAttributeFormatException($"В классе {typeof(T).Name} cвойство {nameof(EntityBase.Id)} помечено сразу двумя взаимоисключающимися атрибутами: {nameof(EuidAttribute)} и {nameof(IdentityAttribute)}.");

            if (isIdentityAttribute)
                return new T { Id = 0};

            int id = 0;
            
            if (!isEuidAttribute)
                // Очередной идентификатор для сущности
                id = GetSequenceNextValue(tableName);
            else // --------------- Свойство Id помечено атрибутом Euid
            {
                // Свойство Id помеченое атрибутом EuidAttribute должно быть также помечено атрибутом PrimaryKeyAttribute.
                if (!idProperty.IsDefined(typeof(PrimaryKeyAttribute)))
                {
                    throw new CustomAttributeFormatException($"В классе {typeof(T).Name} cвойство {nameof(EntityBase.Id)} помеченое атрибутом {nameof(EuidAttribute)} должно быть также помечено атрибутом {nameof(PrimaryKeyAttribute)}.");
                }

                id = GetEuidNextValue(tableName);
            }

            // Новая сущность
            return new T { Id = id };
        }

        /// <summary>
        /// Получение следующего порядкового номера
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <returns>Сгенерированный уникальный в рамках таблицы идентификатор</returns>
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

        /// <summary>
        /// Получение уникального межтабличного идентификатора. Используется для D3_BP
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Сгенерированый уникальный межтабличный идентификатор</returns>
        public int GetEuidNextValue(string tableName)
        {
            var query = $"INSERT INTO EntityUNID(TableID) OUTPUT INSERTED.UNID AS Id SELECT TableID FROM [Table] WHERE TableName = '{tableName}'";

            EntityDto result;


            using (var context = new DataConnection(GetConnectionName()))
            {
                result = context.Query<EntityDto>(query).First();
            }

            return result.Id;

        }

        internal void Insert<T>(T entity) 
            where T : EntityBase, new()
        {
            Type typeEntity = entity.GetType();
            bool isIdentity = typeEntity.GetProperty("Id").IsDefined(typeof(IdentityAttribute));

            if (isIdentity)
            {
                entity.Id = Convert.ToInt32(DataContext.InsertWithIdentity(entity));
            }
            else
            {
                DataContext.Insert(entity);
            }

            if(typeEntity.IsDefined(typeof(LogTableAttribute)))
                LogInsert(entity, ActionLog.Insert);
        }

        internal void Update<T>(T entity)
            where T : EntityBase, new()
        {
            DataContext.Update(entity);

            if (entity.GetType().IsDefined(typeof(LogTableAttribute)))
                LogInsert(entity, ActionLog.Update);
        }

        /// <summary>
        /// Удаляет строку из таблицы в бд.
        /// </summary>
        /// <typeparam name="T">Тип удаляемой сущности.</typeparam>
        /// <param name="entity">Сущность для удаления.</param>
        internal void Delete<T>(T entity)
            where T : EntityBase, new()
        {
            bool isLog = entity.GetType().IsDefined(typeof(LogTableAttribute));
            if (isLog)
            {
                int logUserId = entity.LogUser_Id;
                entity = DataContext.GetTable<T>().FirstOrDefault(item => item.Id == entity.Id);
                if (entity == null)
                {
                    return;
                }

                entity.LogUser_Id = logUserId;
            }


            DataContext.Delete(entity);
            if (isLog)
                LogInsert(entity, ActionLog.Delete);
        }




        /// <summary>
        /// Инсерт лога
        /// </summary>
        /// <param name="entity">Сущность</param>
        /// <param name="action">Действие</param>
        private void LogInsert<TEntity>(TEntity entity, ActionLog action) where TEntity : EntityBase
        {
            Type entityType = entity.GetType();

            string logTableName = entityType.GetCustomAttribute<LogTableAttribute>().LogTableName;

            // Получение имени таблицы для Лога
            if (string.IsNullOrEmpty(logTableName))
                logTableName = $"{entityType.GetCustomAttribute<TableAttribute>().Name}_log";


            // Словарь содержащий название колонок и их новых значений
            Dictionary<string, object> columnNameValueDictionary = new Dictionary<string, object>();

            // Обязательный колонки для таблицы с Логом
            columnNameValueDictionary.Add("UserIdLog", entity.LogUser_Id); // Идентификатор пользователя вносящего изменения
            columnNameValueDictionary.Add("DateLog", DateTime.Now); // Время изменения
            columnNameValueDictionary.Add("ActionLog", $"{Enum.GetName(typeof(ActionLog), action)}"); // Действие
            columnNameValueDictionary.Add("IdLog", entity.Id); // Идентификатор логируемой сущности



            // Получаем Все свойства сущности
            var properties = entityType.GetProperties();


            // Добавляем в лог значения в свойствах если это не удаление
            foreach (var prop in properties.Where(prop => prop.Name != "Id" // Идентификатор логируется в колонку IdLog. Поэтому убираем его из выборки колонок для логирования;
                        && prop.IsDefined(typeof(ColumnAttribute))          // Брать свойства помеченные атрибутом ColumnAttribute;
                        && !prop.IsDefined(typeof(NotColumnAttribute)))     // Не брать свойства помеченные атрибутом NotColumnAttribute.
                        )
            {
                var columnAttribute = prop.GetCustomAttribute<ColumnAttribute>();

                if (columnAttribute != null)
                {
                    string value = prop.PropertyType == typeof(string) || prop.PropertyType == typeof(DateTime)
                                                ? $"'{prop.GetValue(entity)?.ToString()}'"
                                                : prop.GetValue(entity)?.ToString();

                    columnNameValueDictionary.Add(columnAttribute.Name ?? prop.Name, prop.GetValue(entity));
                }
            }
            // Название колонок в таблице логирования
            string columnNames = string.Join("\r\n, ", columnNameValueDictionary.Keys);

            // Значения в этих колонках
            string columnParameters = "@" + string.Join("\r\n, @", columnNameValueDictionary.Keys);


            string insert = $"INSERT INTO {logTableName}({columnNames})\r\nVALUES\r\n({columnParameters});";

            var paramList = new List<DataParameter>(columnNameValueDictionary.Count);

            foreach (var colNameValue in columnNameValueDictionary)
            {
                paramList.Add(new DataParameter("@" + colNameValue.Key, colNameValue.Value));
            }


            DataContext.Query<EntityDto>(insert, paramList.ToArray()).ToArray();

            // Если идентификатор пользователя меньше нуля, то логируем это как ошибку.
            if (entity.LogUser_Id < 1)
            {
                string message = $"При логирование указан некорректный идентификатор пользователя(UserIdLog) = {entity.LogUser_Id}. Таблица логирования - {logTableName}. Идентификатор сущности(IdLog) = {entity.Id}";
                Logger.Error(message);
            }
        }

        
    }
}
