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
    class HintTest
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

                _entity = new CrudEntity
                {
                    Id = 1,
                    BirthDate = new DateTime(2019, 1, 1),
                    Name = "Crud",
                    Year = 20154,
                };


                EntityService.Insert(_entity);

            }
        }

        [Test, Order(1)]
        public void Hint_Test()
        {
            
            // --====================================================== Assertion
            var query = EntityService.GetQuery();

            EntityService.DataContext.QueryHint("option ( optimize for unknown );");

            Assert.IsTrue(((LinqToDB.Linq.IExpressionQuery<CrudEntity>)query).SqlText.IndexOf("option ( optimize for unknown );") >= 0);

           
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
