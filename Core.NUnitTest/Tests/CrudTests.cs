using System.Linq;
using Core.Internal.Dependency;
using Core.NUnitTest.Tests.Sequences;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using NUnit.Framework;

namespace Core.NUnitTest.Tests
{
    [TestFixture]
    public class CrudTests
    {
        private Scope _scope;
        private IServiceContainer _container;
        public PersonEntityService Service { get; set; }

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

      
    }
}
