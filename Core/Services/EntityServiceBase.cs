using Core.Domain;
using Core.Dto;
using System;
using System.Linq;

namespace Core.Services
{
    /// <summary>
    /// Базовый сервис. Содержит методы загрузки или создания сущностей.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityServiceBase<TEntity> : ServiceBase
        where TEntity : EntityBase, new()
    {
        /// <summary>
        /// Загружает сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности.</param>
        /// <returns>Сущность.</returns>
        public virtual TEntity LoadEntity(int id)
        {
            return DataContext.GetTable<TEntity>().First(item => item.Id == id);
        }



        /// <summary>
        /// Пытается загрузить сущность по идетификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности.</param>
        /// <returns>Сущность или null, если сущность не найдена.</returns>
        public virtual TEntity LoadEntityOrNull(int id)
        {
            return DataContext.GetTable<TEntity>().FirstOrDefault(item => item.Id == id);
        }

        /// <summary>
        /// Загружает сущность по экземпляру dto.
        /// </summary>
        /// <typeparam name="TEntity">Тип загружаемой сущности.</typeparam>
        /// <param name="dto">dto-объект.</param>
        /// <returns>Сущность.</returns>
        public virtual TEntity LoadEntityOrNull(EntityDto dto)
        {
            return LoadEntityOrNull<TEntity>(dto);
        }

      

        /// <summary>
        /// Загружает заполненую сущность или создает новую со сгенерированным идентификатором.
        /// </summary>
        /// <param name="dto">Дто-объект для которого загружается сущность.</param>
        /// <returns>Загруженная или созданная сущность.</returns>
        public virtual TEntity LoadEntityOrCreate(EntityDto dto)
        {
            return LoadEntityOrCreate<TEntity>(dto);
        }


        /// <summary>
        /// Создает новую сущность
        /// </summary>
        /// <returns>Созданная сущность</returns>
        public virtual TEntity CreateEntity()
        {
            return CreateEntity<TEntity>();
        }



    


        /// <summary>
        /// Получение IQUeryable по сущности
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetQuery()
        {
            return DataContext.GetTable<TEntity>();
        }


        /// <summary>
        /// Инсертит в база новую строку и возвращает сущность с заполненым идентити идентификатором.
        /// </summary>
        /// <typeparam name="T">Тип инсерт-сущности.</typeparam>
        /// <param name="entity">Инсерт-сущность.</param>
        /// <returns>Сущность с идентификатором.</returns>
        public T InsertWithIdentity<T>(T entity)
            where T : EntityBase
        {
            var identify = DataContext.InsertWithIdentity(entity);

            entity.Id = Convert.ToInt32(identify);

            return entity;
        }

        /// <summary>
        /// Инсертит или обновляет новую сущность в бд.
        /// </summary>
        /// <typeparam name="T">Тип сущности.</typeparam>
        /// <param name="entity">Сущность для инсерта или обновления</param>
        public void InsertOrUpdate<T>(T entity)
        {
            //DataContext.Insert(entity);
            DataContext.InsertOrReplace(entity);
        }

        /// <summary>
        /// Инсертит или обновляет сущность в бд.
        /// </summary>
        /// <param name="entity">Сущность для инсерта или обновления</param>
        public void InsertOrUpdate(TEntity entity)
        {
            InsertOrUpdate<TEntity>(entity);
        }



        public void Update<T>(T entity)
        {
            DataContext.Update(entity);
        }
        
        public void Update(TEntity entity)
        {
            Update<TEntity>(entity);
        }

        /// <summary>
        /// Удаляет строку из таблицы в бд.
        /// </summary>
        /// <typeparam name="T">Тип удаляемой сущности.</typeparam>
        /// <param name="entity">Сущность для удаления.</param>
        public void Delete<T>(T entity)
        {
            DataContext.Delete(entity);
        }

        /// <summary>
        /// Удаляет строку из таблицы в бд.
        /// </summary>
        /// <param name="entity">Сущность для удаления.</param>
        public void Delete(TEntity entity)
        {
            Delete<TEntity>(entity);
        }

        /// <summary>
        /// Удаляет строку из таблицы в бд.
        /// </summary>
        /// <param name="id">id для удаления.</param>
        public void Delete(int id)
        {
            var entity = new TEntity
            { Id = id };

            DataContext.Delete(entity);
        }

    }

}
