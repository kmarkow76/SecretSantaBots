using SecretSantaBots.DataBase.Models;
using SecretSantaBots.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecretSantaBots.DataBase;
using Xunit;
using Assert = Xunit.Assert;

namespace SecretSantaBots.Tests
{
    public class PairingServiceTests
    {
        private readonly PairingService _pairingService;
        private readonly ApplicationDbContext _context;

        public PairingServiceTests()
        {
            // Создаем сервис с InMemoryDatabase для тестов
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb") // Название базы данных для тестов
                .Options;

            _context = new ApplicationDbContext(options);
            _pairingService = new PairingService(_context);
        }

        /// <summary>
        /// Тестирование метода AssignPairs.
        /// Проверяется корректность распределения участников по парам.
        /// </summary>
        [Fact]
        public async Task AssignPairs_ShouldAssignPairs_WhenCalledWithEvenNumberOfParticipants()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            // Добавляем участников в базу данных (предположим, что метод AddParticipants существует)
            // _context.AddParticipants(gameId);

            // Act
            await _pairingService.AssignPairs(gameId);

            // Assert
            var participants = await _context.Participants.Where(p => p.GameId == gameId).ToListAsync();
            // Проверяем, что все участники имеют назначенную пару
            Assert.All(participants, p => Assert.NotNull(p.AssignedToId));
        }

        /// <summary>
        /// Тестирование метода AssignPairs.
        /// Проверяется выброс исключения, если количество участников нечетное.
        /// </summary>
        [Fact]
        public async Task AssignPairs_ShouldThrowException_WhenNumberOfParticipantsIsOdd()
        {
            // Arrange
            var gameId = Guid.NewGuid();
    
            // Добавляем нечетное количество участников с обязательным полем Username
            _context.Participants.Add(new Participant { GameId = gameId, Username = "user1" });
            _context.Participants.Add(new Participant { GameId = gameId, Username = "user2" });
            _context.Participants.Add(new Participant { GameId = gameId, Username = "user3" });
            await _context.SaveChangesAsync();  // Обязательно сохраняем изменения

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _pairingService.AssignPairs(gameId));
    
            // Assert: Проверяем, что исключение выброшено
            Assert.Equal("Количество участников должно быть чётным для распределения пар.", exception.Message);
        }

    }
}
