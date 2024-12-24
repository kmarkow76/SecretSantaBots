using FluentValidation;
using SecretSantaBots.DataBase.Models;

namespace SecretSantaBots.Validators;

public class GameValidator : AbstractValidator<Game>
{
    public GameValidator()
    {
        RuleFor(game => game.Id)
            .NotEmpty().WithMessage("Необходим идентификатор игры.");

        RuleFor(game => game.ChatId)
            .NotEmpty().WithMessage("Необходим идентификатор чата.")
            .GreaterThan(0).WithMessage("Идентификатор чата должен быть положительным числом.");

        RuleFor(game => game.Currency)
            .NotEmpty().WithMessage("Необходима валюта.")
            .Length(3, 10).WithMessage("Валюта должна быть от 3 до 10 символов.");

        RuleFor(game => game.Amount)
            .GreaterThan(0).WithMessage("Сумма должна быть больше 0.");
    }
}