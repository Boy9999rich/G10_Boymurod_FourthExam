
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using UserBll;

namespace TgBot_UserInfo;

public class TelegramBotListener
{
    private static string BotToken = "8080848231:AAEb_ugGCMpKZ9hb-Z8UCnmw4Rf-XynV9Qw";

    private long AdminID = 5979013794;

    private TelegramBotClient BotClient = new TelegramBotClient(BotToken);

    private List<long> adminChatsSender = new List<long>();

    private Dictionary<long, string> UserForUserInfo = new Dictionary<long, string>();

    private Dictionary<long, UserInfo> UserInfos = new Dictionary<long, UserInfo>();

    private readonly IUserService _userService;
    private readonly IUserInfoService _userInfoService;

    public TelegramBotListener(IUserInfoService userInfoService, IUserService userService)
    {
        _userInfoService = userInfoService;
        _userService = userService;
    }

    public string EscapeMarkdownV2(string text)
    {
        string[] specialChars = { "[", "]", "(", ")", ">", "#", "+", "-", "=", "|", "{", "}", ".", "!" };
        foreach (var ch in specialChars)
        {
            text = text.Replace(ch, "\\" + ch);
        }
        return text;
    }
    private bool ValidateFNameAndLName(string name)
    {
        foreach (var l in name)
        {
            if (!char.IsLetter(l) || l == ' ')
            {
                return false;
            }
        }
        return !string.IsNullOrEmpty(name) && name.Length <= 50;
    }
    private bool ValidatePhone(string phone)
    {
        foreach (var l in phone)
        {
            if (!char.IsDigit(l) || l == ' ')
            {
                return false;
            }
        }
        return phone.Length == 9;
    }
    private bool ValidateEmail(string email)
    {
        email.ToLower();

        return email.EndsWith("@gmail.com") && !string.IsNullOrEmpty(email) && email.Length <= 200 && email.Length > 10;
    }
    public async Task StartBot()
    {
        var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message, UpdateType.InlineQuery } };

        Console.WriteLine("Your bot is starting");

        BotClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions
            );

        Console.ReadKey();
    }



    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {

            var message = update.Message;
            var user = message.Chat;
            BotUser userObject = null;
            try
            {
                userObject = await _userService.GetUserByID(user.Id);
            }
            catch (Exception ex)
            {
            }

            Console.WriteLine($"{user.Id},  {user.FirstName}, {message.Text}");

            if (message.Text == "Informatsiya Kiritish")
            {
                if (userObject.UserInfo is null)
                {
                    try
                    {
                        UserInfos.Add(user.Id, new UserInfo());
                        UserForUserInfo.Add(user.Id, "Ism");
                    }
                    catch (Exception ex)
                    {
                        UserInfos.Remove(user.Id);
                        UserInfos.Add(user.Id, new UserInfo());

                        UserForUserInfo.Remove(user.Id);
                        UserForUserInfo.Add(user.Id, "Ism");
                    }

                    await bot.SendTextMessageAsync(user.Id, "Isminggizni Kiriting : ", cancellationToken: cancellationToken);
                }
                else if (userObject.UserInfo is not null)
                {
                    var userInformation = await _userInfoService.GetUserInfByID(userObject.BotUserId);//userID
                    var userInfo = $"~You Has Already Have Informations~\n\n*Ism* : _{userInformation.FirstNamee}_\n" +
                        $"*Familiya* : _{userInformation.LastNamee}_\n" +
                        $"*Email* : {userInformation.Email}\n" +
                        $"*PhoneNumber* : {userInformation.PhoneNumber}\n" +
                        $"*Adress* : `{userInformation.Address}`\n" +
                        $"*Summary* : *{userInformation.Summary}*";

                    await bot.SendTextMessageAsync(user.Id, EscapeMarkdownV2(userInfo), cancellationToken: cancellationToken, parseMode: ParseMode.MarkdownV2);
                    return;
                }
            }
            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Ism")
            {
                var validate = ValidateFNameAndLName(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Isminggizni To'g'ri Kiriting !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.FirstNamee = message.Text;
                var ch = info.FirstNamee[0];
                info.FirstNamee = info.FirstNamee.Remove(0, 1);
                info.FirstNamee = char.ToUpper(ch) + info.FirstNamee;
                UserForUserInfo[user.Id] = "Fam";
                await bot.SendTextMessageAsync(user.Id, "Familiyanggizni Kiriting : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Fam")
            {
                var validate = ValidateFNameAndLName(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Familiyanggizni To'g'ri Kiriting !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.LastNamee = message.Text;
                var ch = info.LastNamee[0];
                info.LastNamee = info.LastNamee.Remove(0, 1);
                info.LastNamee = char.ToUpper(ch) + info.LastNamee;
                UserForUserInfo[user.Id] = "Ema";
                await bot.SendTextMessageAsync(user.Id, "Emailinggizni Kiriting : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Ema")
            {
                var validate = ValidateEmail(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Emailni To'g'ri Kiriting !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.Email = message.Text;
                info.Email.ToLower();
                UserForUserInfo[user.Id] = "Pho";
                await bot.SendTextMessageAsync(user.Id, "Telefon raqamni kiriting (909009090 formatida):", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Pho")
            {
                var validate = ValidatePhone(message.Text);
                if (!validate)
                {
                    await bot.SendTextMessageAsync(user.Id, "Telefon Nomerni To'g'ri Kiriting !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.PhoneNumber = message.Text;
                info.PhoneNumber = "+998" + info.PhoneNumber;
                UserForUserInfo[user.Id] = "Adr";
                await bot.SendTextMessageAsync(user.Id, "Adresinggizni Kiriting : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Adr")
            {
                if (message.Text.Length > 200 && !string.IsNullOrEmpty(message.Text))
                {
                    await bot.SendTextMessageAsync(user.Id, "Adressni To'g'ri Kiriting !!!", cancellationToken: cancellationToken);
                    return;
                }
                var info = UserInfos[user.Id];
                info.Address = message.Text;
                UserForUserInfo[user.Id] = "Sum";
                await bot.SendTextMessageAsync(user.Id, "Summary Kiriting : ", cancellationToken: cancellationToken);
            }

            else if (UserForUserInfo.ContainsKey(user.Id) && UserForUserInfo[user.Id] == "Sum")
            {
                var info = UserInfos[user.Id];
                info.UserId = userObject.BotUserId;
                info.Summary = message.Text;

                await _userInfoService.AddUserInfo(info);

                UserInfos.Remove(user.Id);
                UserForUserInfo.Remove(user.Id);
                await bot.SendTextMessageAsync(user.Id, "User Info Saqlandi", cancellationToken: cancellationToken);
            }




            if (message.Text == "Informatsiyani Ko'rish")
            {
                UserInfo userInformation;
                try
                {
                    userInformation = await _userInfoService.GetUserInfByID(userObject.BotUserId);
                }
                catch (Exception ex)
                {
                    await bot.SendTextMessageAsync(user.Id, "User Info Topilmadi", cancellationToken: cancellationToken);
                    return;
                }

                var userInfo = $"*Ism* : _{userInformation.FirstNamee}_\n" +
                    $"*Familiya* : _{userInformation.LastNamee}_\n" +
                    $"*Email* : {userInformation.Email}\n" +
                    $"*PhoneNumber* : {userInformation.PhoneNumber}\n" +
                    $"*Adress* : `{userInformation.Address}`\n" +
                    $"*Summary* : *{userInformation.Summary}*";

                await bot.SendTextMessageAsync(user.Id, EscapeMarkdownV2(userInfo), cancellationToken: cancellationToken, parseMode: ParseMode.MarkdownV2);
            }

            if (message.Text == "All Send")
            {
                if (user.Id == AdminID)
                {
                    await bot.SendTextMessageAsync(user.Id, "So'zni Kiriting : ", cancellationToken: cancellationToken);
                    adminChatsSender.Add(AdminID);
                }
            }
            else if (adminChatsSender.Contains(AdminID))
            {
                var users = await _userService.GetAllUser();
                foreach (var u in users)
                {
                    await bot.SendTextMessageAsync(u.TelegramUserId, message.Text, cancellationToken: cancellationToken);
                }
                adminChatsSender.Remove(AdminID);
            }


            if (message.Text == "Informatsiyani O'Chirish")
            {
                var userInformation = await _userService.GetUserByID(user.Id);
                if (userInformation.UserInfo is null)
                {
                    await bot.SendTextMessageAsync(user.Id, "Informatsiyani O'chirish Uchun\nAvval Informatsiya Qo'shing", cancellationToken: cancellationToken);
                    return;
                }
                else
                {
                    await _userInfoService.DeleteUserInfo(userInformation.UserInfo.UserId);

                    await bot.SendTextMessageAsync(user.Id, "Informatsiya O'chirildi", cancellationToken: cancellationToken);
                }
            }

            if (message.Text == "/start")
            {

                if (userObject == null)
                {
                    userObject = new BotUser()
                    {
                        CreatedAt = DateTime.UtcNow,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        IsBlocked = false,
                        PhoneNumberr = null,
                        TelegramUserId = user.Id,
                        UpdatedAt = DateTime.UtcNow,
                        Username = user.Username
                    };


                    await _userService.AddUser(userObject);
                }
                else
                {
                    if (user.FirstName != userObject.FirstName || user.LastName != userObject.LastName || user.Username != userObject.Username)
                    {
                        userObject.UpdatedAt = DateTime.UtcNow;
                    }
                    ;
                    userObject.FirstName = user.FirstName;
                    userObject.LastName = user.LastName;
                    userObject.Username = user.Username;
                    await _userService.UpdateUser(userObject);
                }

                var keyboard = new ReplyKeyboardMarkup(new[]
            {
                    new[]
                    {
                        new KeyboardButton("Informatsiya Kiritish"),
                        new KeyboardButton("Informatsiyani Ko'rish"),
                    },
                    new[]
                    {
                        new KeyboardButton("Informatsiyani O'Chirish"),
                    },
                })
                { ResizeKeyboard = true };

                await bot.SendTextMessageAsync(user.Id, "Assalomu Alaykum 👋", replyMarkup: keyboard);
                return;
            }
        }
        else if (update.Type == UpdateType.CallbackQuery)
        {
            var id = update.CallbackQuery.From.Id;

            var text = update.CallbackQuery.Data;

            CallbackQuery res = update.CallbackQuery;
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
    }
}
