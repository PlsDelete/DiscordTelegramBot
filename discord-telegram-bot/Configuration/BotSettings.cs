namespace DiscordTelegramBot.Configuration;

public class BotSettings
{
    public DiscordSettings Discord { get; set; } = new();
    public TelegramSettings Telegram { get; set; } = new();
}

public class DiscordSettings
{
    public string Token { get; set; } = string.Empty;
}

public class TelegramSettings
{
    public string Token { get; set; } = string.Empty;
    public long ChatId { get; set; }
    public int MessageId { get; set; }
}