using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace GodBot.Services;
public class BotStatusService : DiscordClientService
{
    public BotStatusService(DiscordSocketClient client, ILogger<DiscordClientService> logger) : base(client, logger)
    {
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Client.WaitForReadyAsync(stoppingToken);
        await Client.SetGameAsync("╭∩╮( •̀_•́ )╭∩╮", "https://www.youtube.com/watch?v=xvFZjo5PgG0&ab_channel=Duran");
    }
}