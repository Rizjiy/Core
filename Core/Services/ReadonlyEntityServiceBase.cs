using Core.Domain;
using Core.Dto;
using Core.Internal.Kendo.DynamicLinq;
using LinqToDB;
using System;
using System.Linq;

namespace Core.Services
{
    [Obsolete("Используйте новый тип сервиса только для чтения ReadonlyServiceBase.") ]
    public abstract class ReadonlyEntityServiceBase<TEntity, TListDto, TFilterDto, TEntityDto> : LegacyEntityServiceBase<TEntity>
        where TEntity : EntityBase, new()
        where TListDto : BaseListDto, new()
        where TFilterDto: class, new()
        where TEntityDto: EntityDto, new()
    {

        /// <summary>
        /// Проекция данных в дто при запросе списка.
        /// </summary>
        /// <returns>Возвращает проекцию дто-объекта из списка.</returns>
        protected virtual System.Linq.Expressions.Expression<Func<TEntity, TListDto>> Projection()
        {
            return null;
        }

        /// <summary>
        ///  Позволяет наложить дополнительные фильтры на запрос ДО получения проекции.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filterDto"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> QueryCustomFilters(IQueryable<TEntity> query, TFilterDto filterDto)
        {
            return query;
        }

        /// <summary>
        /// Позволяет наложить дополнительные фильтры на запрос ПОСЛЕ получения проекции.
        /// </summary>
        /// <param name="query">Запрос списка.</param>
        /// <param name="filter">Фильтр.</param>
        /// <returns></returns>
        protected virtual IQueryable<TListDto> QueryCustomFilters(IQueryable<TListDto> query, TFilterDto filter)
        {
            return query;
        }

        /// <summary>
        /// Запрос к списочным данным. Метод может быть переопеределен и в этом случае тогда
        /// не требуется переопределять методы Projection() и QueryCustomFilters().
        /// </summary>
        /// <param name="queryParam">Параметры запроса: фильтр, сортировки, пейджинг</param>
        /// <returns>Страница списка.</returns>
        public virtual DataSourceResult<TListDto> GetQueryResultDto(DataSourceRequestDto<TFilterDto> request)
        {
            var query = GetQuery().AsQueryable();

            // Накладываем кастомные фильтры
            query = QueryCustomFilters(query, request.FilterDto);

            // Получение проекции.
            var projection = Projection();

            var queryList = projection != null
                                ? query.Select(projection)
                                : Mapper.ProjectTo<TListDto>(query);

            // Накладываем кастомные фильтры
            queryList = QueryCustomFilters(queryList, request.FilterDto);

            //Kendo фильтры
            var dsResult = queryList.ToDataSourceResult(request);

            return dsResult;
        }

        /// <summary>
        /// Загружает данные по идентификатору из базы в TEntityDto.
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Заполненое dto или null</returns>
        public virtual TEntityDto LoadDtoOrNull(int id)
        {
            return LoadDtoOrNull<TEntityDto, TEntity>(id);
        }

    }
}
