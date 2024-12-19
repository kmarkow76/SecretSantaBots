using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSantaBots.DataBase.Models;
[Table("participant")]
public class Participant
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("gameId")]
    public Guid GameId { get; set; }
    [Column("telegramId")]
    public long TelegramId { get; set; }
    [Column("username")]
    public string Username { get; set; }
    [Column("assignedToId")]
    public Guid? AssignedToId { get; set; }
    [Column("role")]
    public bool Role { get; set; }
    // Навигационные свойства
    public Game Game { get; set; }
    public Participant AssignedTo { get; set; }
}