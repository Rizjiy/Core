using Core.Domain;
using Core.Dto;
using Core.Utils.CustomeException;
using System.Linq;

namespace Core.Services
{
    public class EntityDtoServiceBase<TEntity, TDto> : EntityServiceBase<TEntity>
        where TEntity : EntityBase, new()
        where TDto: EntityDto, new()
    {

     
        /// <summary>
        /// Сохраняет сущность полученную из Dto.
        /// </summary>
        /// <param name="dto">Дто-объект</param>
        /// <returns>Сохраненная сущность с применными изменениями.</returns>
        public virtual TEntity SaveDto(TDto dto)
        {
            var entity = LoadEntityOrNull(dto.Id);

            if (entity == null)
            {
                Mapper.Map(dto, entity = CreateEntity());
                entity.LogUser_Id = dto.LogUser_Id;

                // Валидируем сущность после мэппинга
                Validate(entity);
                Insert(entity);
            }
            else
            {
                Mapper.Map(dto, entity);
                entity.LogUser_Id = dto.LogUser_Id;

                // Валидируем сущность после мэппинга
                Validate(entity);
                Update(entity);
            }

            return entity;
        }

      

        public virtual void DeleteDto(EntityDto  dto)
        {
            var entity = new TEntity()
            {
                Id = dto.Id,
                LogUser_Id = dto.LogUser_Id
            };

            Delete(entity);
        }

        public virtual TDto LoadDtoOrNull(int id)
        {
            var query = Mapper.ProjectTo<TDto>(DataContext.GetTable<TEntity>());

            var resultDto = query.FirstOrDefault(dto => dto.Id == id);

            if (resultDto == null)
                return default(TDto);
            else
                return resultDto;
        }

        public virtual TDto LoadDto(int id)
        {
            var dto = LoadDtoOrNull(id);

            if (dto == null)
                throw new NotFoundException($"Не удалось загрузить dto-объект типа {nameof(TDto)}. Идентификатор: {id}.");
            return dto;
        }
    }
}
