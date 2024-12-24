using Microsoft.AspNetCore.Mvc;
using SecretSantaBots.Services;

namespace SecretSantaBots.Controllers;
/// <summary>
/// Контроллер для управления действиями пользователей в игре "Секретный Санта".
/// </summary>
public class UserController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly AuthorizationService _authorizationService;
    private readonly NotificationService _notificationService;
    /// <summary>
    /// Конструктор контроллера для инициализации необходимых сервисов.
    /// </summary>
    /// <param name="gameService">Сервис для управления игрой.</param>
    /// <param name="authorizationService">Сервис для проверки прав администратора (хотя для пользователей он не нужен).</param>
    public UserController(GameService gameService, AuthorizationService authorizationService, NotificationService notificationService)
    {
        _gameService = gameService;
        _authorizationService = authorizationService;
        _notificationService = notificationService;
    }
    
    /// <summary>
    /// Позволяет пользователю присоединиться к игре.
    /// Если игра не активна, возвращается ошибка.
    /// </summary>
    /// <param name="chatId">Идентификатор чата, в котором проводится игра.</param>
    /// <param name="userId">Идентификатор пользователя, который пытается присоединиться.</param>
    /// <param name="username">Имя пользователя, который присоединяется к игре.</param>
    /// <returns>Ответ с подтверждением или ошибкой при попытке присоединиться.</returns>
    [HttpPost("join")]
    public async Task<IActionResult> JoinGame(long chatId, long userId, string username)
    {
        //Проверка на наличие активной игры
        var game = await _gameService.GetGameByChatId(chatId);
        if (game == null)
        {
            await _notificationService.NotifyGameNotFound(chatId);
            return null;
        }
        var isUserInGame = await _gameService.IsUserInGame(chatId, userId);
        if (isUserInGame)
        {
             await _notificationService.AlreadyAMember(chatId, username);
             return null;
        }
        //Присоединение пользователя к игре
        await _gameService.JoinGame(chatId, userId, username);
        return Ok("Вы учавствуете.");
    }
}