using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.Internal.Kendo.DynamicLinq;
using Core.Internal.LinqToDB;
using Core.Services;
using Core.Demo.Domain;
using Core.Demo.Dto;
using Core.Validation;

namespace Core.Demo.Services
{
    public class TransExitNodeActionEntityService : AbstractEntityService<TransExitNodeActionEntity, TransExitNodeActionListDto, TransExitNodeActionFilterDto, TransExitNodeActionDto>
    {
        /// <summary>
        /// Реализация метод сохранения dto-объекта в бд.
        /// </summary>
        /// <param name="dto">DTO-объект для сохранения.</param>
        /// <returns>Сохраненая сущность.</returns>
        public override TransExitNodeActionEntity SaveDto(TransExitNodeActionDto dto)
        {
            var entity = LoadEntityOrCreate(dto);

            entity.HostName = dto.HostName;
            entity.ExternalIP = dto.ExternalIP;
            entity.Action = dto.Action;
            entity.Date = dto.Date;
            entity.DefaultGW = dto.DefaultGW;
            entity.SessionId = dto.SessionId;
            entity.ProfileName = dto.ProfileName;
            entity.VpnName = dto.VpnName;

            entity.Networks = new List<TransNetworkExitNodeInActionEntity>(dto.Network.Length);


            foreach (var networkDto in dto.Network)
            {
                var networkEntity = LoadEntityOrCreate<TransNetworkExitNodeInActionEntity>(dto);
                networkEntity.TransExitNodeAction = entity;
                networkEntity.Name = networkDto.Name;
                networkEntity.Mac = networkDto.Mac;
                networkEntity.Ip = networkDto.IP;
                networkEntity.Mask = networkDto.Mask;
                networkEntity.IpGW = networkEntity.IpGW;

                // заполнить свойства маркированные в ассоциациях(TransExitNodeActionId).
                networkEntity.FillKeys();

                entity.Networks.Add(networkEntity);
                
            }


            new Validation.TransExitNodeActionValidator().ValidateAndThrow(entity);
            InsertOrUpdate(entity);

            foreach (var network in entity.Networks)
                InsertOrUpdate(network);

            return entity;
         
            
        }

        /// <summary>
        /// Списочный запрос
        /// </summary>
        /// <param name="request">Параметры запроса</param>
        /// <returns>Результат, с которым умеет работать Kendo.</returns>
        public override DataSourceResult<TransExitNodeActionListDto> GetQueryResultDto(DataSourceRequestDto<TransExitNodeActionFilterDto> request)
        {
            // Запрос с выборкой гуида сессии и даты последнего действия сесси.
            var queryA = DataContext.GetTable<TransExitNodeActionEntity>()
                  .GroupBy(item => item.SessionId)
                  .Select(g => new { SessionId = g.Key, Date = g.Max(i => i.Date) });

            // Запрос с выборкой последних действий в сессии.
            var query = DataContext.GetTable<TransExitNodeActionEntity>()
                .Join(queryA,
                tena => new { SessionId = tena.SessionId, Date = tena.Date },
                a => new { SessionId = a.SessionId, Date = a.Date },
                (tena, a) => tena);

            // Применение фильтра с датами.
            if (request.FilterDto != null)
            {
                var filter = request.FilterDto;
                if (filter.StartPeriodDateTime.HasValue)
                    query = query.Where(item => item.Date >= filter.StartPeriodDateTime.Value);

                if (filter.EndPeriodDateTime.HasValue)
                    query = query.Where(item => item.Date < filter.EndPeriodDateTime.Value);
            }

            // Общее кол-во.
            int total = query.Count();

            // Пейжинг.
            query = query.OrderBy(item => item.Id).Skip(request.Skip).Take(request.Take);

            // Финальный запрос - соедиение action с network'ами.
            var resultQuery = query.Join(DataContext.GetTable<TransNetworkExitNodeInActionEntity>(),
                tena => tena.Id,
                n => n.TransExitNodeActionId,
                (tena, n) => new { ActionEnity = tena, NetworkEntity = n });
            
            // Часть логики реализовано на IEnumerable, потому-что требуется конкатенация занчения networkIps.
            List<TransExitNodeActionListDto> resultList = new List<TransExitNodeActionListDto>();


            // Группируем по идентификатору сущности Действия.
            foreach (var groupItem in resultQuery.ToArray().GroupBy(i => i.ActionEnity.Id))
            {
                // Получаем первую любое действия, т.к. они одинаковые.
                TransExitNodeActionEntity entity = groupItem.First().ActionEnity;

                // Получаем network с именем LAN.
                var lanNetwork = groupItem.FirstOrDefault(i => i.NetworkEntity != null && i.NetworkEntity.Name == "LAN");

                // Получаем ip-адереса network'a через запятую.
                var networks = groupItem.Where(i => i.NetworkEntity != null).Select(i => i.NetworkEntity);
                string networkIps = networks.Any() ? string.Join(", ", networks.Select(n => n.Ip)) : null;

                resultList.Add(new TransExitNodeActionListDto
                {
                    SessionId = entity.SessionId,
                    Action = entity.Action,
                    Date = entity.Date,
                    ExternalIP = entity.ExternalIP,
                    HostName = entity.HostName,
                    ProfileName = entity.ProfileName,
                    NetworkIps = networkIps,
                    Mac = lanNetwork != null ? lanNetwork.NetworkEntity.Mac : null
                });
            }

            return new DataSourceResult<TransExitNodeActionListDto>
            {
                Total = total,
                Data = resultList
            };

        }

