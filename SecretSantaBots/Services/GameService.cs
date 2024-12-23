using SecretSantaBots.DataBase.CRUD;
using SecretSantaBots.DataBase.Models;
using System;
using System.Threading.Tasks;

namespace SecretSantaBots.Services
{
    /// <summary>
    /// Сервис для управления игрой в "Секретный Санта", включая создание, остановку, сброс игры и присоединение участников.
    /// </summary>
    public class GameService
    {
        private readonly GameCrudOperations _gameCrud;
        private readonly ParticipantCrudOperations _participantCrud;
        private readonly NotificationService _notificationService;
        private readonly PairingService _pairingService;
        private readonly AuthorizationService _authorizationService; // Сервис для проверки прав администратора

        /// <summary>
        /// Конструктор для инициализации сервисов, необходимых для управления игрой.
        /// </summary>
        /// <param name="gameCrud">Сервис для операций с играми в базе данных.</param>
        /// <param name="participantCrud">Сервис для операций с участниками игры в базе данных.</param>
        /// <param name="notificationService">Сервис для отправки уведомлений участникам игры.</param>
        /// <param name="pairingService">Сервис для формирования пар участников.</param>
        /// <param name="authorizationService">Сервис для проверки прав администратора.</param>
        public GameService(GameCrudOperations gameCrud, ParticipantCrudOperations participantCrud, NotificationService notificationService, PairingService pairingService, AuthorizationService authorizationService)
        {
            _gameCrud = gameCrud;
            _participantCrud = participantCrud;
            _notificationService = notificationService;
            _pairingService = pairingService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Начинает новую игру. Только администратор может начать игру.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором проводится игра.</param>
        /// <param name="userId">Идентификатор пользователя, который инициирует игру.</param>
        /// <param name="currency">Валюта для игры.</param>
        /// <param name="amount">Сумма для игры.</param>
        public async Task StartGame(long chatId, long userId, string currency, decimal amount)
        {
            // Проверка прав администратора
            if (!await _authorizationService.IsAdmin(userId))
            {
                await _notificationService.NotifyUnauthorized(chatId);
                return;
            }

            // Создание игры и уведомление участников
            await _gameCrud.Create(chatId, currency, amount);
            await _notificationService.NotifyGameStarted(chatId, currency, amount);
        }

        /// <summary>
        /// Останавливает игру. Только администратор может остановить игру.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <param name="userId">Идентификатор пользователя, который останавливает игру.</param>
        /// <param name="gameId">Идентификатор игры, которую нужно остановить.</param>
        public async Task StopGame(long chatId, long userId, Guid gameId)
        {
            // Проверка прав администратора
            if (!await _authorizationService.IsAdmin(userId))
            {
                await _notificationService.NotifyUnauthorized(chatId);
                return;
            }

            // Поиск игры и остановка
            var game = await _gameCrud.Read(gameId);
            if (game == null)
            {
                await _notificationService.NotifyGameNotFound(chatId);
                return;
            }

            await _pairingService.AssignPairs(gameId); // Назначение пар
            await _notificationService.NotifyGameStopped(game.ChatId);
        }

        /// <summary>
        /// Сбрасывает игру. Только администратор может сбросить игру.
        /// </summary>
        /// <param name="chatId">Идентификатор чата.</param>
        /// <param name="userId">Идентификатор пользователя, который сбрасывает игру.</param>
        /// <param name="gameId">Идентификатор игры, которую нужно сбросить.</param>
        public async Task ResetGame(long chatId, long userId, Guid gameId)
        {
            // Проверка прав администратора
            if (!await _authorizationService.IsAdmin(userId))
            {
                await _notificationService.NotifyUnauthorized(chatId);
                return;
            }

            // Поиск игры и сброс
            var game = await _gameCrud.Read(gameId);
            if (game == null)
            {
                await _notificationService.NotifyGameNotFound(chatId);
                return;
            }

            await _gameCrud.Delete(gameId); // Удаление игры
            await _notificationService.NotifyGameReset(chatId); // Уведомление об удалении игры
        }

        /// <summary>
        /// Присоединяет участника к игре.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором проводится игра.</param>
        /// <param name="userId">Идентификатор пользователя, который присоединяется к игре.</param>
        /// <param name="username">Имя пользователя, который присоединяется.</param>
        public async Task JoinGame(long chatId, long userId, string username)
        {
            var game = await _gameCrud.GetByChatId(chatId);
            if (game == null)
            {
                await _notificationService.NotifyGameStopped(chatId);
                return;
            }

            var participant = new Participant
            {
                Id = Guid.NewGuid(),
                GameId = game.Id,
                TelegramId = userId,
                Username = username,
                AssignedToId = Guid.Empty,
                Role = false // Участник без роли
            };

            await _participantCrud.Create(participant); // Добавление участника
            await _notificationService.NotifyParticipantJoined(chatId, username, game.Amount, game.Currency); // Уведомление о присоединении
        }
    }
}
