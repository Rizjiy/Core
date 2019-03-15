using Core.Internal.Dependency;
using System.Linq;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using NUnit.Framework;
using System.Reflection;

namespace Core.NUnitTest.Tests.Sequences
{
    [TestFixture]
    public class EntityServiceBase_CreateEntityTests
    {
        private Scope _scope;
        private IServiceContainer _container;

        public PersonEntityService Service { get; set; }

        public PersonEuidIdentityEntityService EuidIdentityService { get; set; }

        public PersonOnlyEuidEntityService OnlyEuidService { get; set; }

        public PersonOnlyIdentityEntityService OnlyIdentityService { get; set; }



        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    dbConfig["Core.NUnitTest.Tests.Sequences"] = "Core";

                    _scope = container.BeginScope();
                    _container = container;

                });

            _container.InjectProperties(this);

            // Создаем таблицы используемые для генерации Euid-последовательностей
            using (DataConnection context = new DataConnection("Core"))
            {
                var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);

                if (schema.Tables.All(table => table.TableName != "Table"))
                    context.CreateTable<TableEntity>();

                if (schema.Tables.All(table => table.TableName != "EntityUNID"))
                    context.CreateTable<EntityUnIdEnity>();

                if (schema.Tables.All(table => table.TableName != "Person"))
                    context.CreateTable<PersonEntity>();

                context.Insert(new TableEntity
                {
                    TableName = "Person"
                });

               
            }
        }

        /// <summary>
        /// Сервис с сущностью в которой свойство Id корректно помечено атрибутом EuidAttribute
        /// </summary>
        [Test]
        public void CreateWithEuidPKEntity()
        {
            PersonEntity person = Service.CreateEntity();
        }

        /// <summary>
        /// Сервис с сущностью в которой свойство Id некорректно помечено атрибутом EuidAttribute:
        /// Нельзя использовать совместно атрибута EuidAttribute и IdentityAttribute.
        /// </summary>
        [Test]
        public void CreateWithEuidIdentityPKEntity()
        {
            var exception = Assert.Throws<CustomAttributeFormatException>(() =>
            {
                var person = EuidIdentityService.CreateEntity();
            });

            var expectedMessage = "В классе PersonEuidIdentityEntity cвойство Id помечено сразу двумя взаимоисключающимися атрибутами: EuidAttribute и IdentityAttribute.";

            Assert.AreEqual(expectedMessage, exception.Message);
        }


        /// <summary>
        /// Сервис с сущностью в которой свойство Id некорректно помечено атрибутом EuidAttribute:
        /// Не хватает атрибута PrimaryKeyAttribute.
        /// </summary>
        [Test]
        public void CreateWithOnlyEuidEntity()
        {
            var exception = Assert.Throws<CustomAttributeFormatException>(() => {

                var person = OnlyEuidService.CreateEntity();
            });

            var expectedMessage = "В классе PersonOnlyEuidEntity cвойство Id помеченое атрибутом EuidAttribute должно быть также помечено атрибутом PrimaryKeyAttribute.";

            Assert.AreEqual(expectedMessage, exception.Message);
        }


        /// <summary>
        /// Сервис с сущностью в которой свойство Id корректно помечено атрибутом IdentityAttribute:
        /// </summary>
        [Test]
        public void CreateWithOnlyIdentity()
        {
            var person = OnlyIdentityService.CreateEntity();

            Assert.AreEqual(0, person.Id);
        }

        [OneTimeTearDown]
        public virtual void Dispose()
        {
            using (DataConnection context = new DataConnection("Core"))
            {
                context.DropTable<TableEntity>();
                context.DropTable<EntityUnIdEnity>();
                context.DropTable<PersonEntity>();
            }
        }
    }
}
