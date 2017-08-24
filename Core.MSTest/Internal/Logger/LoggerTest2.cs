using LightInject;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Core.Log;
using Core.Internal.Dependency;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.MSTest.Internal.Logger
{
    [TestClass]
    public class LoggetTest2
    {
        Scope _scope;

        public Logger<LoggetTest2> Logger { get; set; }

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

            //регистрирую логгер с неверной строкой подключения
            LoggerConfiguration.InitLoggerAdoNet("Data Source=G-DB-test-01;Initial Catalog=ClientNetwork;Persist Security Info=True;User ID=CNUser;Password=errorpass", "LogException");

        }

        [TestCleanup]
        public void Dispose()
        {
            _scope.Dispose();
        }

        [TestMethod]
        public void InnerErrorTest()
        {
            //должен записать в EventLog в соответствии с секцией <system.diagnostics>
            Logger.Error("Тестовая ошибка");
        }
    }
}
