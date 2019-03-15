using Core.Domain;
using Core.Dto;

namespace Core.Services
{
    public abstract class AbstractEntityService<TEntity, TListDto, TFilterDto, TEntityDto> : ReadonlyEntityServiceBase<TEntity, TListDto, TFilterDto, TEntityDto>
        where TEntity : EntityBase, new()
        where TListDto: BaseListDto, new()
        where TFilterDto: class, new()
        where TEntityDto: EntityDto, new()
    {

        /// <summary>
        /// Сохраняет сущность полученную из Dto.
        /// </summary>
        /// <param name="dto">Дто-объект</param>
        /// <returns>Сохраненная сущность с применными изменениями.</returns>
        public virtual TEntity SaveDto(TEntityDto dto)
        {
            var entity = LoadEntityOrNull(dto.Id);

            if(entity == null)
            {
                Mapper.Map(dto, entity = CreateEntity());
                entity.LogUser_Id = dto.LogUser_Id;
                Insert(entity);
            }
            else
            {
                Mapper.Map(dto, entity);
                entity.LogUser_Id = dto.LogUser_Id;
                Update(entity);
            }
            
            return entity;
        }
        
    }
}
