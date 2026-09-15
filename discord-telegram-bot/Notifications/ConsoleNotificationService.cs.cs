namespace DiscordTelegramBot.Notifications;

public class ConsoleNotificationService : INotificationService
{
    public Task NotificateAsync(string message)
    {
        Console.WriteLine(message);

        return Task.CompletedTask;
    }
}