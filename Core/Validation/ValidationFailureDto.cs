namespace Core.Validation
{
    /// <summary>
    /// выявленная ошибка валидации сущности
    /// </summary>
    public class ValidationFailureDto
    {
        /// <summary>
        /// сообщение для отображения пользователю
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// свойство на которое сработала валидация
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// уровень критичности в виде строки (возможные варианты описаны в SeverityEnum: Error, Warning, Info)
        /// </summary>
        public string Severity { get; set; }
        /// <summary>
        /// уровень критичности в виде числа (возможные варианты описаны в SeverityEnum: Error = 1, Warning = 2, Info = 3)
        /// </summary>
        public int SeverityCode { get; set; }
    }
}
