using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSantaBots.DataBase.Models;
[Table("game")]
public class Game
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("chatId")]
    public long ChatId { get; set; }
    [Column("currency")]
    public string Currency { get; set; }
    [Column("amount")]
    public decimal Amount { get; set; }
    public ICollection<Participant> Participants { get; set; }
}