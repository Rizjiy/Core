using Core.Internal.Dependency;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace Core.NUnitTest.Tests.Logging
{
    [TestFixture]
    public class LoggingTests
    {
        private Scope _scope;

        private IServiceContainer _container;
        
        public LoggingEntityService EntityService { get; set; }

        private LoggingEntity _entity;

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
               .TestMode(true)
               .ForAssembly(GetType().Assembly)
               .Init((dbConfig, container) =>
               {
                   dbConfig["Core.NUnitTest.Tests.Logging"] = "Core";

                   _scope = container.BeginScope();
                   _container = container;

               });

            _container.InjectProperties(this);


            using (DataConnection context = new DataConnection("Core"))
            {
                var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);

                // Создание таблиц для сущности LoggingEntity
                if (schema.Tables.Any(table => table.TableName == typeof(LoggingEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<LoggingEntity>();

                context.CreateTable<LoggingEntity>();

                // Создание таблицы-логирования
                if (schema.Tables.Any(table => table.TableName == typeof(LoggingLogEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<LoggingLogEntity>();

                context.CreateTable<LoggingLogEntity>();

            }

            // Накидываем сущности, чтобы быть точно уверенным, что удалится нужная сущность
            EntityService.Insert(new LoggingEntity
            {
                Id = 1,
            });
            EntityService.Insert(new LoggingEntity
            {
                Id = 2
            });
        }

        /// <summary>
        /// Логирование при инсерте
        /// </summary>
        [Test,Order(1)]
        public void Insert_Log_Test()
        {
           

            // --========================================================================================================== Action
            EntityService.Insert(_entity = new LoggingEntity
            {
                Id = 3,
                Name = "SomeName",
                Year = 100,
                BirthDate = new DateTime(1982, 5, 2),
                Amount = 1200,
                LogUser_Id = 1000,
                NullableName = null
            });

            // --========================================================================================================== Assertion
            var entityFromBase = EntityService.LoadEntity(_entity.Id);
            Assert.AreEqual("SomeName", entityFromBase.Name);
            Assert.AreEqual(100, entityFromBase.Year);
            Assert.AreEqual(new DateTime(1982, 5, 2), entityFromBase.BirthDate);
            Assert.AreEqual(1200, entityFromBase.Amount);


            LoggingLogEntity logEntity = null;

            using (DataConnection context = new DataConnection("Core"))
            {
                // Получаем залогированную сущность
                logEntity = context.GetTable<LoggingLogEntity>().Single(item=>item.IdLog == _entity.Id);
            }

            Assert.AreEqual("Insert", logEntity.ActionLog);
            Assert.AreEqual("SomeName", logEntity.Name);
            Assert.AreEqual(100, logEntity.Year);
            Assert.AreEqual(new DateTime(1982, 5, 2), logEntity.BirthDate);
            Assert.AreEqual(1200, logEntity.Amount);
            Assert.AreEqual(1000, logEntity.UserIdLog);
            Assert.AreEqual(null, logEntity.NullableName);

        }

        /// <summary>
        /// Логирование при апдейте
        /// </summary>
        [Test, Order(2)]
        public void Update_Log_Test()
        {


            // --========================================================================================================== Assigement
            _entity.Name = "NewName";
            _entity.Year = 2000;
            _entity.BirthDate = new DateTime(2012, 11, 11);
            _entity.LogUser_Id = 1234;

            // --========================================================================================================== Action
            EntityService.Update(_entity);

            // --========================================================================================================== Assertion
            var entityFromBase = EntityService.LoadEntity(_entity.Id);
            Assert.AreEqual("NewName", entityFromBase.Name);
            Assert.AreEqual(2000, entityFromBase.Year);
            Assert.AreEqual(new DateTime(2012, 11, 11), entityFromBase.BirthDate);
            Assert.AreEqual(1200, entityFromBase.Amount);


         
            LoggingLogEntity logEntity = null;

            using (DataConnection context = new DataConnection("Core"))
            {
                // Получаем залогированную сущность
                logEntity = context.GetTable<LoggingLogEntity>().Single(log=>log.ActionLog == "Update" && log.IdLog == _entity.Id);
            }

            Assert.AreEqual("Update", logEntity.ActionLog);
            Assert.AreEqual("NewName", logEntity.Name);
            Assert.AreEqual(2000, logEntity.Year);
            Assert.AreEqual(new DateTime(2012, 11, 11), logEntity.BirthDate);
            Assert.AreEqual(1200, logEntity.Amount);
            Assert.AreEqual(1234, logEntity.UserIdLog);
        }

        /// <summary>
        /// Логирование при делете
        /// </summary>
        [Test, Order(3)]
        public void Delete_Log_test()
        {
            // --========================================================================================================== Arrangement

          


            // Идентификатор пользователя удаляющего запись
            _entity.LogUser_Id = 123453;

            // --========================================================================================================== Action
            EntityService.Delete(_entity);

            var entityFromBase = EntityService.LoadEntityOrNull(_entity.Id);

            Assert.IsNull(entityFromBase);


            // --========================================================================================================== Assertion
            LoggingLogEntity logEntity = null;

            using (DataConnection context = new DataConnection("Core"))
            {
                // Получаем залогированную сущность
                logEntity = context.GetTable<LoggingLogEntity>().Single(log => log.ActionLog == "Delete" && log.IdLog == _entity.Id);
            }

            Assert.AreEqual(logEntity.UserIdLog, 123453);
            Assert.AreEqual("Delete", logEntity.ActionLog);
            //--------------------------------------- Все бизнес свойсва должны быть заполены так же как перед удалением.

            Assert.AreEqual("NewName", logEntity.Name);
            Assert.AreEqual(2000, logEntity.Year);
            Assert.AreEqual(new DateTime(2012, 11, 11), logEntity.BirthDate);
            Assert.AreEqual(1200, logEntity.Amount);
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            using (DataConnection context = new DataConnection("Core"))
            {
                context.DropTable<LoggingEntity>();
                context.DropTable<LoggingLogEntity>();
            }
        }
    }
}
