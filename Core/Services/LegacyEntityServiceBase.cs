using Core.Domain;
using Core.Dto;
using System.Linq;

namespace Core.Services
{
    /// <summary>
    /// Базовый сервис. Содержит методы загрузки или создания сущностей.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class LegacyEntityServiceBase<TEntity> : LegacyServiceBase
        where TEntity : EntityBase, new()
    {

        /// <summary>
        /// Сохраняет сущность
        /// </summary>
        /// <param name="entity">Сущность для сохранения</param>
        /// <returns>Сохраненная сущность с применными изменениями.</returns>
        public virtual TEntity SaveEntity(TEntity entity)
        {
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
        /// Получение IQUeryable по сущности
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> GetQuery()
        {
            return DataContext.GetTable<TEntity>();
        }


        public virtual bool Exists(int id)
        {
            return Exists<TEntity>(id);
        }

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

        public virtual TDto LoadDtoOrNull<TDto>(int id)
            where TDto: EntityDto
        {
            return LoadDtoOrNull<TDto, TEntity>(id);
        }

    
        /// <summary>
        /// Вставляет новые записи в бд
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(TEntity entity)
        {
            Insert<TEntity>(entity);
        }
        
        /// <summary>
        /// Обновляет записи в бд
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(TEntity entity)
        {
            Update<TEntity>(entity);

          
        }

        /// <summary>
        /// Удаляет строку из таблицы в бд.
        /// </summary>
        /// <param name="entity">Сущность для удаления.</param>
        public virtual void Delete(TEntity entity)
        {
            Delete<TEntity>(entity);
        }
        
    }

}
