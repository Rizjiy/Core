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
using System.Reflection;

namespace Core.Services
{
    public class EntityServiceBase<TEntity> : ServiceBase
        where TEntity : EntityBase, new()
    {
        
        /// <summary>
        /// Создает новую сущность
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <returns>Созданная сущность</returns>
        public virtual TEntity CreateEntity()
        {

            var type = typeof(TEntity);
            // Получаем наименование таблицы.
            var tableName = type.GetTableName();

            var idProperty = type.GetProperty("Id");

            var isEuidAttribute = idProperty.IsDefined(typeof(EuidAttribute));

            var isIdentityAttribute = idProperty.IsDefined(typeof(IdentityAttribute));

            if (isEuidAttribute && isIdentityAttribute)
                throw new CustomAttributeFormatException($"В классе {typeof(TEntity).Name} cвойство {nameof(EntityBase.Id)} помечено сразу двумя взаимоисключающимися атрибутами: {nameof(EuidAttribute)} и {nameof(IdentityAttribute)}.");

            if (isIdentityAttribute)
                return new TEntity { Id = 0 };

            int id = 0;

            if (!isEuidAttribute)
                // Очередной идентификатор для сущности
                id = GetSequenceNextValue(tableName);
            else // --------------- Свойство Id помечено атрибутом Euid
            {
                // Свойство Id помеченое атрибутом EuidAttribute должно быть также помечено атрибутом PrimaryKeyAttribute.
                if (!idProperty.IsDefined(typeof(PrimaryKeyAttribute)))
                {
                    throw new CustomAttributeFormatException($"В классе {typeof(TEntity).Name} cвойство {nameof(EntityBase.Id)} помеченое атрибутом {nameof(EuidAttribute)} должно быть также помечено атрибутом {nameof(PrimaryKeyAttribute)}.");
                }

                id = GetEuidNextValue(tableName);
            }

            // Новая сущность
            return new TEntity { Id = id };
        }

        public virtual TEntity SaveEntity(TEntity entity)
        {
            // Сначала валидируем сущность
            Validate(entity);

            var exists = Exists(entity.Id);

            if (exists)
            {
                Update(entity);
            }
            else
            {
                Insert(entity);
            }

            return entity;
        }

        /// <summary>
        /// Показывает существует ли сущность с указанным идентификатором
        /// </summary>
        /// <typeparam name="T">Тип сущности</typeparam>
        /// <param name="id">Идентификатор сущности</param>
        /// <returns>True - сущность с таким идентификатором существует; False - сущность с таким идентификатором не существует</returns>
        public bool Exists(int id)
        {
            bool result = DataContext.GetTable<TEntity>().Any(entity => entity.Id == id);

            return result;
        }


        /// <summary>
        /// Получение следующего порядкового номера
        /// </summary>
        /// <param name="sequenceName"></param>
        /// <returns>Сгенерированный уникальный в рамках таблицы идентификатор</returns>
        private int GetSequenceNextValue(string sequenceName)
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
        private int GetEuidNextValue(string tableName)
        {
            var query = $"INSERT INTO EntityUNID(TableID) OUTPUT INSERTED.UNID AS Id SELECT TableID FROM [Table] WHERE TableName = '{tableName}'";

            EntityDto result;


            using (var context = new DataConnection(GetConnectionName()))
            {
                result = context.Query<EntityDto>(query).First();
            }

            return result.Id;

        }


        protected internal void Update(TEntity entity)
        {
            DataContext.Update(entity);

            if (entity.GetType().IsDefined(typeof(LogTableAttribute)))
                LogInsert(entity, ActionLog.Update);
        }


        /// <summary>
        /// Метода валидация сущности перед сохранением изменений
        /// </summary>
        /// <param name="entity">Сущность</param>
        protected virtual void Validate(TEntity entity)
        {
            return;
        }


        public TEntity LoadEntityOrNull(int id)
        {
            return DataContext.GetTable<TEntity>().FirstOrDefault(entity => entity.Id == id);
        }

        public TEntity LoadEntity(int id)
        {
            return DataContext.GetTable<TEntity>().First(entity => entity.Id == id);
        }

        protected internal void Insert(TEntity entity)
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

            if (typeEntity.IsDefined(typeof(LogTableAttribute)))
                LogInsert(entity, ActionLog.Insert);
        }


        /// <summary>
        /// Удаляет строку из таблицы в бд.
        /// </summary>
        /// <typeparam name="T">Тип удаляемой сущности.</typeparam>
        /// <param name="entity">Сущность для удаления.</param>
        protected internal void Delete(TEntity entity)
        {
            bool isLog = entity.GetType().IsDefined(typeof(LogTableAttribute));
            if (isLog)
            {
                int logUserId = entity.LogUser_Id;
                entity = DataContext.GetTable<TEntity>().FirstOrDefault(item => item.Id == entity.Id);
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
        private void LogInsert(TEntity entity, ActionLog action)
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
