using Core.Internal.Dependency;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace Core.NUnitTest.Tests.Services.EntityServiceBaseTests
{
    [TestFixture]
    public class EntityServiceBaseTests
    {
        private Scope _scope;

        private IServiceContainer _container;

        public AddressEntityService AddressEntityService { get; set; }

        private int _entityId;

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
               .TestMode(true)
               .ForAssembly(GetType().Assembly)
               .Init((dbConfig, container) =>
               {
                   dbConfig["Core.NUnitTest.Tests.Services.EntityServiceBaseTests"] = "Core";

                   _scope = container.BeginScope();
                   _container = container;

               });

            _container.InjectProperties(this);

            using (DataConnection context = new DataConnection("Core"))
            {
                var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);

                // Создание таблиц для сущности AddressEntity
                if (schema.Tables.Any(table => table.TableName == typeof(AddressEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<AddressEntity>();

                context.CreateTable<AddressEntity>();
            }
        }

        [Test, Order(1)]
        public void SaveEntity_Insert_Test()
        {
            // --================================================================== Arrangement
            AddressEntity entity = new AddressEntity
            {
                BuildingNumber = 12,
                City = "Москва",
                Street = "Большая Семеновская"
            };

            // --================================================================== Action
            _entityId = AddressEntityService.SaveEntity(entity).Id;

            // --================================================================== Assertion
            using (DataConnection context = new DataConnection("Core"))
            {
                entity = context.GetTable<AddressEntity>().First(address => address.Id == _entityId);
            }

            Assert.AreEqual(12, entity.BuildingNumber);
            Assert.AreEqual("Москва", entity.City);
            Assert.AreEqual("Большая Семеновская", entity.Street);
        }

        [Test, Order(2)]
        public void SaveEntity_Update_Test()
        {
            // --================================================================== Arrangement
            AddressEntity entity = null;

            using (DataConnection context = new DataConnection("Core"))
            {
                entity = context.GetTable<AddressEntity>().First(address => address.Id == _entityId);
            }

            entity.BuildingNumber = 14;
            // --================================================================== Action

            AddressEntityService.SaveEntity(entity);


            using (DataConnection context = new DataConnection("Core"))
            {
                entity = context.GetTable<AddressEntity>().First(address => address.Id == _entityId);
            }

            // --================================================================== Assertion
            Assert.AreEqual(14, entity.BuildingNumber);
            Assert.AreEqual("Москва", entity.City);
            Assert.AreEqual("Большая Семеновская", entity.Street);
        }
    }
}
