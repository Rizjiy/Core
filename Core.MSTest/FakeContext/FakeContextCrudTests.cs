using Core.Fakes;
using Core.Internal.Dependency;
using LightInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MSTest.FakeContext
{
    [TestClass]
    public class FakeContextCrudTests
    {
        Scope _scope;
        IServiceContainer _container;

        public IServiceContainer container { get; set; }

        public TaxRateForBankEntityService Service { get; set; }


        /// <summary>
        /// Инициализация фейкововово контекста
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
                        DateFrom = new DateTime(2017,1,1),
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
        /// Тест проверяет запись сущности
        /// </summary>
        [TestMethod]
        public void FakeInsertTest()
        {
            var entity = new TaxRateForBankEntity
            {
                Id = 333,
                Bik = "333333333",
                DateFrom = new DateTime(2017, 3, 1),
                Rate = 0.3m,
                UserLog = "testUser"
            };

            //Количество до
            var oldCount = Service.GetQuery().Count();

            //Инсертим сущность
            int count = Service.DataContext.Insert<TaxRateForBankEntity>(entity);

            //Ищем нашу сущность
            var foundEntity = Service.GetQuery().FirstOrDefault(e => e.Id == 333);

            //количество после
            var newCount = Service.GetQuery().Count();

            Assert.AreEqual(1, count);
            Assert.AreEqual(oldCount + 1, newCount);
            Assert.AreEqual(entity.Bik, foundEntity.Bik);
        }

        /// <summary>
        /// Тест проверяет запись сущности c Identity
        /// </summary>
        [TestMethod]
        public void FakeInsertWithIdentityTest()
        {
            var entity = new TaxRateForBankEntity
            {
                Id = 0,
                Bik = "333333333",
                DateFrom = new DateTime(2017, 3, 1),
                Rate = 0.3m,
                UserLog = "testUser"
            };

            //Количество до
            var oldCount = Service.GetQuery().Count();

            //Инсертим сущность
            Service.DataContext.InsertWithIdentity(entity);

            //Ищем нашу сущность
            var foundEntity = Service.GetQuery().FirstOrDefault(e => e.Id == 5);

            //количество после
            var newCount = Service.GetQuery().Count();

            Assert.IsNotNull(foundEntity);
            Assert.AreEqual(oldCount + 1, newCount);
            Assert.AreEqual(entity.Bik, foundEntity.Bik);
        }


        /// <summary>
        /// Тест проверяет обновление сущности
        /// </summary>
        [TestMethod]
        public void FakeUpdateTest()
        {
            var entity = new TaxRateForBankEntity
            {
                Id = 1, //существует
                Bik = "333333333", //новый
                DateFrom = new DateTime(2017, 3, 1),
                Rate = 0.3m,
                UserLog = "testUser"
            };

            // Количество до обновления
            var oldCount = Service.GetQuery().Count();

            // Обновление сущность
            int count = Service.DataContext.Update<TaxRateForBankEntity>(entity);

            //Ищем нашу сущность
            var foundEntity = Service.GetQuery().First(e => e.Id == 1);

            //количество после обновления
            var newCount = Service.GetQuery().Count();

            Assert.AreEqual(1, count);
            Assert.AreEqual(oldCount, newCount);
            Assert.AreEqual(entity.Bik, foundEntity.Bik);
        }

        /// <summary>
        /// Тест проверяет удаление сущности
        /// </summary>
        [TestMethod]
        public void FakeDeleteTest()
        {
            var entity = new TaxRateForBankEntity
            {
                Id = 1, //существует
                Bik = "333333333", 
                DateFrom = new DateTime(2017, 3, 1),
                Rate = 0.3m,
                UserLog = "testUser"
            };

            // Количество до удаления
            var oldCount = Service.GetQuery().Count();

            // Удаляем сущность
            int count = Service.DataContext.Delete<TaxRateForBankEntity>(entity);

            // Была ли удалена сущность с идентификатором равным 1.
            var isDeleted = !Service.GetQuery().Any(e => e.Id == 1);

            // Количество после удаления
            var newCount = Service.GetQuery().Count();

            Assert.AreEqual(1, count);
            Assert.AreEqual(oldCount - 1, newCount);
            Assert.IsTrue(isDeleted);
        }


    }
}
