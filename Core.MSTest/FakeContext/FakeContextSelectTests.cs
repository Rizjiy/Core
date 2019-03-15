using Core.Domain;
using Core.Dto;
using Core.Fakes;
using Core.Internal.Dependency;
using Core.Internal.Kendo.DynamicLinq;
using Core.Utils.Linq2Db;
using LightInject;
using LinqToDB.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.MSTest.FakeContext
{
    [TestClass]
    public class FakeContextSelectTests
    {
        Scope _scope;
        IServiceContainer _container;

        public TaxRateForBankEntityService Service { get; set; }

        /// <summary>
        /// Инициализация фейкоговог контекста
        /// </summary>
        [TestInitialize]
        public void InitFake()
        {
            //нужен Ioc
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    container.SetFakeContext();

                    _scope = container.BeginScope();

                    container.InjectProperties(this);

                    _container = container;
                });


            //Определим фейковые данные
            var bnkSeekList = new List<BnkSeekEntity>
            {
                new BnkSeekEntity
                {
                    Id = 1,
                    Bik = "123456",
                    Name = "Bank1"
                },
                new BnkSeekEntity
                {
                    Id = 2,
                    Bik = "3456789",
                    Name = "Bank2"
                },
                new BnkSeekEntity
                {
                    Id = 3,
                    Bik = "4444444",
                    Name = "Bank4"
                }

            };

            _container.SetTable(bnkSeekList);


            var taxRateForBankList = new List<TaxRateForBankEntity>
              {
                    new TaxRateForBankEntity
                    {
                        Id=1,
                        Bik="123456",
                        DateFrom= new DateTime(2017,1,1),
                        Rate = 0.01M,
                        Bank = bnkSeekList.First(e=>e.Id==1)
                    },
                    new TaxRateForBankEntity
                    {
                        Id=2,
                        Bik="123456",
                        DateFrom= new DateTime(2017,1,2),
                        Rate = 0.02M,
                        Bank = bnkSeekList.First(e=>e.Id==1)
                    },
                    new TaxRateForBankEntity
                    {
                        Id=3,
                        Bik="3456789",
                        DateFrom= new DateTime(2017,1,1),
                        Rate = 0.03M,
                        Bank = bnkSeekList.First(e=>e.Id==2)
                    },
                    new TaxRateForBankEntity
                    {
                        Id=4,
                        Bik="4444444",
                        DateFrom= new DateTime(2017,1,1),
                        Rate = 0.03M,
                        Bank = bnkSeekList.First(e=>e.Id==3)
                    },
              };

            _container.SetTable(taxRateForBankList);

        }



        [TestCleanup]
        public void Dispose()
        {
            _scope.Dispose();
        }

        /// <summary>
        /// Простой тест на фейковых данных
        /// </summary>
        [TestMethod]
        public void SimpleSelectFakeTest()
        {
            var queryDto = Service.GetQuery().Take(3);

            Assert.AreEqual(3, queryDto.Count());

        }

        /// <summary>
        /// запрос на фейках
        /// </summary>
        [TestMethod]
        public void SelectFakeTest()
        {
            var request = new DataSourceRequestDto<object>()
            {
                Take = 3
            };

            var queryDto = Service.GetBankQueryResultDto(request);

            Assert.AreEqual(3, queryDto.Data.Count());

        }


        /// <summary>
        /// сложный запрос на фейках
        /// </summary>
        [TestMethod]
        public void SelectDifficultFakeTest()
        {
            // Подготовка

            var request = new DataSourceRequestDto<TaxRateForBankFilterDto>()
            {
                FilterDto = new TaxRateForBankFilterDto()
                { CurDate = new DateTime(2017, 1, 3) }

            };

            // Действие
            var queryDto = Service.GetQueryResultDto(request);

            // Проверка. Ожидаем три записи, т.к. данные группируются по БИК,
            // а в тестовых данных три уникальных БИКа
            Assert.AreEqual(3, queryDto.Data.Count());
        }

        /// <summary>
        /// Транзакция должна игнорироваться
        /// </summary>
        [TestMethod]
        public void TransactionFakeTest()
        {
            using (Service.DataContext.BeginTransaction())
            {
                var queryDto = Service.GetQuery().Take(3);

                Assert.AreEqual(3, queryDto.Count());
            }
        }

    }
}
