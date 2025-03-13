namespace EntityDal.Entity;

public class TelegramUser
{
    public long TelegramUserId { get; set; }
    public long ChatId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime? UpdateAt { get; set; }
}
