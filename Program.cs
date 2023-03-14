using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using GodBot;
using GodBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Victoria;
using Victoria.Node;
using Victoria.WebSocket;

DotNetEnv.Env.Load();
var host = Host.CreateDefaultBuilder()
    .ConfigureDiscordHost((_, config) =>
    {
        config.SocketConfig = new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose,
            GatewayIntents = GatewayIntents.All,
            AlwaysDownloadUsers = true,
            MessageCacheSize = 1000,
            LogGatewayIntentWarnings = false,
        };

        config.Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? string.Empty;
    })
    .UseInteractionService((_, config) =>
    {
        config.LogLevel = LogSeverity.Info;
        config.UseCompiledLambda = true;
        config.DefaultRunMode = Discord.Interactions.RunMode.Async;
    })
    .ConfigureServices((_, services) =>
    {
        services.AddLavaNode(x =>
        {
            x.SelfDeaf = true;
            x.SocketConfiguration = new WebSocketConfiguration
            {
                BufferSize = UInt16.MaxValue
            };
        });
        services.AddSingleton<JokeService>();
        services.AddSingleton<MemeService>();
        services.AddSingleton<MusicService>();
        services.AddHostedService<InteractionHandler>();
        services.AddHostedService<BotStatusService>();
    }).Build();

await host.RunAsync();