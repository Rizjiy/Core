using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utils.WebApi
{
    public static class WebApiExtensions
    {
        public static HttpResponseFile AttachmentResponse(this MemoryStream ms, string fileName)
        {
            HttpResponseFile httpResponseMessage = new HttpResponseFile();
            httpResponseMessage.Content = new StreamContent(ms);
            httpResponseMessage.Content.Headers.Add("x-filename", fileName);
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            httpResponseMessage.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            httpResponseMessage.Content.Headers.ContentDisposition.FileName = fileName;
            httpResponseMessage.StatusCode = HttpStatusCode.OK;
            return httpResponseMessage;

        }
    }

    public class HttpResponseFile : HttpResponseMessage, IHttpResponseFile
    {
    }
}
