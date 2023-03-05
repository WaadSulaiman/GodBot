using Discord;
using Discord.Addons.Hosting;
using Discord.WebSocket;
using GodBot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
    .UseInteractionService((context, config) =>
    {
        config.LogLevel = LogSeverity.Info;
        config.UseCompiledLambda = true;
    })
    .ConfigureServices((context, services) =>
    {
        //Add any other services here
        services.AddHostedService<InteractionHandler>();
    }).Build();

await host.RunAsync();