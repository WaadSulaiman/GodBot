using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using GodBot;
using GodBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Victoria;
using Victoria.Node;

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
        services.AddLavaNode(x => x.SelfDeaf = true);
        services.AddSingleton<JokeService>();
        services.AddSingleton<MemeService>();
        services.AddSingleton<MusicService>();
        services.AddSingleton<LavaNode>();
        services.AddHostedService<InteractionHandler>();
    }).Build();

await host.RunAsync();