        /// <summary>
        /// Списочный запрос. Возвращает детализация по сессии.
        /// </summary>
        /// <param name="request">Параметры запроса. Содержат последние "действие" сессии для которой нужно вывести детализацию.</param>
        /// <returns>Результат, с которым умеет работать Kendo.</returns>
        public DataSourceResult<TransExitNodeActionListDto> GetSesionDetailsByLastAction(DataSourceRequestDto<TransExitNodeActionListDto> request)
        {
            string sessionId = request.FilterDto.SessionId;

            var query = DataContext.GetTable<TransExitNodeActionEntity>()
                .Where(action => action.SessionId == sessionId);

            // Финальный запрос - соедиение action с network'ами.
            var resultQuery = query.Join(DataContext.GetTable<TransNetworkExitNodeInActionEntity>(),
                tena => tena.Id,
                n => n.TransExitNodeActionId,
                (tena, n) => new { ActionEnity = tena, NetworkEntity = n });

            // Часть логики реализовано на IEnumerable, потому-что требуется конкатенация занчения networkIps.
            List<TransExitNodeActionListDto> resultList = new List<TransExitNodeActionListDto>();


            // Группируем по идентификатору сущности Действия.
            foreach (var groupItem in resultQuery.ToArray().GroupBy(i => i.ActionEnity.Id))
            {
                // Получаем первую любое действия, т.к. они одинаковые.
                TransExitNodeActionEntity entity = groupItem.First().ActionEnity;

                // Получаем network с именем LAN.
                var lanNetwork = groupItem.FirstOrDefault(i => i.NetworkEntity != null && i.NetworkEntity.Name == "LAN");

                // Получаем ip-адереса network'a через запятую.
                var networks = groupItem.Where(i => i.NetworkEntity != null).Select(i => i.NetworkEntity);
                string networkIps = networks.Any() ? string.Join(", ", networks.Select(n => n.Ip)) : null;

                resultList.Add(new TransExitNodeActionListDto
                {
                    Action = entity.Action,
                    Date = entity.Date,
                    ExternalIP = entity.ExternalIP,
                    HostName = entity.HostName,
                    ProfileName = entity.ProfileName,
                    NetworkIps = networkIps,
                    Mac = lanNetwork != null ? lanNetwork.NetworkEntity.Mac : null
                });
            }

            return new DataSourceResult<TransExitNodeActionListDto>
            {
                Data = resultList
            };
        }
        

        /// <summary>
        /// Не вызывается, т.к. переопределен метод GetQueryResultDto, в котором вызывается это метод.
        /// </summary>
        /// <returns></returns>
        protected override Expression<Func<TransExitNodeActionEntity, TransExitNodeActionListDto>> Projection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Не вызывается, т.к. переопределен метод GetQueryResultDto, в котором вызывается это метод.
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<TransExitNodeActionListDto> QueryCustomFilters(IQueryable<TransExitNodeActionListDto> query, TransExitNodeActionFilterDto filter)
        {
            throw new NotImplementedException();
        }

      
        

    
    }
}
