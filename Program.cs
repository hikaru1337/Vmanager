using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using DarlingDb;
using Microsoft.EntityFrameworkCore;
using VManager.db.Model;
using System.Diagnostics;

using (var mContext = new Db())
{
    mContext.Database.Migrate();
    if(!mContext.Generate.Any())
    mContext.Generate.Add(new Generate { Id = 1, img = new byte[0] });
    await mContext.SaveChangesAsync();
    mContext.Dispose();
}

var botClient = new TelegramBotClient("TELEGRAM:TOKEN");
var updates = await botClient.GetUpdatesAsync(null, null, 0);
using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>(), // receive all update types except ChatMember related updates
    ThrowPendingUpdates = true,
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);



var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username} Vmanager]");
Console.ReadLine();


cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    Console.WriteLine($"Received a '{update.Message?.Text}' message in chat {update.Message?.Chat.Id}.");
    var handler = update switch
    {
        { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
    };

    await handler;

}



async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
{
    using (var db = new Db())
    {
        if (message.Text is not { } messageText)
            return;

        if (!db.Vtuber.Any(X => X.TelegramId == message.From.Id))
            return;

        string[] messageParts = messageText.Split(' ');
        string Command = messageParts[0];
        string Parametr1 = string.Join(" ", messageParts.Skip(1));
        string Parametr2 = string.Join(" ", messageParts.Skip(2));

        var action = Command switch
        {
            "/me" => GetMe(botClient, message, cancellationToken),
            "/generate" => GeneratePhoto(botClient, message, cancellationToken),
            "/offline" => iAmOffline(botClient, message, cancellationToken),
            "/edit" => EditEvent(botClient, message, Convert.ToInt32(Parametr1), Parametr2, cancellationToken),
            "/remove" => DelEvent(botClient, message, Convert.ToInt32(Parametr1), cancellationToken),
            "/add" => AddEvent(botClient, message, Parametr1, cancellationToken),
        }; ;
        Message sentMessage = await action;

        if (Command != "/me" && Command != "/generate")
            await GetMe(botClient, message, cancellationToken);
    }

}

static async Task<Message> GeneratePhoto(TelegramBotClient botClient, Message message, CancellationToken cancellationToken)
{
    using (var db = new Db())
    {
        var User = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.TelegramId == message.From.Id);

        if (User is not null)
        {
            var process = Process.Start("ImageGenerator.exe", $"-all");
            process.WaitForExit();
            await Task.Delay(500);
            var gen = db.Generate.FirstOrDefault();
            await using Stream stream = new MemoryStream(gen.img);
            var zxc = InputFile.FromStream(stream, "Result.png");

            await botClient.SendPhotoAsync(
            chatId: message.Chat.Id,
            photo: zxc,
                caption: $"Вот расписание размером меньше.",
                cancellationToken: cancellationToken);

            return await botClient.SendDocumentAsync(
                chatId: message.Chat.Id,
                document: zxc,
                cancellationToken: cancellationToken);
        }
        return null;
    }
}

static async Task<Message> iAmOffline(TelegramBotClient botClient, Message message, CancellationToken cancellationToken)
{
    using (var db = new Db())
    {
        var User = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.TelegramId == message.From.Id);

        if (User is not null)
        {
            var OfflineDate = new Dates { Date_Description = "Offline", Time = DateTime.Now, VtuberId = User.Id };
            db.Dates.Add(OfflineDate);
            await db.SaveChangesAsync();

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Вы успешно отключили стримы на неделю.",
                cancellationToken: cancellationToken);
        }
        return null;
    }
}

static async Task<Message> EditEvent(TelegramBotClient botClient, Message message, int parametr1, string parametr2, CancellationToken cancellationToken)
{
    using (var db = new Db())
    {
        var User = db.Vtuber.Include(x => x.Dates).FirstOrDefault(x => x.TelegramId == message.From.Id);

        if (User is not null)
        {
            var Date = User.Dates.ElementAt(parametr1 - 1);
            string oldDate = Date.Date_Description;
            bool important = false;
            if (parametr2.Contains("[+]"))
            {
                parametr2.Replace("[+]", "");
                important = true;
            }
            Date.Date_Description = parametr2;
            Date.Important = important;
            db.Dates.Update(Date);
            await db.SaveChangesAsync();

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Вы успешно изменили стрим.\n\nДо: {oldDate}\nПосле: {Date.Date_Description}\nВажность: {(important ? "Важное" : "Простое")}",
                cancellationToken: cancellationToken);
        }
        return null;
    }
}

static async Task<Message> DelEvent(TelegramBotClient botClient, Message message, int parametr1, CancellationToken cancellationToken)
{
    using (var db = new Db())
    {
        var User = db.Vtuber.FirstOrDefault(x => x.TelegramId == message.From.Id);

        if (User is not null)
        {
            var Date = db.Dates.ElementAt(parametr1 - 1);
            db.Dates.Remove(Date);
            await db.SaveChangesAsync();

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Вы успешно удалили стрим.\n\n• {Date.Date_Description}",
                cancellationToken: cancellationToken);
        }
        return null;
    }
}

static async Task<Message> AddEvent(TelegramBotClient botClient, Message message, string Parametr1, CancellationToken cancellationToken)
{
    using (var db = new Db())
    {
        var User = db.Vtuber.FirstOrDefault(x => x.TelegramId == message.From.Id);

        if (User is not null)
        {
            bool important = false;
            if(Parametr1.Contains("[+]"))
            {
                Parametr1.Replace("[+]", "");
                important = true;
            }    
            var DateAdd = new Dates { Time = DateTime.Now, Date_Description = Parametr1, VtuberId = User.Id, Important = important };
            db.Dates.Add(DateAdd);
            await db.SaveChangesAsync();

            return await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: $"Вы успешно добавили стрим\n\n• {Parametr1}\nВажность: {(important ? "Важное" : "Простое")}",
                cancellationToken: cancellationToken);
        }
        return null;
    }
}

static async Task<Message> GetMe(TelegramBotClient botClient, Message message, CancellationToken cancellationToken)
{
    using (var db = new Db())
    {
        var User = db.Vtuber.FirstOrDefault(x => x.TelegramId == message.From.Id);

        if (User is not null)
        {
            string command = "/add [text] - добавить событие\n" +
                             "/edit [number] [text] - изменить событие\n" +
                             "/remove [number] - удалить событие\n" +
                             "/me - профиль\n" +
                             "/offline - не актив\n" +
                             "/generate - сгенерировать фото\n\n" +
                             "Чтобы пометить стрим цветом, добавьте в конце текста: [+]";

            var startDate = new DateTime();
            var endDate = new DateTime();
            CalculateData(ref startDate, ref endDate);

            var process = Process.Start("ImageGenerator.exe", $"-only {User.Name}");
            process.WaitForExit();
            await Task.Delay(500);
            await using (Stream stream = new MemoryStream(User.Image))
            {
                return await botClient.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: InputFile.FromStream(stream, "Result.png"),
                    caption: $"Привет {User.Name}. Вот твое расписание с {startDate.ToString("dd.MM")} по {endDate.ToString("dd.MM")} \n\n{command}",
                    cancellationToken: cancellationToken);
            }
            
        }
        return null;
    }
}

static void CalculateData(ref DateTime startDateOfWeek, ref DateTime endDateOfWeek)
{
    DateTime currentDate = DateTime.Now;
    startDateOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday).AddDays(7);
    endDateOfWeek = startDateOfWeek.AddDays(6);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}