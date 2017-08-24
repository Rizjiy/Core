using Core.Domain;
using Core.Dto;
using Core.Fakes;
using Core.Internal.Dependency;
using Core.Internal.Kendo.DynamicLinq;
using Core.Internal.LinqToDB;
using Core.Services;
using Core.Validation;
using Core.Utils.Linq2Db;
using LightInject;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;



namespace Core.MSTest.FakeContext
{

    public class TaxRateForBankEntityService : AbstractEntityService<TaxRateForBankEntity, TaxRateForBankListDto, TaxRateForBankFilterDto, TaxRateForBankDto>
    {



        protected override Expression<Func<TaxRateForBankEntity, TaxRateForBankListDto>> Projection()
        {
            return e => new TaxRateForBankListDto
            {
                Id = e.Id,
                DateFrom = e.DateFrom,
                Bik = e.Bik,
                Rate = e.Rate,
                UserLog = e.UserLog,
                BankName = e.Bank.Name != null ? e.Bank.Name : "Банк не найден!"
            };

        }

        public override TaxRateForBankEntity SaveDto(TaxRateForBankDto dto)
        {
            //Проверим, если Бик и Дата совпадает - генерируем ошибку
            if (dto.Id == 0)
            {
                var existingItem = GetQuery().Where(e => e.DateFrom == dto.DateFrom && e.Bik == dto.Bank.Bik).FirstOrDefault();
                if (existingItem != null)
                {
                    ValidationUtils.ThrowCustomFailure("Ставка для банка уже существует на это число!");
                }
            }
            //

            TaxRateForBankEntity entity = LoadEntityOrCreate(dto);

            //todo: поменять на Automapper
            entity.DateFrom = dto.DateFrom;
            entity.Bik = dto.Bank.Bik;
            entity.Rate = dto.Rate;
            entity.UserLog = dto.UserLog;

            InsertOrUpdate(entity);

            return entity;
        }

        protected override IQueryable<TaxRateForBankEntity> QueryCustomFilters(IQueryable<TaxRateForBankEntity> query, TaxRateForBankFilterDto filterDto)
        {
            return query.Where(e => e.DateFrom <= filterDto.CurDate)
                .GroupBy(e => e.Bik)
                .Select(g => new
                { Bik = g.Key, MaxDate = g.Max(e => e.DateFrom) })
                .Join(query
                , o => new { Bik = o.Bik, DateFrom = o.MaxDate }
                , i => new { i.Bik, i.DateFrom }
                , (o, i) => i);
        }

        public TaxRateForBankDto LoadDto(int id)
        {
            var entity = LoadEntity(id);

            //todo: заменить на automapper
            return new TaxRateForBankDto
            {
                Id = entity.Id,
                DateFrom = entity.DateFrom,
                Bank = new BankDto
                {
                    Bik = entity.Bik,
                    Name = entity.Bank != null ? entity.Bank.Name : "Банк не найден!",
                },
                Rate = entity.Rate,
                UserLog = entity.UserLog,
            };
        }

        public override TaxRateForBankEntity LoadEntity(int id)
        {
            return DataContext
                .GetTable<TaxRateForBankEntity>()
                //todo .LoadWith(e => e.Bank)
                .First(e => e.Id == id);
        }

        /// <summary>
        /// Получает Список всех банков. todo: Вынести  в отдельный сервис
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public DataSourceResult<BankDto> GetBankQueryResultDto(DataSourceRequestDto<object> request)
        {
            IQueryable<BankDto> query = DataContext.GetTable<BnkSeekEntity>().
                                                Select(e => new BankDto
                                                {
                                                    Bik = e.Bik,
                                                    Name = e.Name
                                                });

            return query.ToDataSourceResult(request);
        }

    }

    [Table(Name = "TaxRatesForBank", Schema = "dbo", Database = "ClientNetwork")]
    public class TaxRateForBankEntity : EntityBase
    {
        [Column]
        [PrimaryKey]
        public override int Id { get; set; }

        /// <summary>
        /// Дата начала (переодичность день)
        /// </summary>
        [Column]
        public DateTime DateFrom { get; set; }

        [Column(CanBeNull = false)]
        public string Bik { get; set; }

        /// <summary>
        /// Ставка в долях
        /// </summary>
        [Column]
        public decimal Rate { get; set; }

        [Column]
        public string UserLog { get; set; }

        /// <summary>
        /// Банк. Может быть Null
        /// </summary>
        [Association(OtherKey = "Bik", ThisKey = "Bik", CanBeNull = true)]
        public BnkSeekEntity Bank { get; set; }


    }

    /// <summary>
    /// Справочник банков
    /// </summary>
    [Table(Name = "bnkseek", Schema = "dbo", Database = "ClientNetwork")]
    public class BnkSeekEntity : EntityBase
    {

        public override int Id { get; set; }

        /// <summary>
        /// БИК 
        /// </summary>
        [Column(Name = "NEWNUM"), NotNull]
        public string Bik { get; set; }

        //[Column]
        //public string Real { get; set; }

        //[Column]
        //public string Tnp { get; set; }

        ///// <summary>
        ///// город
        ///// </summary>
        //[Column]
        //public string Nnp { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        [Column(Name = "NAMEP")]
        public string Name { get; set; }

        //[Column(Name = "NAMEN")]
        //public string Namen { get; set; }

        /// <summary>
        /// Корр счет
        /// </summary>
        [Column]
        public string Ksnp { get; set; }

        //[Column]
        //public string Imy { get; set; }

        //[Column]
        //public string P { get; set; }


    }

    public class TaxRateForBankListDto : EntityDto
    {
        public DateTime DateFrom { get; set; }
        public string Bik { get; set; }
        public decimal Rate { get; set; }
        public string UserLog { get; set; }
        public string BankName { get; set; }

        public NamedEntityDto Bank { get; set; }
    }

    public class TaxRateForBankFilterDto
    {
        public DateTime CurDate { get; set; }
    }

    public class TaxRateForBankDto : EntityDto
    {
        public DateTime DateFrom { get; set; }
        //public string Bik { get; set; }
        public decimal Rate { get; set; }
        public string UserLog { get; set; }

        public BankDto Bank { get; set; }

    }

    public class BankDto : EntityDto
    {
        public string Bik { get; set; }

        /// <summary>
        /// Наименование банка
        /// </summary>
        public string Name { get; set; }

    }



}
