using SecretSantaBots.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecretSantaBots.DataBase;

namespace SecretSantaBots.Services
{
    /// <summary>
    /// Сервис для распределения участников игры "Секретный Санта" по парам.
    /// </summary>
    public class PairingService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Конструктор для инициализации сервиса с контекстом базы данных.
        /// </summary>
        /// <param name="context">Контекст базы данных для работы с участниками и играми.</param>
        public PairingService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Простой конструктор без параметров.
        /// </summary>
        public PairingService()
        {
        }

        /// <summary>
        /// Распределяет участников игры по парам.
        /// </summary>
        /// <param name="gameId">Идентификатор игры, для которой нужно распределить участников по парам.</param>
        /// <returns>Возвращает список участников с назначенными парами.</returns>
        /// <exception cref="InvalidOperationException">Бросает исключение, если количество участников нечётное.</exception>
        public async Task AssignPairs(Guid gameId)
        {
            // Извлекаем всех участников игры, которые ещё не имеют назначенной пары
            var participants = await _context.Participants
                .Where(p => p.GameId == gameId && !p.AssignedToId.HasValue) // Только не назначенные участники
                .ToListAsync();

            // Проверяем, что количество участников чётное
            if (participants.Count % 2 != 0)
            {
                throw new InvalidOperationException("Количество участников должно быть чётным для распределения пар.");
            }

            // Перемешиваем участников случайным образом
            var shuffledParticipants = participants.OrderBy(p => Guid.NewGuid()).ToList();

            // Назначаем пары
            for (int i = 0; i < shuffledParticipants.Count; i += 2)
            {
                var first = shuffledParticipants[i];
                var second = shuffledParticipants[i + 1];

                // Назначаем одного участника другому
                first.AssignedToId = second.Id;
                second.AssignedToId = first.Id;

                // Обновляем участников в базе данных
                _context.Participants.Update(first);
                _context.Participants.Update(second);
            }

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();
        }
    }
}
