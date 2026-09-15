namespace DiscordTelegramBot.Notifications;

public interface INotificationService
{
    Task NotificateAsync(string message);
}