using System;
using Core.Demo.Domain;
using FluentValidation;

namespace Core.Demo.Validation
{
    public class TransExitNodeActionValidator : AbstractValidator<TransExitNodeActionEntity>
    {
        public TransExitNodeActionValidator()
        {
            RuleFor(item => item.Action).Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("Не заполнено значение поля action");

            RuleFor(item => item.HostName).Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("Не заполнено значение поля hostName");

            RuleFor(item => item.SessionId).Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("Не заполнено значение поля sessionId");

            RuleFor(item=>item.ProfileName).Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("Не заполнено значение поля profileName");

            RuleFor(item => item.VpnName).Must(value => !string.IsNullOrWhiteSpace(value)).WithMessage("Не заполнено значение поля vpnName");

            RuleFor(item => item.Networks).Must(value => value != null && value.Count > 0).WithMessage("Не заполнено значение поля network");

            RuleFor(item => item.Date).Must(value => value != default(DateTime)).WithMessage("Не удалось распознать формат поля date или оно незаполнено. Необходимый формат: \"dd.MM.yyyy hh:mm:ss\"");
        }
    }
}
