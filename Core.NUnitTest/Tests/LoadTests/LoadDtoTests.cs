using Core.Internal.Dependency;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.NUnitTest.Tests.LoadTests
{
    [TestFixture]
    public class LoadDtoTests
    {
        private Scope _scope;

        private IServiceContainer _container;

        public BankCardTestEntityService CardService { get; set; }

        public PersonTestEntityService PersonService { get; set; }

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
               .TestMode(true)
               .ForAssembly(GetType().Assembly)
               .Init((dbConfig, container) =>
               {
                   dbConfig["Core.NUnitTest.Tests.LoadTests"] = "Core";

                   _scope = container.BeginScope();
                   _container = container;

               });

            _container.InjectProperties(this);


            using (DataConnection context = new DataConnection("Core"))
            {
                var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);

                // Создание таблиц для сущности Карты
                if (schema.Tables.Any(table => table.TableName == typeof(BankCardTestEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<BankCardTestEntity>();

                context.CreateTable<BankCardTestEntity>();

                // Создание таблицы для сущности человека
                if (schema.Tables.Any(table => table.TableName == typeof(PersonTestEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<PersonTestEntity>();

                context.CreateTable<PersonTestEntity>();

                

            }
        }

     

        [Test]
        public void LoadDtoOrNullTest()
        {
            // --========================================================================================================== Assigement
            PersonService.Insert(new PersonTestEntity
            {
                Id = 101,
                Firstname = "John",
                Surname = "Rambo"
            });

            CardService.Insert(new BankCardTestEntity
            {
                Id = 3,
                Year = 2025,
                Month = 1,
                PersonId = 101
            });

            // --========================================================================================================== Action

            var resultDto = CardService.LoadDtoOrNull(3);

            // --========================================================================================================== Assertion
            Assert.AreEqual(2025, resultDto.Year);
            Assert.AreEqual(1, resultDto.Month);
            Assert.AreEqual("John", resultDto.OwnerFirstname);
            Assert.AreEqual("Rambo", resultDto.OwnerSurname);


        }

       

        [OneTimeTearDown]
        public void Dispose()
        {
            using (DataConnection context = new DataConnection("Core"))
            {
                context.DropTable<BankCardTestEntity>();
                context.DropTable<PersonTestEntity>();
            }
        }
    }
}
