using System.Collections.Generic;
using System.Linq;
using Core.Internal;

namespace Core.Validation
{
    /// <summary>
    /// DTO для передачи на клиент результата валидации сущности.
    /// </summary>
    public class ValidationFailureListDto
    {
        /// <summary>
        /// Список выявленных ошибок
        /// </summary>
        public IList<ValidationFailureDto> Violations { get; set; }

        /// <summary>
        /// Есть-ли ошибка
        /// </summary>
        public bool HasErrors()
        {
            return Violations != null && !Violations.IsEmpty() && Violations.FirstOrDefault(o => o.SeverityCode == (int)SeverityEnum.Error) != null;
        }

        /// <summary>
        /// Есть-ли значения
        /// </summary>
        public bool IsEmpty()
        {
            return Violations == null || Violations.IsEmpty();
        }

    }
}
