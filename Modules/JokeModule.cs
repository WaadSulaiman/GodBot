using Discord.Interactions;
using GodBot.Models;
using GodBot.Services;

namespace GodBot.Modules;

public class JokeModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly JokeService _service;
    public JokeModule(JokeService service)
    {
        _service = service;
    }

    [SlashCommand("random-joke", "Get a random joke!")]
    public async Task GetRandomJokeAsync()
    {
        var joke = await _service.GetRandomJokeAsync();
        if (joke == null)
            await RespondAsync("Sorry, at the moment there seems to be a shortage of jokes!");
        else
            await RespondAsync(joke.Content);
    }
}