using Microsoft.EntityFrameworkCore;
using SecretSantaBots.DataBase.Models;

namespace SecretSantaBots.DataBase.CRUD;

public class ParticipantCrudOperations
{
    private ApplicationDbContext _context { get; set; }

    public ParticipantCrudOperations(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }
    /// <summary>
    /// Метод создания
    /// </summary>
    /// <param name="idGame">Id игры(не знаю откуда будет браться)</param>
    /// <param name="tgId">Telegramm Id</param>
    /// <param name="username">Имя пользователя</param>
    /// <param name="assignedId">Id пользователя, которому надо подойти.(Как мысль по демфолту это одно значение, допустим ноль,которое после меняется на какое-то другое)</param>
    /// <param name="role">Статус пользователя(администратор или пользователь)</param>
    public async Task Create(Participant participant)
    {
        await _context.Participants.AddAsync(participant);
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// Чтение
    /// </summary>
    /// <param name="id">Id пользователя, не знаю как передоваться</param>
    /// <param name="gameId">Id игры, не знаю как передаваться</param>
    /// <returns>Возвращает нужного пользователя</returns>
    public async Task<Participant> Read(Guid id,Guid gameId)
    {
        return await _context.Participants.Include(p => p.GameId == gameId).FirstOrDefaultAsync(p=>p.Id==id);
    }
    /// <summary>
    /// Обновление
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <param name="role">Роль(В будущем можно обдумать что можно поменять)</param>
    /// <returns>Возвращает и сохраняет обновленного пользователя</returns>
    public async Task<Participant> Update(Guid id,bool role)
    {
        var participant =await _context.Participants.FirstOrDefaultAsync(p=>p.Id==id);
        participant.Role = role;
        await _context.SaveChangesAsync();
        return participant;
    }
    /// <summary>
    /// Удаление
    /// </summary>
    /// <param name="id">Id пользователя</param>
    public async Task Delete(Guid id)
    {
        var participant = await _context.Participants.FirstOrDefaultAsync(p=>p.Id==id);
        _context.Participants.Remove(participant);
        await _context.SaveChangesAsync();
    }
    public async Task<Participant> GetByUserId(long userId)
    {
        return await _context.Participants.FirstOrDefaultAsync(p => p.TelegramId == userId);
    }
}