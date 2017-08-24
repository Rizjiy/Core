using Core.Dto;
using Core.Internal.Kendo.DynamicLinq;
using Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Core.Web.Controllers.Demo
{
    public class TableTemplateGridController : ApiController
    {

        public static IEnumerable<TableTemplateGridDto> Source { get; set; } = new List<TableTemplateGridDto>
        {
            new TableTemplateGridDto
                {
                    Id = 1,
                    DateFrom = new DateTime(2016,1,1),
                    Rate = 0.01m
                },
                new TableTemplateGridDto
                {
                    Id = 2,
                    DateFrom = new DateTime(2016,1,2),
                    Rate = 10000.02m
                },

        };



        [HttpPost]
        public DataSourceResult<TableTemplateGridListDto> GetList(DataSourceRequestDto<TableTemplateGridListDto> request)
        {
            var list = Source.Select(i => new TableTemplateGridListDto
            {
                Id = i.Id,
                DateFrom = i.DateFrom,
                Rate = i.Rate
            });

            return list.AsQueryable().ToDataSourceResult(request);

        }

        [HttpPost]
        public TableTemplateGridDto GetById(TableTemplateGridDto dto)
        {
            return Source.FirstOrDefault(i => i.Id == dto.Id);
        }

        [HttpPost]
        public void Save(TableTemplateGridDto dto)
        {
            var foundItem = Source.FirstOrDefault(i => i.Id == dto.Id);
            foundItem.Rate = dto.Rate;

        }


        [HttpPost]
        public void GetValidationException()
        {
            var resultDto = new ValidationFailureListDto
            {
                Violations = new List<ValidationFailureDto>(new ValidationFailureDto[] {
                new ValidationFailureDto
                {
                    Message = "Тестовая ошибка 1",
                    SeverityCode = (int)SeverityEnum.Error
                },
                new ValidationFailureDto
                {
                    Message = "Тестовая ошибка 2",
                    SeverityCode = (int)SeverityEnum.Error
                } } )
            };

            throw new ValidationFailureException(resultDto);

            //ValidationUtils.ThrowCustomFailure("Тестовая ошибка!");
        }

        [HttpPost]
        public void GetException(TableTemplateGridDto dto)
        {
            throw new Exception("Необработанное исключение");
        }

        public class TableTemplateGridListDto 
        {
            public int Id { get; set; }
            public DateTime DateFrom { get; set; }
            public decimal Rate { get; set; }
        }

        public class TableTemplateGridDto
        {
            public int Id { get; set; }
            public DateTime DateFrom { get; set; }
            public decimal Rate { get; set; }
        }


    }
}