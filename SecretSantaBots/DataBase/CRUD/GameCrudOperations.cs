using Microsoft.EntityFrameworkCore;
using SecretSantaBots.DataBase.Models;

namespace SecretSantaBots.DataBase.CRUD;

public class GameCrudOperations
{
    private ApplicationDbContext _context { get; set; }

    public GameCrudOperations(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }
    /// <summary>
    /// Метод создания и добавления игры в базу данных
    /// </summary>
    /// <param name="chatId">Id чата где запускается игра</param>
    /// <param name="currency">Наименование валюты</param>
    /// <param name="amount">Стоимость подарка</param>
    public async Task Create(long chatId,string currency,decimal amount)
    {
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
        var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
        _context.Games.Remove(game);
    }
}