using Discord;
using Discord.WebSocket;
using DiscordTelegramBot.Notifications;

namespace DiscordTelegramBot.Discord;

public class DiscordBot
{
    private readonly DiscordSocketClient _client;
    private readonly INotificationService[] _notificationServices;

    public DiscordBot(params INotificationService[] notificationServices)
    {
        _notificationServices = notificationServices;

        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
                            | GatewayIntents.GuildMembers
                            | GatewayIntents.GuildVoiceStates
        };

        _client = new DiscordSocketClient(config);

        _client.Log += LogAsync;
        _client.UserVoiceStateUpdated += UserVoiceStateUpdated;
    }

    public async Task StartAsync(string token)
    {
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    private async Task UserVoiceStateUpdated(
        SocketUser socketUser,
        SocketVoiceState before,
        SocketVoiceState after)
    {
        var guild = _client.Guilds.FirstOrDefault();

        if (guild == null)
        {
            return;
        }

        var voiceChannels = guild.VoiceChannels
            .Where(x => x.Users.Count > 0)
            .ToList();

        var users = voiceChannels.SelectMany(x => x.ConnectedUsers).ToList();

        var emoji = users.Count > 0 ? "🟢" : "🔴";

        var message = $"{emoji} Сейчас в DS: {users.Count}\n";

        foreach (var channel in voiceChannels)
        {
            if (channel.ConnectedUsers.Count == 0) continue;
            message += $"\n🔊 {channel.Name}:\n";

            foreach (var user in channel.ConnectedUsers)
            {
                message += $"• {user.Username}\n";
            }
        }

        foreach (var service in _notificationServices)
        {
            await service.NotificateAsync(message);
        }
    }

    private Task LogAsync(LogMessage message)
    {
        Console.WriteLine(message);

        return Task.CompletedTask;
    }
}