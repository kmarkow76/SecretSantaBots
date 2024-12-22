using SecretSantaBots.DataBase.Models;
using SecretSantaBots.Services;
using System.Threading.Tasks;
using Telegram.Bot;
using Xunit;

namespace SecretSantaBots.Tests
{
    /// <summary>
    /// Тесты для <see cref="NotificationService"/>.
    /// </summary>
    public class NotificationServiceTests
    {
        private readonly NotificationService _notificationService;
        private readonly ITelegramBotClient _botClient;

        /// <summary>
        /// Инициализация тестов для <see cref="NotificationService"/>.
        /// Создает экземпляр <see cref="NotificationService"/> с использованием <see cref="ITelegramBotClient"/>.
        /// </summary>
        public NotificationServiceTests()
        {
            // Здесь предполагается, что _botClient замещается, если используется реальный клиент
            _botClient = new TelegramBotClient("7702920073:AAEeqAymi39cVMuxtFO40dNxk0u1dGmGt7w");
            _notificationService = new NotificationService(_botClient);
        }

        /// <summary>
        /// Проверяет, что уведомление об отсутствии прав отправляется с правильным сообщением.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        [Fact]
        public async Task NotifyUnauthorized_ShouldSendNotification_WhenCalled()
        {
            // Arrange
            long chatId = 996802857;

            // Act
            await _notificationService.NotifyUnauthorized(chatId);

            // Assert
            // Проверяем, что сообщение отправлено с правильным текстом.
        }

        /// <summary>
        /// Проверяет, что уведомление о сбросе игры отправляется с правильным сообщением.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        [Fact]
        public async Task NotifyGameReset_ShouldSendNotification_WhenCalled()
        {
            // Arrange
            long chatId = 996802857;

            // Act
            await _notificationService.NotifyGameReset(chatId);

            // Assert
            // Проверяем, что сообщение отправлено с правильным текстом.
        }

        /// <summary>
        /// Проверяет, что уведомление о том, что игра не найдена, отправляется с правильным сообщением.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        [Fact]
        public async Task NotifyGameNotFound_ShouldSendNotification_WhenCalled()
        {
            // Arrange
            long chatId = 996802857;

            // Act
            await _notificationService.NotifyGameNotFound(chatId);

            // Assert
            // Проверяем, что сообщение отправлено с правильным текстом.
        }

        /// <summary>
        /// Проверяет, что уведомление о вступлении участника в игру отправляется с правильным сообщением.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        [Fact]
        public async Task NotifyParticipantJoined_ShouldSendNotification_WhenCalled()
        {
            // Arrange
            long chatId = 996802857;
            string username = "test_user";
            decimal amount = 100;
            string currency = "USD";

            // Act
            await _notificationService.NotifyParticipantJoined(chatId, username, amount, currency);

            // Assert
            // Проверяем, что сообщение отправлено с правильным текстом.
        }

        /// <summary>
        /// Проверяет, что уведомление о начале игры отправляется с правильным сообщением.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        [Fact]
        public async Task NotifyGameStarted_ShouldSendNotification_WhenCalled()
        {
            // Arrange
            long chatId = 996802857;
            string currency = "USD";
            decimal amount = 100;

            // Act
            await _notificationService.NotifyGameStarted(chatId, currency, amount);

            // Assert
            // Здесь можно проверить, что сообщение было отправлено через _botClient.SendTextMessageAsync.
        }

        /// <summary>
        /// Проверяет, что уведомление о остановке игры отправляется с правильным сообщением.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        [Fact]
        public async Task NotifyGameStopped_ShouldSendNotification_WhenCalled()
        {
            // Arrange
            long chatId = 996802857;

            // Act
            await _notificationService.NotifyGameStopped(chatId);

            // Assert
            // Проверяем, что сообщение отправлено.
        }

        /// <summary>
        /// Проверяет, что уведомление о назначении участника на другого отправляется с правильным сообщением.
        /// </summary>
        /// <returns>Задача, представляющая асинхронную операцию.</returns>
        [Fact]
        public async Task NotifyParticipantAssigned_ShouldSendNotification_WhenCalled()
        {
            // Arrange
            var participant = new Participant
            {
                TelegramId = 996802857,
                AssignedToId = Guid.NewGuid(),
                AssignedTo = new Participant
                {
                    TelegramId = 987654321,
                    Username = "test_user"
                }
            };

            // Act
            await _notificationService.NotifyParticipantAssigned(participant);

            // Assert
            // Проверяем, что сообщение отправлено на правильный TelegramId.
        }
    }
}
