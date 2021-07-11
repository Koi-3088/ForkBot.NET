using Discord.Commands;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord
{
    public class PingModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Makes the bot respond, indicating that it is running.")]
        public async Task PingAsync()
        {
            await ReplyAsync("Yes, I'm still alive. Thanks for checking up on me. Or would you rather me just reply with **pong!** That'd be boring and I'd like to give myself more credit than that. How about I just show you a kitty on a piggy? You're welcome.\n https://i.imgur.com/TxDolbn.gif").ConfigureAwait(false);
        }
    }
}