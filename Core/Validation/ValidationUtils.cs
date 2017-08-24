using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.Internal;
using FluentValidation;

namespace Core.Validation
{
    public static class ValidationUtils
    {


        /// <summary>
        /// явно указать критичность ошибки (если метод не вызван по умолчанию будет Error)
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <typeparam name="TProperty">тип свойства</typeparam>
        /// <param name="rule">правило</param>
        /// <param name="severity">уровень критичности</param>
        public static IRuleBuilderOptions<T, TProperty> WithSeverity<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule,
            SeverityEnum severity)
        {
            return rule.WithState(t => severity);
        }

        /// <summary>
        /// явно указать критичность ошибки (если метод не вызван по умолчанию будет Error)
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <typeparam name="TProperty">тип свойства</typeparam>
        /// <param name="rule">правило</param>
        /// <param name="predicate">выражение для Severity</param>
        public static IRuleBuilderOptions<T, TProperty> WithSeverity<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule,
            Func<SeverityEnum> predicate)
        {
            return rule.WithState(t => predicate());
        }

        /// <summary>
        /// Mетод запускает валидацию для переданной сущности и формирует результат в виде ValidationFailureListDto для передачи на клиент
        /// </summary>
        /// <typeparam name="T">тип сущности</typeparam>
        /// <param name="validator">используемый валидатор</param>
        /// <param name="instance">экземпляр типа сущности</param>
        /// <param name="ruleSet">набор правил (RuleSet)</param>
        /// <returns></returns>
        public static ValidationFailureListDto ValidateAndTransformToDto<T>(this AbstractValidator<T> validator, T instance, string ruleSet = null)
        {
            //валидируем сущность
            var validationResult = validator.Validate(instance, ruleSet: ruleSet);

            //трасформируем результат валидации в DTO для передачи на клиент
            var resultDto = new ValidationFailureListDto
            {
                Violations = validationResult.Errors.Select(failure => new ValidationFailureDto
                {
                    Message = failure.ErrorMessage,
                    PropertyName = failure.PropertyName,
                    SeverityCode = failure.CustomState != null
                        ? (int)failure.CustomState.To(SeverityEnum.Error)
                        : (int)SeverityEnum.Error,
                    Severity = failure.CustomState != null
                        ? failure.CustomState.To(SeverityEnum.Error).ToString()
                        : SeverityEnum.Error.ToString()
                }).ToList()
            };

            return resultDto;
        }

        /// <summary>
        /// Выбрасывает ошибку по аналогию с FluentValidator'у.
        /// </summary>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        public static void ThrowCustomFailure(string errorMessage)
        {
            var resultDto = new ValidationFailureListDto
            {
                Violations = new List<ValidationFailureDto>(new ValidationFailureDto[] { new ValidationFailureDto
                {
                    Message = errorMessage,
                    SeverityCode = (int)SeverityEnum.Error
                } })
            };
            throw new ValidationFailureException(resultDto);
        }

        /// <summary>
        /// Mетод запускает валидацию для переданной сущности, формирует результат в виде ValidationFailureListDto и,
        /// в случае наличия ошибок - выбрасывает ValidationFailureException для перехвата и передачи на клиент
        /// </summary>
        /// <typeparam name="T"> Тип сущности валидатора </typeparam>
        /// <param name="validator"> Валидатор, к котормоу применяется расширение </param>
        /// <param name="instance"> Экземпляр сущности </param>
        /// <param name="ruleSet"> Набор правил </param>
        public static void ValidateAndThrow<T>(this AbstractValidator<T> validator, T instance, string ruleSet = null)
        {
            // Валидирую сущность и трасформирую результат валидации в DTO для передачи на клиент
            var resultDto = validator.ValidateAndTransformToDto(instance, ruleSet);
            if (!resultDto.IsEmpty())
                throw new ValidationFailureException(resultDto);
        }

        /// <summary>
        /// Преобразователь ValidationFailureException для Api-контроллеров
        /// </summary>
        /// <param name="failure"> ValidationFailureException </param>
        /// <returns> HttpResponseException с кодом 409 </returns>
        public static HttpResponseException ToHttpResponseException(this ValidationFailureException failure)
        {
            var response = new HttpResponseException(new HttpResponseMessage
            {
                Content = new StringContent(failure.FailureList.ToJson()),
                StatusCode = HttpStatusCode.Conflict
            });

            return response;
        }
    }
}
