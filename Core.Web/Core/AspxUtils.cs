using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using BundlerMinifier;
using System.Diagnostics;

namespace Core.Web.Core
{
    /// <summary>
    /// Утилиты для в aspx-файлов
    /// </summary>
    public static class AspxUtils
    {
        private const string StylesheetTemplatePart = "STYLESHEET";
        private static readonly Regex SplitCssBrowserType = new Regex("IE[789]");
        private const string IeBrowserName = "IE";

        /// <summary>
        /// Создаём хэшированную ссылку
        /// </summary>
        private static string CreateHashedLink(string link, string template)
        {
            if (IsDebugConfiguration)
            {
                return string.Format(template, $"{link}") + Environment.NewLine;
            }

            var filePath = HttpContext.Current.Server.MapPath("/" + link);

            using (var md5 = MD5.Create())
            {
                var hash = new Guid(md5.ComputeHash(File.ReadAllBytes(filePath)));
                return string.Format(template, $"{link}?md5={hash.ToString("N").ToLower()}") + Environment.NewLine;
            }

        }

        /// <summary>
        /// Экспорт index.aspx в index.html
        /// </summary>
        public static void ExportToHtml()
        {
            if (HttpContext.Current.Request.Params[null] != "export")
                return;

            if (HttpContext.Current.Request["internalProcess_ExportToHtml"] == "1")
            {
                var response = HttpContext.Current.Response;
                response.StatusCode = 307;
                response.RedirectLocation = "index.html";
                response.Flush();
                return;
            }

            // Export index.aspx to index.html
            var htmlPath = HttpContext.Current.Server.MapPath("index.html");
            using (TextWriter tw = new StreamWriter(htmlPath))
            {
                HttpContext.Current.Server.Execute("index.aspx?export&internalProcess_ExportToHtml=1", tw);
            }
        }

        /// <summary>
        /// Преобразование файлов .bundle в набор файлов и обратно
        /// </summary>
        /// <param name="bundleFile"> Секция bundleconfig.json </param>
        /// <param name="template"> Шаблон тега(ов) в формате String.Format </param>
        /// <param name="singleFile">Режим одного файла или многих файлов </param>
        public static void BundleFiles(string bundleFile, string template, bool singleFile)
        {
            var server = HttpContext.Current.Server;
            var response = HttpContext.Current.Response;
            var bundleConfigFile = server.MapPath("/bundleconfig.json");

            // IE9 не переваривает стили с >4000 селекторов, поэтому отказываемся от бандла
            if (singleFile && template.ToUpper().Contains(StylesheetTemplatePart))
                singleFile = HttpContext.Current.Request.Browser.Browser != IeBrowserName || !SplitCssBrowserType.IsMatch(HttpContext.Current.Request.Browser.Type);

            if (singleFile)
            {
                response.Write(CreateHashedLink(bundleFile, template));
            }
            else
            {
                var boundles = BundleHandler.GetBundles(bundleConfigFile);
                boundles.FirstOrDefault(b => b.OutputFileName == bundleFile)?.InputFiles.ForEach(fileUrl =>
                {
                    var debugFile = fileUrl;

                    var unMinified = fileUrl.Replace(".min.", ".");
                    if (File.Exists(server.MapPath("/" + unMinified)))
                        debugFile = unMinified;

                    response.Write(CreateHashedLink(debugFile, template));
                });
            }
        }

        /// <summary>
        /// Добавляем хэш в виде параметра к адресу скрипта
        /// </summary>
        /// <param name="link">Путь к скрипту</param>
        public static void GetHashedLink(string link, string template)
        {
            HttpContext.Current.Response.Write(CreateHashedLink(link, template));
        }

        /// <summary>
        /// Сборка Debug / Release
        /// </summary>
        public static bool IsDebugConfiguration
        {
            get
            {
                if (Debugger.IsAttached)
                    return true;
                return false;
            }
        }
    }
}