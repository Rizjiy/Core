using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Domain;
using LinqToDB.Mapping;
using Core.Dto;
using Core.Services;
using System.Linq.Expressions;
using LightInject;
using AutoMapper;
using Core.Internal.Dependency;

namespace Core.MSTest.Services
{
    [TestClass]
    public class MapperTest
    {
        Scope _scope;

        public TestEntityService TestService { get; set; }

        [TestInitialize]
        public void Init()
        {
            //нужен Ioc
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {

                _scope = container.BeginScope();

                container.InjectProperties(this);
            });

        }

        [TestCleanup]
        public void Dispose()
        {
            _scope.Dispose();
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

        public class TestEntityService: ReadonlyEntityServiceBase<TestEntity, TestDto, object, TestDto>
        {
            public IMapper Mapper { get; set; }

            protected override Expression<Func<TestEntity, TestDto>> Projection()
            {
                throw new NotImplementedException();
            }

            public TestDto GetDto(EntityBase entity)
            {
                return Mapper.Map<TestDto>(entity); 
            }
        }

        public class TestAutomapperProfile: Profile, IDependency
        {
            public TestAutomapperProfile()
            {
                CreateMap<TestDto, TestEntity>();
                CreateMap<TestEntity, TestDto>();
            }
        }

        [TestMethod]
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
