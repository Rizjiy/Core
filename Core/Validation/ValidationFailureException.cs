using System;

namespace Core.Validation
{
    public class ValidationFailureException: Exception
    {
        /// <summary>
        /// Список ошибок валидации
        /// </summary>
        public readonly ValidationFailureListDto FailureList;

        /// <summary>
        /// Конструктор, организующий передачу списка ошибок
        /// </summary>
        /// <param name="failureList"></param>
        public ValidationFailureException(ValidationFailureListDto failureList)
        {
            FailureList = failureList;
        }
    }
}
