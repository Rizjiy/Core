using System.Net;
using System.Net.Http;
using System.Web.Http;
using FluentValidation;
using Core.Utils;
using FluentValidation.Results;

namespace Core.Validation
{
    public static class ValidationUtils
    {
        /// <summary>
        /// Выбрасывает ошибку по аналогию с FluentValidator'у.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        public static void ThrowCustomFailure(string errorMessage)
        {
            /* Если передать текст ошибки параметром Message в конструктур, то на клиент не сериализуется (сереализацией занимается GlobalExceptionHandler)
            Поэтому пришлось дополнительно обернуть в ValidationFailure. 
            upd. Альтернативным вариантом этому костылю может быть доработка GlobalExceptionHandler. Хотя  вряд ли удастся сильно упростить код.*/
            throw new ValidationException(new [] { new ValidationFailure(
                propertyName: "ThrowCustomFailure",
                errorMessage: errorMessage) });
        }

        
        /// <summary>
        /// Преобразователь ValidationFailureException для Api-контроллеров
        /// </summary>
        /// <param name="failure"> ValidationFailureException </param>
        /// <returns> HttpResponseException с кодом 409 </returns>
        public static HttpResponseException ToHttpResponseException(this ValidationException failure)
        {
            var response = new HttpResponseException(new HttpResponseMessage
            {
                Content = new StringContent(failure.Errors.ToJson()),
                StatusCode = HttpStatusCode.Conflict
            });

            return response;
        }
    }
}
