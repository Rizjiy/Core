using NConsoler;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using ServiceGenConsole.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGenConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Consolery.Run(typeof(Program), args);
        }

        [Action]
        public static void Multiple(
        [Required(Description = "Имя Angular модуля")]
        string appModule,
        [Required(Description = "Путь до библиотеки")]
        string dllPath,
        [Required(Description = "Сгенерированный файл сервисов")]
        string outputFile
            )
        {
            //пишем в дебаг параметрах: "DemoModule" "Core.Web.dll" "Services.js"
            //var stream = typeof(Program).Assembly.GetManifestResourceStream("ServiceGenConsole.Templates.NoModel.cshtml");
            //appModule = "DemoModule";
            //dllName = "Core.Web.dll";
            //outputFile = @"c:\Projects\TestT4Template\TestT4Template\Content\Services\Services.js";

            //Подключаем библиотеку
            Assembly.LoadFrom(dllPath);

            //Получаем файл для записи
            FileInfo file = new FileInfo(outputFile);

            var gen = new AngularWebApiMethodGenerator();
            var model = new AppDomainModel
            {
                AppModule = appModule,
                Data = gen.GetData()
            };

            var rootType = typeof(Program);
            var template = "Templates.ServiceTemplate";

            var config = new TemplateServiceConfiguration
            {
                TemplateManager = new EmbeddedResourceTemplateManager(rootType),

                //http://antaris.github.io/RazorEngine/#Temporary-files
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            IRazorEngineService service = RazorEngineService.Create(config);

            string result = service.RunCompile(template, model: model);

            using (var sw = new StreamWriter(file.FullName, false, System.Text.Encoding.UTF8))
            {
                sw.Write(result);
            }
        }
    }
}
