using DiscordTelegramBot.Configuration;
using DiscordTelegramBot.Discord;
using DiscordTelegramBot.Notifications;
using DiscordTelegramBot.Telegram;
using Microsoft.Extensions.Configuration;
using System.Net;
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
            var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

            _ = Task.Run(async () =>
            {
                var listener = new HttpListener();
                listener.Prefixes.Add($"http://+:{port}/");
                listener.Start();

                while (true)
                {
                    var ctx = await listener.GetContextAsync();
                    var buffer = "OK"u8.ToArray();
                    ctx.Response.StatusCode = 200;
                    ctx.Response.ContentLength64 = buffer.Length;
                    await ctx.Response.OutputStream.WriteAsync(buffer);
                    ctx.Response.Close();
                }
            });

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
