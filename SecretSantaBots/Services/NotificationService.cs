using SecretSantaBots.DataBase.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SecretSantaBots.DataBase;
using SecretSantaBots.DataBase.CRUD;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretSantaBots.Services
{
    /// <summary>
    /// Сервис для отправки уведомлений участникам игры "Секретный Санта".
    /// </summary>
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITelegramBotClient _botClient;
        
        /// <summary>
        /// Конструктор для инициализации сервиса с клиентом Telegram бота.
        /// </summary>
        /// <param name="botClient">Клиент для взаимодействия с Telegram API.</param>
        public NotificationService(ApplicationDbContext context,ITelegramBotClient botClient)
        {
            _context = context;
            _botClient = botClient;
            
        }
        
        /// <summary>
        /// Простой конструктор без параметров.
        /// </summary>
        public NotificationService()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        public async Task OddNumberOfParticipants(long chatId)
        {
            await _botClient.SendTextMessageAsync(chatId, $"⛔ Для распределения пар число участников должно быть чётным \n" +
                                                          $"Найдите ещё друга и попробуйте снова");
            return;
        }
        /// <summary>
        /// Отправляет уведомление о недостаточных правах для выполнения команды.
        /// </summary>
        /// <param name="chatId">Идентификатор чата, в котором отправляется сообщение.</param>
        public async Task NotifyUnauthorized(long chatId)
        {
            await _botClient.SendTextMessageAsync(chatId, $"⛔ У вас нет прав для выполнения этой команды.");
            
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
            var message = "❌ Игра была сброшена. Все данные игры очищены.\n" +
                          "Ожидайте нового начала игры от администратора!";

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
        /// 
        /// </summary>
        /// <param name="gameId"></param>
        public async Task NotifyAllParticipantsAssigned(Guid gameId)
        {
            var participants = await _context.Participants
                .Where(p => p.GameId == gameId && p.AssignedToId.HasValue) // Проверка, что пара назначена
                .ToListAsync();

            foreach (var participant in participants)
            {
                await NotifyParticipantAssigned(participant); // Отправка уведомления
            }
           
        }
        /// <summary>
        /// Отправляет уведомление участнику о назначении ему пары.
        /// </summary>
        /// <param name="participant">Участник, которому назначена пара.</param>
        public async Task NotifyParticipantAssigned(Participant participant)
        {
            
            if (participant.AssignedToId == null)
            {
                // Участник не назначен, можно вывести сообщение для диагностики
                Console.WriteLine($"Участник {participant.Username} не имеет назначенной пары.");
                return;
            }

            // Загружаем данные о назначенной паре
            var assignedParticipant = await _context.Participants
                .Include(p => p.AssignedTo) // Убедимся, что загружаем назначенного участника
                .FirstOrDefaultAsync(p => p.Id == participant.AssignedToId);
            var assignedGame = await _context.Games.FirstOrDefaultAsync(g => g.Id == participant.GameId);
            if (assignedParticipant == null)
            {
                // Если не удалось найти назначенную пару
                Console.WriteLine($"Не удалось найти назначенную пару для участника {participant.Username}.");
                return;
            }


            var message = $"И так, \ud83c\udf85Санта решил по итогу \u2757\ufe0f\n" +
                          $"🎁 Вы подарите подарок \n" +
                          $"Пользователю @{assignedParticipant.Username}\n" +
                          $"Напоминаем на сумму в {assignedGame.Amount} {assignedGame.Currency}";

            // Отправка сообщения участнику
            try
            {
                await _botClient.SendTextMessageAsync(participant.TelegramId, message);
                Console.WriteLine($"Сообщение отправлено участнику {participant.Username}.");
            }
            catch (Exception ex)
            {
                // Логирование ошибки отправки сообщения
                Console.WriteLine($"Ошибка при отправке сообщения участнику {participant.Username}: {ex.Message}");
            }
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
            var message = $"✅ {username}, запустите бота на вашем устройстве в телеграмме, " +
                          $"и вы автоматически становитесь участником!🎁\n" +
                          $"Ссылка на бота: @secretsantasss_bot\nСумма подарка: {amount} {currency}";
            await _botClient.SendTextMessageAsync(chatId, message);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="username"></param>
        public async Task AlreadyAMember(long chatId, string username)
        {
            var message = $"{username} вы уже являетесь участником 🎁";
            
            await _botClient.SendTextMessageAsync(chatId, message);
        }
    }
}
