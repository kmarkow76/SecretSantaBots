using Microsoft.AspNetCore.Mvc;
using SecretSantaBots.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using static Telegram.Bot.Types.Enums.UpdateType;
using Message = Telegram.Bot.Types.Message;

namespace SecretSantaBots.Controllers;
[ApiController]
[Route("api/[controller]")]
/// <summary>
/// Контроллер для упраления действиями администратора в игре "Секретный Санта".
/// </summary>
public class AdminController : ControllerBase
{
    private readonly GameService _gameService;
    private readonly AuthorizationService _authorizationService;
    private readonly NotificationService _notificationService;
    
    /// <summary>
    /// Конструктор контроллера для инициализации необходимых сервисов.
    /// </summary>
    /// <param name="gameService">Сервис для управления игрой.</param>
    /// <param name="authorizationService">Сервис для проверки прав администратора.</param>
    public AdminController(GameService gameService, AuthorizationService authorizationService, NotificationService notificationService
    )
    {
        _gameService = gameService;
        _authorizationService = authorizationService;
        _notificationService = notificationService;
       
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Работает! ✅");
    }
    /// <summary>
    /// Запускает игру с указанием валюты и суммы подарка.
    /// Доступно только для администраторов.
    /// </summary>
    /// <param name="chatId">Идентификатор чата, в котором будет проходить игра.</param>
    /// <param name="userId">Идентификатор пользователя, который пытается начать игру (должен быть администратором)</param>
    /// <param name="currency">Валюта для игры.</param>
    /// <param name="amount">Сумма подарка для участников игры.</param>
    /// <returns>Ответ с подтверждением о запуске игры или ошибке.</returns>
    [HttpPost("start")]
    public async Task<IActionResult> StartGame(long chatId, long userId, string currency, decimal amount)
    {
        
        //Проверка для администратора
        if (!await _authorizationService.IsAdmin(userId))
        {
            await _notificationService.NotifyUnauthorized(chatId);
            return null;
        }
        //Запуск игры
        await _gameService.StartGame(chatId, userId, currency, amount);
        
        return Ok("Игра стартовала успешно.");
    }
    
    /// <summary>
    /// Останавливает игру, распределяет участников по парам и уведомляет их.
    /// Доступно только для администраторов.
    /// </summary>
    /// <param name="chatId">Идентификатор чата, в котором проводится игра.</param>
    /// <param name="userId">Идентификатор пользователя, который пытается остановить игру (должен быть администратором)</param>
    /// <param name="gameId">Идентификатор игры, которую необходимо остановить.</param>
    /// <returns>Ответ с подтверждением о завершении игры и распределении участников по парам.</returns>
    [HttpPost("stop")]
    public async Task<IActionResult> StopGame(long chatId, long userId, Guid gameId)
    {
        //Проверка прав администратора
        if (!await _authorizationService.IsAdmin(userId))
        {
            await _notificationService.NotifyUnauthorized(chatId);
            return null;
        }
        //Остановка игры и распределение игроков по парам
        await _gameService.StopGame(chatId, userId, gameId);
        return Ok("Игра остановлена и назначены пары.");
    }

    /// <summary>
    /// Сбрасывает игру, очищая все данные и позволяя начать новую.
    /// Доступно только для администраторов.
    /// </summary>
    /// <param name="chatId">Идентификатор чата, в котором проводится игра.</param>
    /// <param name="userId">Идентификатор пользователя, который пытается сбросить игру (должен быть администратором)</param>
    /// <param name="gameId">Идентификатор игры, которую необходимо сбросить.</param>
    /// <returns>Ответ с подтверждением о сбросе игры.</returns>
    [HttpPost("reset")]
    public async Task<IActionResult> ResetGame(long chatId, long userId, Guid gameId)
    {
        //Проверка для администратора
        if (!await _authorizationService.IsAdmin(userId))
        {
            await _notificationService.NotifyUnauthorized(chatId);
            return null;
        }
        //Сброс игры
        await _gameService.ResetGame(chatId, userId, gameId);
        return Ok("Игра была сброшена.");
    }
    
    
}