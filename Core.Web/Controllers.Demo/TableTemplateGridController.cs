using Core.Dto;
using Core.Internal.Kendo.DynamicLinq;
using Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.Web.Cache;
using Core.Utils;
using System.IO;
using System.Text;
using System.Net.Http.Headers;
using Core.Utils.WebApi;
using Core.Web.Core;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Web.Controllers.Demo
{
    public class TableTemplateGridController : ApiController
    {
        public UserCache UserCache { get; set; }

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
            new TableTemplateGridDto
            {
                Id = 3,
                DateFrom = new DateTime(2016,1,2),
                Rate = 10000.1234567m
            },
            new TableTemplateGridDto
            {
                Id = 3,
                DateFrom = new DateTime(2016,1,2),
                Rate = 10000m
            },
            new TableTemplateGridDto
            {
                Id = 3,
                DateFrom = new DateTime(2016,1,2),
                Rate = null
            },
            new TableTemplateGridDto {
                Id = 4,
                DateFrom = new DateTime(2016,1,2),
                Rate = 10000
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
            var failures =  new []
            {
                new ValidationFailure(
                    propertyName: "property1",
                    errorMessage: "Тестовая ошибка 1"),
                new ValidationFailure(
                    propertyName: "property2",
                    errorMessage: "Тестовая ошибка 2")
            };

            //throw new ValidationException(failures);

            ValidationUtils.ThrowCustomFailure("Тестовая ошибка!");
        }

        [HttpPost]
        public void GetException(TableTemplateGridDto dto)
        {
            throw new Exception("Необработанное исключение");
        }

        [HttpGet]
        public UserDto[] GetCachedUsers()
        {
            //var unexisted = UserCache.Get("UserTheFourth");

            return new UserDto[]
            {
                UserCache.Get("UserthefIRST"),
                UserCache.Get("UserTheSecond"),
                UserCache.Get("UserThethirD"),
                
            };
        }

        [HttpPost]
        public HttpResponseFile DownloadExcel(DataSourceRequestDto<TableTemplateGridListDto> request)
        {
            var list = Source.Select(i => new TableTemplateGridListDto
            {
                Id = i.Id,
                DateFrom = i.DateFrom,
                Rate = i.Rate
            });

            string strDto =  list.AsQueryable().ToDataSourceResult(request).ToJson();

            string fileName = "TestFile.txt";
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(strDto));

            return ms.AttachmentResponse(fileName);

        }

        public class TableTemplateGridListDto 
        {
            public int Id { get; set; }
            public DateTime DateFrom { get; set; }
            public decimal? Rate { get; set; }
        }

        public class TableTemplateGridDto
        {
            public int Id { get; set; }
            public DateTime DateFrom { get; set; }
            public decimal? Rate { get; set; }
        }


    }
}