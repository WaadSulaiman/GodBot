using Discord.Interactions;
using GodBot.Services;

namespace GodBot.Modules;

public class MemeModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly MemeService _service;
    public MemeModule(MemeService service)
    {
        _service = service;
    }

    [SlashCommand("random-meme", "Get a random meme!")]
    public async Task GetRandomMemeAsync()
    {
        var meme = await _service.GetRandomMemeAsync();
        if (meme == null)
            await RespondAsync("What do you think I am? a meme generator?");
        else
            await RespondAsync(meme.Url);
    }
}