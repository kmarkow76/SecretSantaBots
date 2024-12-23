using SecretSantaBots.DataBase.Models;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SecretSantaBots.Services
{
    /// <summary>
    /// Сервис для отправки уведомлений участникам игры "Секретный Санта".
    /// </summary>
    public class NotificationService
    {
        private readonly ITelegramBotClient _botClient;

        /// <summary>
        /// Конструктор для инициализации сервиса с клиентом Telegram бота.
        /// </summary>
        /// <param name="botClient">Клиент для взаимодействия с Telegram API.</param>
        public NotificationService(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        /// <summary>
        /// Простой конструктор без параметров.
        /// </summary>
        public NotificationService()
        {
        }

        /// <summary>
        /// Отправляет уведомление о недостаточных правах для выполнения команды.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором отправляется сообщение.</param>
        public async Task NotifyUnauthorized(long chatId)
        {
            await _botClient.SendTextMessageAsync(chatId, "⛔ У вас нет прав для выполнения этой команды.");
        }

        /// <summary>
        /// Отправляет уведомление о начале игры и открытии регистрации.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором отправляется сообщение.</param>
        /// <param name="currency">Валюта игры.</param>
        /// <param name="amount">Сумма подарка.</param>
        public async Task NotifyGameStarted(long chatId, string currency, decimal amount)
        {
            var message = $"🚨 Игра началась! 🚨\n\n" +
                          $"Регистрация открыта. Пожалуйста, присоединяйтесь!\n\n" +
                          $"💰 Валюта: {currency}\n" +
                          $"🎁 Сумма подарка: {amount}";

            await _botClient.SendTextMessageAsync(chatId, message);
        }

        /// <summary>
        /// Отправляет уведомление о завершении регистрации и распределении участников по парам.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором отправляется сообщение.</param>
        public async Task NotifyGameStopped(long chatId)
        {
            var message = "🏁 Регистрация завершена! Теперь участники распределены по парам. Вы получите личное сообщение с информацией о вашем подарке.";

            await _botClient.SendTextMessageAsync(chatId, message);
        }

        /// <summary>
        /// Отправляет уведомление о сбросе игры и удалении всех данных.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором отправляется сообщение.</param>
        public async Task NotifyGameReset(long chatId)
        {
            var message = "❌ Игра была сброшена. Все данные игры очищены.";

            await _botClient.SendTextMessageAsync(chatId, message);
        }

        /// <summary>
        /// Отправляет уведомление о том, что игра не найдена.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором отправляется сообщение.</param>
        public async Task NotifyGameNotFound(long chatId)
        {
            await _botClient.SendTextMessageAsync(chatId, "❌ Игра не найдена.");
        }

        /// <summary>
        /// Отправляет уведомление участнику о назначении ему пары.
        /// </summary>
        /// <param name="participant">Участник, которому назначена пара.</param>
        public async Task NotifyParticipantAssigned(Participant participant)
        {
            if (participant.AssignedToId == null)
                return;

            var assignedParticipant = participant.AssignedTo;
            var message = $"🎁 Вы подарите подарок пользователю: {assignedParticipant.Username}\n" +
                          $"Пользователь @{assignedParticipant.Username} с Telegram ID: {assignedParticipant.TelegramId}";

            await _botClient.SendTextMessageAsync(participant.TelegramId, message);
        }

        /// <summary>
        /// Отправляет уведомление о том, что участник успешно зарегистрирован в игре.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором отправляется сообщение.</param>
        /// <param name="username">Имя участника, который присоединился.</param>
        /// <param name="amount">Сумма подарка.</param>
        /// <param name="currency">Валюта игры.</param>
        public async Task NotifyParticipantJoined(long chatId, string username, decimal amount, string currency)
        {
            var message = $"✅ {username}, вы успешно зарегистрированы! 🎁\nСумма подарка: {amount} {currency}";
            await _botClient.SendTextMessageAsync(chatId, message);
        }
    }
}
