using Core.Internal.Dependency;
using Core.Utils.Linq2Db;
using LightInject;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MSTest.FakeContext
{
    [TestClass]
    public class LoadWithTest
    {

        Scope _scope;

        public TaxRateForBankEntityService Service { get; set; }

        /// <summary>
        /// Обычная инициализация IoC
        /// </summary>
        [TestInitialize]
        public void Init()
        {
            //нужен Ioc
            new DependencyInitializer()
                .TestMode(true)
                .ForAssembly(GetType().Assembly)
                .Init((dbConfig, container) =>
                {
                    dbConfig["Core.MSTest"] = "CN";

                    _scope = container.BeginScope();

                    container.InjectProperties(this);

                });

        }

        [TestCleanup]
        public void Dispose()
        {
            _scope.Dispose();
        }

        /// <summary>
        /// Метод не относится к фейкам, а тестирует расширение LoadWith
        /// </summary>
        [TestMethod]
        public void SelectLoadWithTest()
        {
            //Без LoadWith
            var TaxRate = Service.GetQuery().FirstOrDefault();
            Assert.IsNull(TaxRate.Bank);

            //С LoadWith
            var TaxRate2 = Service.GetQuery().LoadWith(e => e.Bank).FirstOrDefault();
            Assert.IsNotNull(TaxRate2.Bank);

        }

    }
}
