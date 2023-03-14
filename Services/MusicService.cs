using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Victoria.Node;
using Victoria.Node.EventArgs;
using Victoria.Player;

namespace GodBot.Services;
public sealed class MusicService
{
    public readonly LavaNode LavaNode;
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;

    public MusicService(LavaNode lavaNode, ILoggerFactory loggerFactory)
    {
        LavaNode = lavaNode;
        _logger = loggerFactory.CreateLogger<LavaNode>();
        _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();

        LavaNode.OnTrackEnd += OnTrackEndAsync;
        LavaNode.OnTrackStart += OnTrackStartAsync;
        LavaNode.OnWebSocketClosed += OnWebSocketClosedAsync;
        LavaNode.OnTrackStuck += OnTrackStuckAsync;
        LavaNode.OnTrackException += OnTrackExceptionAsync;
    }

    private async Task OnTrackExceptionAsync(TrackExceptionEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
    {
        arg.Player.Vueue.Enqueue(arg.Track);
        await arg.Player.TextChannel.SendMessageAsync($"`{arg.Track.Title}` has been requeued because it threw an exception.");
    }

    private async Task OnTrackStuckAsync(TrackStuckEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
    {
        arg.Player.Vueue.Enqueue(arg.Track);
        await arg.Player.TextChannel.SendMessageAsync($"`{arg.Track.Title}` has been requeued because it got stuck.");
    }

    private Task OnWebSocketClosedAsync(WebSocketClosedEventArg arg)
    {
        _logger.LogCritical($"{arg.Code} {arg.Reason}");
        return Task.CompletedTask;
    }
    private async Task OnTrackStartAsync(TrackStartEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
    {
        _logger.LogInformation($"Started playing {arg.Track}.");
        await arg.Player.TextChannel.SendMessageAsync($"Started playing `{arg.Track.Title}`.");
    }
    private async Task OnTrackEndAsync(TrackEndEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg)
    {
        if (arg.Reason is TrackEndReason.Finished)
        {
            await arg.Player.TextChannel.SendMessageAsync($"Finished playing `{arg.Track.Title}`.");
        }

        if (!arg.Player.Vueue.TryDequeue(out var track))
        {
            await arg.Player.TextChannel.SendMessageAsync("Queue completed! Please add more tracks to rock n' roll!");
            return;
        }

        if (track != null)
            await arg.Player.PlayAsync(track);
    }
}
