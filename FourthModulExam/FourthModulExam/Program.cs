
using EntityDal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using UserBll;

namespace TgBot_UserInfo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IUserInfoService, UserInfoService>();
            serviceCollection.AddSingleton<MainContext>();
            serviceCollection.AddSingleton<TelegramBotListener>();


            var serviceProvider = serviceCollection.BuildServiceProvider();

            var botListenerService = serviceProvider.GetRequiredService<TelegramBotListener>();
            await botListenerService.StartBot();

            Console.ReadKey();
        }
    }
}