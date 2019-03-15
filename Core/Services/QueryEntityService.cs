using Core.Domain;
using Core.Utils.CustomeException;
using System.Linq;

namespace Core.Services
{
    public class QueryEntityService<TEntity> : ServiceBase
        where TEntity: EntityBase, new()
    {
        public virtual IQueryable<TEntity> GetQuery()
        {
            return DataContext.GetTable<TEntity>();
        }

        /// <summary>
        /// Загружает сущность по идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор сущности.</param>
        /// <returns>Сущность.</returns>
        public virtual TEntity LoadEntity(int id)
        {
            var entity = LoadEntityOrNull(id);
            if (entity == null)
                throw new NotFoundException($"Сущность не найдена. Тип сущности: {nameof(TEntity)}. Идентификатор: {id}.");
            
            return entity;
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
    }
}
