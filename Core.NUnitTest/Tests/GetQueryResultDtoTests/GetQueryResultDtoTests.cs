using Core.Internal.Dependency;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace Core.NUnitTest.Tests.GetQueryResultDtoTests
{
    [TestFixture]
    public class GetQueryResultDtoTests
    {
        private Scope _scope;

        private IServiceContainer _container;

        public CarEntityService Service { get; set; }
        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
               .TestMode(true)
               .ForAssembly(GetType().Assembly)
               .Init((dbConfig, container) =>
               {
                   dbConfig["Core.NUnitTest.Tests"] = "Core";

                   _scope = container.BeginScope();
                   _container = container;

               });

            _container.InjectProperties(this);


            using (DataConnection context = new DataConnection("Core"))
            {
                var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);


                string tableName = typeof(CarEntity).GetCustomAttribute<TableAttribute>().Name;
                // Создание таблиц для сущности Карты
                if (schema.Tables.Any(table => table.TableName == typeof(CarEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<CarEntity>();

                context.CreateTable<CarEntity>();


                context.Insert(new CarEntity { Id = 1, Make = "BMW", Model = "X5", Year = 2017 });
                context.Insert(new CarEntity { Id = 2, Make = "Mercedes", Model = "C190", Year = 2014 });
                context.Insert(new CarEntity { Id = 3, Make = "Tesla", Model = "Energe", Year = 2019 });

            }
        }

        [Test]
        public void GetQueryResultDtoTest()
        {
            // --=============================== Action
            var result = Service.GetQueryResultDto(new Internal.Kendo.DynamicLinq.DataSourceRequestDto<object>{});

            // Тестируется только новый мэппинг автомаппера. Ничего при мэппинге не упало.

        }


        [OneTimeTearDown]
        public void Dispose()
        {
            using (DataConnection context = new DataConnection("Core"))
            {
                context.DropTable<CarEntity>();
            }
        }
    }
}
