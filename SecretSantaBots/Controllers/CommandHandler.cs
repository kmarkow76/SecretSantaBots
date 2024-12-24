using Microsoft.AspNetCore.Mvc;
using SecretSantaBots.Services;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SecretSantaBots.Controllers;
/// <summary>
/// Обработчик команд, который принимает команды от пользователя и вызывает соответствующие методы контроллеров.
/// </summary>
public class CommandHandler
{
    private readonly AdminController _adminController;
    private readonly UserController _userController;
    private readonly AuthorizationService _authorizationService;
    /// <summary>
    /// Конструктор для инициализации обработчика команд с необходимыми зависимостями.
    /// </summary>
    /// <param name="adminController">Контроллер для администраторских команд.</param>
    /// <param name="userController">Контроллер для команд пользователей.</param>
    /// <param name="authorizationService">Сервис для проверки прав пользователя.</param>
    public CommandHandler(AdminController adminController, UserController userController, AuthorizationService authorizationService)
    {
        _adminController = adminController;
        _userController = userController;
        _authorizationService = authorizationService;
    }
    /// <summary>
    /// Обрабатывает команду от пользователя и выполняет соответствующее действие.
    /// </summary>
    /// <param name="chatId">Идентификатор чата, из которого поступила команда.</param>
    /// <param name="userId">Идентификатор пользователя, отправившего команду.</param>
    /// <param name="command">Текст команды, отправленной пользователем.</param>
    /// <param name="args">Дополнительные параметры команды.</param>
    /// <returns>Задача, которая выполняет обработку команды.</returns>>
    public async Task HandleCommandAsync(long chatId, long userId, string command, params string[] args)
    {
        switch (command.ToLower())
        {
            case "/start":
                if (args.Length < 2)
                {
                    //Неверный формат команды
                    await NotifyUnauthorized(chatId);
                    break;
                }

                var currency = args[0];
                var amount = decimal.TryParse(args[1], out decimal parsedAmount) ? parsedAmount : 0;
                await _adminController.StartGame(chatId, userId, currency, amount);
                break;

            case "/stop":
                if (args.Length < 1)
                {
                    await NotifyUnauthorized(chatId);
                    break;
                }

                var gameIdStop = Guid.Parse(args[0]);
                await _adminController.StopGame(chatId, userId, gameIdStop);
                break;

            case "/reset":
                if (args.Length < 1)
                {
                    await NotifyUnauthorized(chatId);
                    break;
                }

                var gameIdReset = Guid.Parse(args[0]);
                await _adminController.ResetGame(chatId, userId, gameIdReset);
                break;

            case "/join":
                if (args.Length < 1)
                {
                    await NotifyUnauthorized(chatId);
                    break;
                }

                var username = args[0];
                await _userController.JoinGame(chatId, userId, username);
                break;

            default:
                await NotifyUnauthorized(chatId);
                break;
        }
    }
    /// <summary>
    /// Отправляет уведомление о недостаточных правах для выполнения команды.
    /// </summary>
    /// <param name="chatId">Идентификатор чата, в который будет отправлено уведомление.</param>
    private async Task NotifyUnauthorized(long chatId)
    {
        // Здесь вы можете использовать NotificationService для отправки сообщения пользователю о недостаточных правах
        // Например:
        // await _notificationService.NotifyUnauthorized(chatId);
    }
}