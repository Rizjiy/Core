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
    /// <summary>
    /// Тест логирвания при инсерте сущности с идентити идетификатором
    /// </summary>
    [TestFixture]
    public class LoggingIdentityTests
    {
        private Scope _scope;

        private IServiceContainer _container;

        public LoggingWithIdentityEntityService EntityService { get; set; }

        private LoggingWithIdentityEntity _entity;

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

                // Создание таблиц для сущности LoggingWithIdentityEntity
                if (schema.Tables.Any(table => table.TableName == typeof(LoggingWithIdentityEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<LoggingWithIdentityEntity>();

                context.CreateTable<LoggingWithIdentityEntity>();

                // Создание таблицы-логирования
                if (schema.Tables.Any(table => table.TableName == typeof(LoggingWithIdentityLogEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<LoggingWithIdentityLogEntity>();

                context.CreateTable<LoggingWithIdentityLogEntity>();

            }
        }

        /// <summary>
        /// Логирование при инсерте
        /// </summary>
        [Test]
        public void Insert_Log_Test()
        {


            // --========================================================================================================== Action
            EntityService.Insert(_entity = new LoggingWithIdentityEntity
            {
                Name = "SomeName",
                Year = 100,
                BirthDate = new DateTime(1982, 5, 2),
                Amount = 1200,
                LogUser_Id = 1000
            });

            // --========================================================================================================== Assertion
            var entityFromBase = EntityService.LoadEntity(_entity.Id);
            Assert.AreEqual("SomeName", entityFromBase.Name);
            Assert.AreEqual(100, entityFromBase.Year);
            Assert.AreEqual(new DateTime(1982, 5, 2), entityFromBase.BirthDate);
            Assert.AreEqual(1200, entityFromBase.Amount);


            LoggingWithIdentityLogEntity logEntity = null;

            using (DataConnection context = new DataConnection("Core"))
            {
                logEntity = context.GetTable<LoggingWithIdentityLogEntity>().Single();
            }

            Assert.AreEqual("Insert", logEntity.ActionLog);
            Assert.AreEqual("SomeName", logEntity.Name);
            Assert.AreEqual(100, logEntity.Year);
            Assert.AreEqual(new DateTime(1982, 5, 2), logEntity.BirthDate);
            Assert.AreEqual(1200, logEntity.Amount);
            Assert.AreEqual(1000, logEntity.UserIdLog);

        }

        [OneTimeTearDown]
        public void Dispose()
        {
            using (DataConnection context = new DataConnection("Core"))
            {
                context.DropTable<LoggingWithIdentityEntity>();
                context.DropTable<LoggingWithIdentityLogEntity>();
            }
        }


    }
}
