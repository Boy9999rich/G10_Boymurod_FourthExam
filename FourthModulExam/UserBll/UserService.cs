using EntityDal.Entity;
using EntityDal;
using Microsoft.EntityFrameworkCore;
namespace UserBll;

public class UserService : IUserService
{
    private readonly MainContext _mainContext;

    public UserService(MainContext mainContext)
    {
        _mainContext = mainContext;
    }

    public async Task AddUserAsync(TelegramUser User)
    {
        var dbUser = await _mainContext.Users.FirstOrDefaultAsync(b => b.TelegramUserId == User.TelegramUserId);
        if (dbUser != null)
        {
            Console.WriteLine($"This user Id {User.TelegramUserId} already exists!");
        }

        try
        {
            await _mainContext.Users.AddAsync(User);
            await _mainContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<TelegramUser>> GetAllUserAsync()
    {
        var users = await _mainContext.Users.ToListAsync();
        return users;
    }

    public async Task UpdateUserAsync(TelegramUser User)
    {
        var dbUser = await _mainContext.Users.FirstOrDefaultAsync(b => b.TelegramUserId == User.TelegramUserId);
        if (dbUser is null)
        {
            Console.WriteLine($"This user Id {User.TelegramUserId} not found");
            return;
        }

        dbUser.FirstName = User.FirstName;
        dbUser.LastName = User.LastName;
        dbUser.Email = User.Email;
        dbUser.PhoneNumber = User.PhoneNumber;

        _mainContext.Users.Update(dbUser);
        await _mainContext.SaveChangesAsync();
    }
}
