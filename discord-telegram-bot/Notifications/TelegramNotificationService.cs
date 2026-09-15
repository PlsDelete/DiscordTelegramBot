using DiscordTelegramBot.Telegram;

namespace DiscordTelegramBot.Notifications
{
    public class TelegramNotificationService : INotificationService
    {
        private readonly TelegramNotificationBot client;
        private readonly long chatId;
        private readonly int messageId;

        public TelegramNotificationService(TelegramNotificationBot client, long chatId, int messageId)
        {
            this.client = client;
            this.chatId = chatId;
            this.messageId = messageId;
        }
        public async Task NotificateAsync(string message)
        {
            await client.EditMessage(chatId, messageId, message);
        }
    }
}
