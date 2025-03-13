using EntityDal.Entity;

namespace UserBll;

public interface IUserService
{
    Task AddUserAsync(TelegramUser User);
    Task UpdateUserAsync(TelegramUser User);
    Task<List<TelegramUser>> GetAllUserAsync();
}
