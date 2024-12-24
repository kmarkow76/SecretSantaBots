using Microsoft.EntityFrameworkCore;
using SecretSantaBots.DataBase.Models;
using SecretSantaBots.Services;

namespace SecretSantaBots.DataBase.CRUD;

public class GameCrudOperations
{
    private ApplicationDbContext _context { get; set; }
    private readonly NotificationService _notificationService;
    /// <summary>
    /// Предоставляет операции CRUD для управления играми в приложении.
    /// </summary>
    /// <param name="applicationDbContext">Контекст базы данных, используемый для работы с данными игр.</param>
    /// <param name="notificationService">Сервис уведомлений, используемый для отправки уведомлений, связанных с операциями с играми.</param>
    public GameCrudOperations(ApplicationDbContext applicationDbContext, NotificationService notificationService)
    {
        _context = applicationDbContext;
        _notificationService = notificationService;
    }
    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="GameCrudOperations"/> с указанным контекстом базы данных.
    /// Этот конструктор не реализован и выбрасывает исключение <see cref="NotImplementedException"/>
    /// </summary>
    /// <param name="applicationDbContext">Контекст базы данных, используемый для работы с данными игр.</param>
    /// <exception cref="NotImplementedException">Выбрасывается, так как данный конструктор пока не реализован.</exception>
    public GameCrudOperations(ApplicationDbContext applicationDbContext)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Метод создания и добавления игры в базу данных
    /// </summary>
    /// <param name="chatId">Id чата где запускается игра</param>
    /// <param name="currency">Наименование валюты</param>
    /// <param name="amount">Стоимость подарка</param>
    public async Task Create(long chatId,string currency,decimal amount)
    {
        await _notificationService.NotifyGameStarted(chatId, currency, amount);
        var game = new Game()
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            Currency = currency,
            Amount = amount
        };
        await _context.Games.AddAsync(game);
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Метод чтения
    /// </summary>
    /// <param name="id">Id игры?(не знаю откуда будет браться,надо продумать если этот метод будет нужен. Надо обдумать!!!!!)</param>
    /// <returns>Возвращает найденную игру</returns>
    public async Task<Game> Read(Guid id)
    {
        return await _context.Games.Include(g => g.Participants).FirstOrDefaultAsync(g => g.Id == id);
        
    }
    /// <summary>
    /// Метод обновления 
    /// </summary>
    /// <param name="id">Id игры.(вопрос передачи id тот же)</param>
    /// <param name="currency">Валюта, параметр который спокойно можно поменять</param>
    /// <param name="amount">Стоимость также параметр, который можно спокойно изменить</param>
    /// <returns>Возвращение новой игры</returns>
    public async Task<Game> Update(Guid id,string currency,decimal amount)
    {
        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
        game.Currency = currency;
        game.Amount = amount;
        await _context.SaveChangesAsync();
        return game;
    }
    /// <summary>
    /// Удаление игры или /reset (Надо обдумать)
    /// </summary>
    /// <param name="id">Id игры</param>
    public async Task Delete(Guid id)
    {
        var game = await _context.Games
            .Include(g => g.Participants) // Подключаем участников игры
            .FirstOrDefaultAsync(g => g.Id == id);
        if (game == null)
        {
            // Если игра не найдена, можем уведомить, что игра не существует
            throw new InvalidOperationException("Игра не найдена.");
        }
        // Удаляем всех участников игры
        _context.Participants.RemoveRange(game.Participants);
        _context.Games.Remove(game);
        // Сохраняем изменения в базе данных
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="chatId"></param>
    /// <returns></returns>
    public async Task<Game?> GetByChatId(long chatId)
    {
        try
        {
            var game = await _context.Games
                .Include(g => g.Participants)
                .FirstOrDefaultAsync(g => g.ChatId == chatId);

            if (game == null)
            {
                // Логируем отсутствие игры
                Console.WriteLine($"Игра с ChatId {chatId} не найдена.");
            }

            return game;
        }
        catch (Exception ex)
        {
            // Логируем исключение
            Console.WriteLine($"Ошибка при получении игры с ChatId {chatId}: {ex.Message}");
            throw;
        }
    }

}