using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Services;
using FluentValidation;
using LightInject;
using Core.Internal.Dependency;

namespace Core.MSTest.Validation
{
    /// <summary>
    /// Валидаторы всегда регистрируются в контейнере 
    /// </summary>
    [TestClass]
    public class RegisterValidatorTest
    {
        Scope _scope;

        public ValidatorTest Validator { get; set; }

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

        public class TestDto 
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class ValidatorTest: AbstractValidator<TestDto>
        {
            public ValidatorTest()
            {
                RuleFor(x => x.Id)
                    .NotEmpty();
            }
        }

        [TestMethod]
        public void ValidatorFromIoc()
        {
            TestDto dto = new TestDto
            {
                Id = 0,
                Name = "Test Dto"
            };

            var results = Validator.Validate(dto);

            Assert.AreEqual(results.Errors.Count, 1);

        }
    }
}
