using Telegram.Bot;

namespace DiscordTelegramBot.Telegram
{
    public class TelegramNotificationBot
    {
        private readonly TelegramBotClient client;

        public TelegramNotificationBot(TelegramBotClient client)
        {
            this.client = client;
        }

        public async Task EditMessage(long chatId, int messageId, string newText)
        {
            await client.EditMessageText(
                       chatId: chatId,
                       messageId: messageId,
                       text: newText
                    );
        }

        public async Task SendTextMessage(int chatId, string text) => await client.SendMessage(chatId: chatId, text: text);

        public async Task PrintUpdates()
        {
            var updates = await client.GetUpdates();

            //Группируем сообщения по ChatId и упорядочиваем сообщения от старых к новым
            var groupedChatsUpdates = updates
                .Where(u => u.Message != null)
                .GroupBy(u => u.Message.Chat.Id)
                .Select(g => new
                {
                    ChatId = g.Key,
                    Messages = g.OrderBy(u => u.Message.Date).ToList()
                });




            foreach (var chat in groupedChatsUpdates)
            {
                Console.WriteLine(new string('*', 30));
                Console.WriteLine($"Chat ID: {chat.ChatId} {chat.Messages.FirstOrDefault()?.Message?.From}");

                // перебираем все заполненные свойства Message и выводим их в консоль
                foreach (var messageUpdate in chat.Messages)
                {
                    Console.WriteLine(new string('-', 30));

                    messageUpdate.Message?.GetType().GetProperties()
                    .Where(p => p.GetValue(messageUpdate.Message) != null)
                    .ToList()
                    .ForEach(p =>
                    {
                        var value = p.GetValue(messageUpdate.Message);
                        Console.WriteLine($"{p.Name}: {value}");
                    });
                }
            }
        }

    }
}
