using Moq;
using Microsoft.EntityFrameworkCore;
using SecretSantaBots.DataBase;
using SecretSantaBots.DataBase.Models;
using SecretSantaBots.DataBase.CRUD;
using SecretSantaBots.Services;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;
using Assert = Xunit.Assert;

/// <summary>
/// Класс для тестирования сервиса GameService.
/// </summary>
public class GameServiceTests
{
    private readonly GameService _gameService;
    private readonly ApplicationDbContext _context;
    private readonly GameCrudOperations _gameCrud;
    private readonly ParticipantCrudOperations _participantCrud;
    private readonly Mock<ITelegramBotClient> _mockBotClient;
    private readonly Mock<NotificationService> _mockNotificationService;
    private readonly Mock<AuthorizationService> _mockAuthorizationService;
    private readonly PairingService _pairingService;

    /// <summary>
    /// Конструктор для инициализации всех зависимостей и моков перед выполнением тестов.
    /// </summary>
    public GameServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _context = new ApplicationDbContext(options);
        _gameCrud = new GameCrudOperations(_context);
        _participantCrud = new ParticipantCrudOperations(_context);

        // Мокаем TelegramBotClient
        _mockBotClient = new Mock<ITelegramBotClient>();

        // Мокаем AuthorizationService для метода IsAdmin
        _mockAuthorizationService = new Mock<AuthorizationService>();
        _mockAuthorizationService.Setup(service => service.IsAdmin(It.IsAny<long>())).ReturnsAsync(true); // Возвращаем true для админа

        // Мокаем NotificationService
        _mockNotificationService = new Mock<NotificationService>(_mockBotClient.Object);

        // Инициализируем PairingService (можно замокать, если необходимо)
        _pairingService = new PairingService(_context);

        // Используем мокированные сервисы в GameService
        _gameService = new GameService(
            _gameCrud, 
            _participantCrud, 
            _mockNotificationService.Object, 
            _pairingService, 
            _mockAuthorizationService.Object
        );
    }

    /// <summary>
    /// Тест, проверяющий, что игра запускается успешно для админа.
    /// </summary>
    /// <returns>Тест на успешный запуск игры для админа.</returns>
    [Fact]
    public async Task StartGame_ValidAdmin_ShouldStartGame()
    {
        // Arrange
        long chatId = 123456;
        long userId = 789012; // Админ
        string currency = "USD";
        decimal amount = 100m;

        // Act
        await _gameService.StartGame(chatId, userId, currency, amount);

        // Assert
        var game = await _gameCrud.GetByChatId(chatId);
        Assert.NotNull(game);
        Assert.Equal(currency, game.Currency);
        Assert.Equal(amount, game.Amount);
    }

    /// <summary>
    /// Тест, проверяющий, что неавторизованный пользователь получает уведомление о невозможности начать игру.
    /// </summary>
    /// <returns>Тест на уведомление для неавторизованного пользователя.</returns>
    [Fact]
    public async Task StartGame_UnauthorizedUser_ShouldNotifyUnauthorized()
    {
        // Arrange
        long chatId = 123456;
        long userId = 111111; // Не админ
        string currency = "USD";
        decimal amount = 100m;

        // Act
        await _gameService.StartGame(chatId, userId, currency, amount);

        // Assert
        // Проверка, что метод NotifyUnauthorized был вызван для неадминистратора
        _mockNotificationService.Verify(service => service.NotifyUnauthorized(chatId), Times.Once);
    }

    /// <summary>
    /// Тест, проверяющий, что участник может присоединиться к игре.
    /// </summary>
    /// <returns>Тест на добавление участника в игру.</returns>
    [Fact]
    public async Task JoinGame_ValidParticipant_ShouldJoinGame()
    {
        // Arrange
        long chatId = 123456;
        long userId = 789012;
        string username = "TestUser";

        // Создаем игру для теста
        await _gameCrud.Create(chatId, "USD", 100m);

        // Act
        await _gameService.JoinGame(chatId, userId, username);

        // Assert
        var participant = await _participantCrud.GetByUserId(userId);
        Assert.NotNull(participant);
        Assert.Equal(username, participant.Username);
    }
}

