using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecretSantaBots.Controllers;
using SecretSantaBots.DataBase;
using SecretSantaBots.DataBase.CRUD;
using SecretSantaBots.DataBase.Models;
using SecretSantaBots.Services;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Polling;
using static Telegram.Bot.Types.Enums.UpdateType;

var builder = WebApplication.CreateBuilder(args);

//Получаем конфигурацию
var configuration = builder.Configuration;

// Получение конфигурации приложения
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
builder.Configuration.AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true);
Console.WriteLine("BasePath: " + Directory.GetCurrentDirectory());

// Настройка подключения к базе данных
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
Console.WriteLine("Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));

// Установка токена бота
var botToken = "7702920073:AAEeqAymi39cVMuxtFO40dNxk0u1dGmGt7w";
if (string.IsNullOrEmpty(botToken))
{
    Console.WriteLine("Ошибка: токен бота не найден. Убедитесь, что 'BotSettings:Token' задан в файле конфигурации.");
    return;
}
Console.WriteLine($"Токен успешно загружен: {botToken}");

// Регистрация сервисов для работы с ботом и бизнес-логики
builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<AuthorizationService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<PairingService>();
builder.Services.AddScoped<AdminController>();
builder.Services.AddScoped<UserController>();
builder.Services.AddScoped<GameCrudOperations>();
builder.Services.AddScoped<ParticipantCrudOperations>();

// Создание и настройка приложения
var app = builder.Build();
var botClient = app.Services.GetRequiredService<ITelegramBotClient>();

// Настройка токена отмены для асинхронных операций
var cancellationTokenSource = new CancellationTokenSource();
var cancellationToken = cancellationTokenSource.Token;

// Опции для приема обновлений от Telegram
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

// Обработчик входящих обновлений (сообщений)
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    using var scope = app.Services.CreateScope();
    var adminController = scope.ServiceProvider.GetRequiredService<AdminController>();
    var userController = scope.ServiceProvider.GetRequiredService<UserController>();
    var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();
    var gameService = scope.ServiceProvider.GetRequiredService<GameService>();
    var gameCrudOperations = scope.ServiceProvider.GetRequiredService<GameCrudOperations>();
    var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

    if (update.Message != null)
    {
        var message = update.Message;
        var chatId = message.Chat.Id;
        var userId = message.From.Id;
        var userName = message.From.Username;

        if (message.Text != null)
        {
            // Обработка команды /start
            if (message.Text.StartsWith("/start"))
            {
                await adminController.StartGame(chatId, userId, "РУБ", 200);
            }
            // Обработка команды /join
            else if (message.Text.StartsWith("/join"))
            {
                await userController.JoinGame(chatId, userId, userName);
            }
            // Обработка команды /stop
            else if (message.Text.StartsWith("/stop"))
            {
                var game = await gameCrudOperations.GetByChatId(chatId);
                if (game == null)
                {
                    if (!await authorizationService.IsAdmin(userId))
                    {
                        await notificationService.NotifyUnauthorized(chatId);
                    }
                    else
                    {
                        await notificationService.NotifyGameNotFound(chatId);
                    }
                }
                await adminController.StopGame(chatId, userId, game.Id);
            }
            // Обработка команды /reset
            else if (message.Text.StartsWith("/reset"))
            {
                var game = await gameCrudOperations.GetByChatId(chatId);
                if (game == null)
                {
                    if (!await authorizationService.IsAdmin(userId))
                    {
                        await notificationService.NotifyUnauthorized(chatId);
                    }
                    else
                    {
                        await notificationService.NotifyGameNotFound(chatId);
                    }
                }
                await adminController.ResetGame(chatId, userId, game.Id);
            }
            else
            {
                // Ответ на неизвестную команду
                await botClient.SendTextMessageAsync(message.Chat.Id, "Неизвестная команда \ud83d\ude15. Пожалуйста, попробуйте снова.");
            }
        }
    }
}

// Обработчик ошибок
async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.WriteLine($"Ошибка: {exception.Message} \u2757");
    // Логирование ошибки или отправка сообщения о проблемах
}

// Запуск получения обновлений от Telegram
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    errorHandler: HandleErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cancellationToken
);

Console.WriteLine("Бот запущен и готов принимать сообщения \ud83e\udd16");

// Запуск приложения
app.Run();
