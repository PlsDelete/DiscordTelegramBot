using DiscordTelegramBot.Configuration;
using DiscordTelegramBot.Discord;
using DiscordTelegramBot.Notifications;
using DiscordTelegramBot.Telegram;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace DiscordTelegramBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            RunBot().GetAwaiter().GetResult();
        }

        static async Task RunBot()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            var settings = configuration.Get<BotSettings>() 
                ?? throw new InvalidOperationException("BotSettings not configured.");

            var telegramBot = new TelegramBotClient(settings.Telegram.Token);
            var telegramNotifyBot = new TelegramNotificationBot(telegramBot);
            var telegramNotificationService = new TelegramNotificationService(telegramNotifyBot, settings.Telegram.ChatId, settings.Telegram.MessageId);

            var consoleNotificationService = new ConsoleNotificationService();

            var discordBot = new DiscordBot(consoleNotificationService,telegramNotificationService);
            await discordBot.StartAsync(settings.Discord.Token);

            await Task.Delay(Timeout.Infinite);
        }
    }
}
