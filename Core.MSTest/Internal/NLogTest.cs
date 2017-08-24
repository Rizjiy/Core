using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LightInject;
using NLog;
using NLog.Config;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Core.Log;
using Core.Internal.Dependency;
using System.IO;
using System.Linq;

namespace Core.MSTest.Internal
{
    [TestClass]
    public class NLogTest
    {

        public Logger<NLogTest> logger { get; set; }

        [TestInitialize]
        public void Init()
        {
            LogManager.Configuration = new XmlLoggingConfiguration(GetTestDataPath(@"NLog.config"));

            new DependencyInitializer()
            .TestMode(true)
            .ForAssembly(GetType().Assembly)
            .Init((dbConfig, container) =>
            {
                //логгер работает вне скопа
                //_scope = container.BeginScope();

                container.InjectProperties(this);
            });

        }

        [TestCleanup]
        public void Dispose()
        {
           // _scope.Dispose();
        }

        [TestMethod]
        public void SimpleNLogTest()
        {
            logger.Error("Test nLog Error", new Exception("Тестовая ошибка"));
            logger.Fatal("Test nLog Fatal", new Exception("Тестовая ошибка"));
            logger.Debug("Test nLog Debug", new Exception("Тестовая ошибка"));
            logger.Info("Test nLog Info");
        }

        [TestMethod]
        public void NLog400_Multithread()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Parallel.For(1, 5, (j) =>
            {
                var i = 100;
                while (i > 0)
                {
                    Thread.Sleep(10);
                    logger.Info($"NLog400_{j}_{i}");
                    i--;
                }
            });

            Debug.WriteLine($"прошло милисекунд: {sw.ElapsedMilliseconds}");

        }

        /// <summary>
        /// Возвращает абсолютный путь
        /// </summary>
        /// <param name="filePath">Относительный путь от папки проекта</param>
        /// <returns></returns>
        public static string GetTestDataPath(string filePath)
        {
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split(Path.DirectorySeparatorChar);
            string projectPath = String.Join(Path.DirectorySeparatorChar.ToString(), pathItems.Take(pathItems.Length - 2));
            return Path.Combine(projectPath, filePath);
        }

    }
}
