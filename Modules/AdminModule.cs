using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using RequireBotPermissionAttribute = Discord.Commands.RequireBotPermissionAttribute;
using RequireUserPermissionAttribute = Discord.Commands.RequireUserPermissionAttribute;

namespace GodBot.Modules;
public class AdminModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("kick", "Kick a user from the server.")]
    [RequireBotPermission(GuildPermission.KickMembers)]
    [RequireUserPermission(GuildPermission.KickMembers)]
    public async Task KickAsync(SocketGuildUser user, [Remainder] string reason = "No reason provided.")
    {
        await user.KickAsync(reason);
        await RespondAsync($"**{user}** has been kicked. Bye bye :wave:");
    }

    [SlashCommand("ban", "ban a user from the server.")]
    [RequireBotPermission(GuildPermission.BanMembers)]
    [RequireUserPermission(GuildPermission.BanMembers)]
    public async Task BanAsync(SocketGuildUser user, [Remainder] string reason = "No reason provided.")
    {
        await Context.Guild.AddBanAsync(user.Id, 0, reason);
        await RespondAsync($"**{user}** has been banned. Bye bye :wave:");
    }

    [SlashCommand("unban", "unban a user from the server.")]
    [RequireBotPermission(GuildPermission.BanMembers)]
    [RequireUserPermission(GuildPermission.BanMembers)]
    public async Task UnbanAsync(ulong user)
    {
        await Context.Guild.RemoveBanAsync(user);
        await Context.Channel.SendMessageAsync($"The user has been unbanned :clap:");
    }
}