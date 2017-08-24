using FluentValidation;
using FluentValidation.Results;

namespace Core.Validation
{
    public abstract class CustomAbstractValidator<T> : AbstractValidator<T>
    {
        private bool _isDefined = false;

        /// <summary>
        /// Здесь определяем правила валидации.
        /// </summary>
        public abstract void DefineValidation();

        public override ValidationResult Validate(ValidationContext<T> context)
        {
            if(!_isDefined)
            {
                DefineValidation();
                _isDefined = true;
            }

            return base.Validate(context);
        }

       
    }
}
