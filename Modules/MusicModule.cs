using Discord;
using Discord.Commands;
using Discord.Interactions;
using GodBot.Services;
using Victoria;
using Victoria.Player;
using Victoria.Responses.Search;

namespace GodBot.Modules;

public class MusicModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly MusicService _service;
    public MusicModule(MusicService service)
    {
        _service = service;
    }

    [SlashCommand("join", "I will join the vc and get ready to play some tunes for yall 🎶.")]
    public async Task JoinAsync()
    {
        if (_service.LavaNode.HasPlayer(Context.Guild))
        {
            await RespondAsync("I'm already connected to a voice channel!");
            return;
        }

        var voiceState = Context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await RespondAsync("You must be connected to a voice channel!");
            return;
        }

        try
        {
            await _service.LavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
            await RespondAsync($"Joined `{voiceState.VoiceChannel.Name}`!");
        }
        catch (Exception exception)
        {
            await RespondAsync(exception.Message);
        }
    }

    [SlashCommand("play", "I will play whatever you like babe 🤘🏻.")]
    public async Task PlayAsync([Remainder] string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            await RespondAsync("Please provide search terms.");
            return;
        }

        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await RespondAsync("You must be connected to a voice channel!");
                return;
            }

            try
            {
                player = await _service.LavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
            }
            catch (Exception exception)
            {
                await RespondAsync(exception.Message);
            }
        }

        var searchResponse = await _service.LavaNode.SearchAsync(Uri.TryCreate(searchQuery, UriKind.Absolute, out _) ? SearchType.Direct : SearchType.YouTube, searchQuery);
        if (searchResponse.Status is SearchStatus.LoadFailed or SearchStatus.NoMatches)
        {
            await RespondAsync($"I wasn't able to find anything for `{searchQuery}`.");
            return;
        }

        if (!string.IsNullOrWhiteSpace(searchResponse.Playlist.Name))
        {
            player.Vueue.Enqueue(searchResponse.Tracks);
            await RespondAsync($"Enqueued `{searchResponse.Tracks.Count}` songs.");
        }
        else
        {
            var track = searchResponse.Tracks.FirstOrDefault();
            player.Vueue.Enqueue(track);

            await RespondAsync($"Enqueued `{track?.Title}`.");
        }

        if (player.PlayerState is PlayerState.Playing or PlayerState.Paused)
        {
            return;
        }

        player.Vueue.TryDequeue(out var lavaTrack);
        await player.PlayAsync(lavaTrack);
    }

    [SlashCommand("leave", "I will leave the vc and take a walk 🚶.")]
    public async Task LeaveAsync()
    {
        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await RespondAsync("I'm not connected to any voice channels!");
            return;
        }

        var voiceChannel = ((IVoiceState)Context.User).VoiceChannel;
        if (voiceChannel == null)
        {
            await RespondAsync("Not sure which voice channel to disconnect from.");
            return;
        }

        try
        {
            await _service.LavaNode.LeaveAsync(voiceChannel);
            await RespondAsync($"I've left `{voiceChannel.Name}`!");
        }
        catch (Exception exception)
        {
            await RespondAsync(exception.Message);
        }
    }

    [SlashCommand("pause", "I will pause the music ヾ(⌐■_■)ノ♪.")]
    public async Task PauseAsync()
    {
        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await RespondAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await RespondAsync("I cannot pause when I'm not playing anything!");
            return;
        }

        try
        {
            await player.PauseAsync();
            await RespondAsync($"Paused `{player.Track.Title}`.");
        }
        catch (Exception exception)
        {
            await RespondAsync(exception.Message);
        }
    }

    [SlashCommand("resume", "I will resume the music ᴺᴼᵂ ᴾᴸᴬᵞᴵᴺᴳ♫♬♪.")]
    public async Task ResumeAsync()
    {
        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await RespondAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Paused)
        {
            await RespondAsync("I cannot resume when I'm not playing anything!");
            return;
        }

        try
        {
            await player.ResumeAsync();
            await RespondAsync($"Resumed `{player.Track.Title}`.");
        }
        catch (Exception exception)
        {
            await RespondAsync(exception.Message);
        }
    }

    [SlashCommand("skip", "I will skip the currently playing track ⏩.")]
    public async Task SkipAsync()
    {
        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await RespondAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await RespondAsync("Woaaah there, I can't skip when nothing is playing.");
            return;
        }

        try
        {
            var (oldTrack, currenTrack) = await player.SkipAsync();
            await RespondAsync($"Skipped `{oldTrack.Title}`.");
        }
        catch (Exception exception)
        {
            await RespondAsync(exception.Message);
        }
    }

    [SlashCommand("stop", "I will stop the music 🟥.")]
    public async Task StopAsync()
    {
        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await RespondAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState == PlayerState.Stopped)
        {
            await RespondAsync("Woaaah there, I can't stop the stopped forced.");
            return;
        }

        try
        {
            await player.StopAsync();
            await RespondAsync("No longer playing anything.");
        }
        catch (Exception exception)
        {
            await RespondAsync(exception.Message);
        }
    }

    [SlashCommand("seek", "I will seek the track to a desired position 🔙.")]
    public async Task SeekAsync(TimeSpan timeSpan)
    {
        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await RespondAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await RespondAsync("Woaaah there, I can't seek when nothing is playing.");
            return;
        }

        try
        {
            await player.SeekAsync(timeSpan);
            await RespondAsync($"I've seeked `{player.Track.Title}` to {timeSpan}.");
        }
        catch (Exception exception)
        {
            await RespondAsync(exception.Message);
        }
    }

    [SlashCommand("now-playing", "I will show you the currently playing track 💽.")]
    public async Task NowPlayingAsync()
    {
        if (!_service.LavaNode.TryGetPlayer(Context.Guild, out var player))
        {
            await RespondAsync("I'm not connected to a voice channel.");
            return;
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await RespondAsync("Woaaah there, I'm not playing any tracks.");
            return;
        }

        var track = player.Track;
        var artwork = await track.FetchArtworkAsync();

        var embed = new EmbedBuilder()
            .WithAuthor(track.Author, Context.Client.CurrentUser.GetAvatarUrl(), track.Url)
            .WithTitle($"Now Playing `{track.Title}`.")
            .WithImageUrl(artwork)
            .WithFooter($"Duration `{track.Duration}`.");

        await RespondAsync(embed: embed.Build());
    }
}