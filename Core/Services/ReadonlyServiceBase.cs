using Core.Domain;
using Core.Dto;
using Core.Internal.Kendo.DynamicLinq;
using System.Linq;

namespace Core.Services
{
    public class ReadonlyServiceBase<TEntity, TListDto, TFilterDto> : ServiceBase
        where TEntity : EntityBase, new()
        where TListDto : BaseListDto, new()
        where TFilterDto: class, new()
    {

        protected virtual IQueryable<TEntity> GetQuery()
        {
            return DataContext.GetTable<TEntity>(); 
        }

        /// <summary>
        /// Проекция данных в списочное дто при запросе списка
        /// </summary>
        /// <param name="entityQuery">Базовый запрос к таблице-сущности</param>
        /// <returns>Возвращает проекцию списочных дто из списка</returns>
        protected virtual IQueryable<TListDto> Projection(IQueryable<TEntity> entityQuery, TFilterDto filterDto)
        {
            return Mapper.ProjectTo<TListDto>(entityQuery);
        }

        /// <summary>
        /// Запрос к списочным данным. Метод может быть переопеределен и в этом случае
        /// не требуется переопределять методы Projection(), EntityQueryCustomFilters() и ListDtoQueryCustomFilters.
        /// </summary>
        /// <param name="request">Параметры запроса: фильтр, сортировки, пейджинг</param>
        /// <returns>Страница списка.</returns>
        public virtual DataSourceResult<TListDto> GetQueryResultDto(DataSourceRequestDto<TFilterDto> request)
        {
            var query = GetQuery(); 

            // Накладываем кастомные фильтры
            query = EntityQueryCustomFilters(query, request.FilterDto);

            // Получение проекции
            var queryListDto = Projection(query, request.FilterDto);

            queryListDto = ListDtoQueryCustomFilters(queryListDto, request.FilterDto);

            // Накладываем кастомные фильтры
            var dsResult = queryListDto.ToDataSourceResult(request);

            return dsResult;
        }
        /// <summary>
        ///  Позволяет наложить дополнительные фильтры на запрос ДО получения проекции.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filterDto"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> EntityQueryCustomFilters(IQueryable<TEntity> query, TFilterDto filterDto)
        {
            return query;
        }

        /// <summary>
        /// Позволяет наложить дополнительные фильтры на запрос ПОСЛЕ получения проекции.
        /// </summary>
        /// <param name="query">Запрос списка.</param>
        /// <param name="filter">Фильтр.</param>
        /// <returns></returns>
        protected virtual IQueryable<TListDto> ListDtoQueryCustomFilters(IQueryable<TListDto> query, TFilterDto filter)
        {
            return query;
        }
    }
}
