using Core.Internal.Dependency;
using LightInject;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Mapping;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace Core.NUnitTest.Tests.Services.EntityDtoServiceBaseTests
{
    [TestFixture]
    public class EntityDtoServiceBaseTests
    {
        private Scope _scope;

        private IServiceContainer _container;

        public SmartPhoneEntityDtoService SmartPhoneEntityDtoService { get; set; }

        private int _entityId;

        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
               .TestMode(true)
               .ForAssembly(GetType().Assembly)
               .Init((dbConfig, container) =>
               {
                   dbConfig["Core.NUnitTest.Tests.Services.EntityDtoServiceBaseTests"] = "Core";

                   _scope = container.BeginScope();
                   _container = container;

               });

            _container.InjectProperties(this);

            using (DataConnection context = new DataConnection("Core"))
            {
                var schema = context.DataProvider.GetSchemaProvider().GetSchema(context);

                // Создание таблиц для сущности SmartPhoneEntity
                if (schema.Tables.Any(table => table.TableName == typeof(SmartPhoneEntity).GetCustomAttribute<TableAttribute>().Name))
                    context.DropTable<SmartPhoneEntity>();

                context.CreateTable<SmartPhoneEntity>();
            }
        }

        /// <summary>
        /// Тестируется метод загрузки dto <see cref="EntityDtoServiceBase{TEntity, TDto}.LoadDtoOrNull(int)"/>
        /// </summary>
        [Test]
        public void LoadDtoOrNull_Test()
        {
            // --================================================================== Arrangement
            int id;
            using (DataConnection context = new DataConnection("Core"))
            {
                id = Convert.ToInt32(context.InsertWithIdentity(new SmartPhoneEntity
                {
                    Year = 2000,
                    Make = "Yandex",
                    Model = "Huyandex 69"
                }));
            }

            // --================================================================== Action
            var dto = SmartPhoneEntityDtoService.LoadDtoOrNull(id);

            // --================================================================== Assertion
            Assert.IsNotNull(dto);
            Assert.AreEqual(2000, dto.Year);
            Assert.AreEqual("Yandex", dto.Make);
            Assert.AreEqual("Huyandex 69", dto.Model);
        }

        /// <summary>
        /// Тестируется сохранение данных из дто-объекта. <see cref="EntityDtoServiceBase{TEntity, TDto}.SaveDto(TDto)"/>.
        /// Тестирования Инсерта
        /// </summary>
        [Test, Order(1)]
        public void SaveDto_InsertTest()
        {
            // --================================================================== Arrangement
            SmartPhoneDto dto = new SmartPhoneDto
            {
                Make = "Apple",
                Model = "IPhone X",
                Year = 2018
            };

            // --================================================================== Action
            _entityId = SmartPhoneEntityDtoService.SaveDto(dto).Id;

            // --================================================================== Assertion
            SmartPhoneEntity entity;

            using (DataConnection context = new DataConnection("Core"))
            {
                entity = context.GetTable<SmartPhoneEntity>().First(smartPhone => smartPhone.Id == _entityId);
            }

            Assert.AreEqual("Apple", entity.Make);
            Assert.AreEqual("IPhone X", entity.Model);
            Assert.AreEqual(2018, entity.Year);
        }

        /// <summary>
        /// Тестируется сохранение данных из дто-объекта. <see cref="EntityDtoServiceBase{TEntity, TDto}.SaveDto(TDto)"/>.
        /// Тестирования Апдейта
        /// </summary>
        [Test, Order(2)]
        public void SaveDto_UpdateTest()
        {
            // --================================================================== Arrangement
            SmartPhoneDto dto = SmartPhoneEntityDtoService.LoadDtoOrNull(_entityId);
            dto.Year = 2019;

            // --================================================================== Action
            _entityId = SmartPhoneEntityDtoService.SaveDto(dto).Id;

            // --================================================================== Assertion
            SmartPhoneEntity entity;
            using (DataConnection context = new DataConnection("Core"))
            {
                entity = context.GetTable<SmartPhoneEntity>().First(smartPhone => smartPhone.Id == _entityId);
            }

            Assert.AreEqual("Apple", entity.Make);
            Assert.AreEqual("IPhone X", entity.Model);
            Assert.AreEqual(2019, entity.Year);
        }
    }
}
