using AutoMapper;
using Core.Domain;
using Core.Dto;
using Core.Interfaces;
using Core.Internal.Dependency;
using Core.Services;
using LightInject;
using LinqToDB.Mapping;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.NUnitTest.Tests.Mapper
{
    [TestFixture]
    public class MapperTest
    {

        private Scope _scope;

        private IServiceContainer _container;

        public TestEntityService TestService { get; set; }


        [OneTimeSetUp]
        public void Init()
        {
            new DependencyInitializer()
               .TestMode(true)
               .ForAssembly(GetType().Assembly)
               .Init((dbConfig, container) =>
               {
                   //dbConfig["Core.NUnitTest.Tests.Logging"] = "Core";

                   _scope = container.BeginScope();
                   _container = container;

               });

            _container.InjectProperties(this);


         
        }

        public class TestEntity : EntityBase
        {
            [PrimaryKey]
            [Column]
            public override int Id { get; set; }

            [Column]
            public string Name { get; set; }

        }

        public class TestDto : EntityDto
        {
            public string Name { get; set; }
        }

        public class TestEntityService : ReadonlyEntityServiceBase<TestEntity, BaseListDto, object, TestDto>
        {
           

            protected override Expression<Func<TestEntity, BaseListDto>> Projection()
            {
                throw new NotImplementedException();
            }

            public TestDto GetDto(EntityBase entity)
            {
                return Mapper.Map<TestDto>(entity);
            }
        }

        public class TestAutomapperProfile : Profile, IDependency
        {
            public TestAutomapperProfile()
            {
                CreateMap<TestDto, TestEntity>();
                CreateMap<TestEntity, TestDto>();
            }
        }

        [Test]
        public void MapperCorrect()
        {
            //Проверяем что entity корректно мапится в Dto
            var entity = new TestEntity
            {
                Id = 1,
                Name = "Test"
            };

            var dto = TestService.GetDto(entity);

            Assert.AreEqual(entity.Id, dto.Id);
            Assert.AreEqual(entity.Name, dto.Name);

        }

    }
}
