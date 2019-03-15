using Core.Internal.Dependency;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace Core.NUnitTest.Tests.CRUD
{
    [TestFixture]
    class CrudTests
    {
        private Scope _scope;

        private IServiceContainer _container;

        public CrudEntityService EntityService { get; set; }

        private CrudEntity _entity;

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
               .TestMode(true)
               .ForAssembly(GetType().Assembly)
               .Init((dbConfig, container) =>
               {
                   dbConfig["Core.NUnitTest.Tests.CRUD"] = "Core";

                   _scope = container.BeginScope();
                   _container = container;

               });

            _container.InjectProperties(this);


            using (DataConnection context = new DataConnection("Core"))
            {
                var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);

                // Создание таблиц для сущности LoggingEntity
                if (schema.Tables.Any(table => table.TableName == typeof(CrudEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<CrudEntity>();

                context.CreateTable<CrudEntity>();


            }
        }

        [Test, Order(1)]
        public void InsertTest()
        {
            // --====================================================== Assigenment
            _entity = new CrudEntity
            {
                Id = 1,
                BirthDate = new DateTime(2019, 1, 1),
                Name = "Crud",
                Year = 20154,
            };

            // --====================================================== Action
            EntityService.Insert(_entity);

            // --====================================================== Assertion
            var entity = EntityService.LoadEntity(_entity.Id);

            Assert.AreEqual(new DateTime(2019, 1, 1), entity.BirthDate);
            Assert.AreEqual("Crud", entity.Name);
            Assert.AreEqual(20154, entity.Year);
           
        }

        [Test, Order(2)]
        public void UpdateTest()
        {
            // --====================================================== Assigenment
            _entity.Name = "NewCrudName";

            // --====================================================== Action
            EntityService.Update(_entity);

            // --====================================================== Assertion
            var entity = EntityService.LoadEntity(_entity.Id);
            Assert.AreEqual("NewCrudName", entity.Name);
        }

        [Test, Order(3)]
        public void DeleteTest()
        {
            // --====================================================== Action
            EntityService.Delete(_entity);

            // --====================================================== Assertion
            Assert.IsNull(EntityService.LoadEntityOrNull(_entity.Id));
        }

        [OneTimeTearDown]
        public void Dispose()
        {
            using (DataConnection context = new DataConnection("Core"))
            {
                context.DropTable<CrudEntity>();
            }
        }
    }
}
