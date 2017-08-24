using System;
using Core.Domain;
using Core.Dto;
using LinqToDB;

namespace Core.Services
{
    public abstract class AbstractEntityService<TEntity, TListDto, TFilterDto, TEntityDto> : ReadonlyEntityServiceBase<TEntity, TListDto, TFilterDto, TEntityDto>
        where TEntity : EntityBase, new()
        where TListDto: EntityDto, new()
        where TFilterDto: class, new()
        where TEntityDto: EntityDto, new()
    {
        
        /// <summary>
        /// Получает сущность из Dto.
        /// </summary>
        /// <param name="dto">Дто-объект</param>
        /// <returns>Сущность с примененными изменениями.</returns>
        public abstract TEntity SaveDto(TEntityDto dto);
    
    }
}
