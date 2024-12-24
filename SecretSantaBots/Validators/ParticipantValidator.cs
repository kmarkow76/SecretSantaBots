using FluentValidation;
using SecretSantaBots.DataBase.Models;

namespace SecretSantaBots.Validators;

public class ParticipantValidator : AbstractValidator<Participant>
{
    public ParticipantValidator()
    {
        RuleFor(participant => participant.Id)
            .NotEmpty().WithMessage("Необходим идентификатор участника.");

        RuleFor(participant => participant.GameId)
            .NotEmpty().WithMessage("Необходим идентификатор игры.");

        RuleFor(participant => participant.TelegramId)
            .NotEmpty().WithMessage("Необходим идентификатор Telegram.")
            .GreaterThan(0).WithMessage("Идентификатор Telegram должен быть положительным числом.");

        RuleFor(participant => participant.Username)
            .NotEmpty().WithMessage("Необходимо указать имя пользователя.")
            .Length(3, 50).WithMessage("Имя пользователя должно быть от 3 до 50 символов.");

        RuleFor(participant => participant.AssignedToId)
            .Must((participant, assignedToId) => assignedToId != participant.Id)
            .WithMessage("Идентификатор AssignedTo не может совпадать с идентификатором участника.");

        RuleFor(participant => participant.Role)
            .Must(role => role == true || role == false)
            .WithMessage("Роль должна быть булевым значением.");
    }
}