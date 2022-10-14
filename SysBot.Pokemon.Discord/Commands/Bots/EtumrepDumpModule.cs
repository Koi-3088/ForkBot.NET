using Discord;
using Discord.Commands;
using PKHeX.Core;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord.Commands
{
    public class EtumrepDumpModule<T> : ModuleBase<SocketCommandContext> where T : PKM, new()
    {
        private static TradeQueueInfo<T> Info => SysCord<T>.Runner.Hub.Queues.Info;

        [Command("etumrepDump")]
        [Alias("ed", "edump")]
        [Summary("Dumps the Pokémon you show via Link Trade, with the option to run EtumrepMMO and PermuteMMO.")]
        [RequireQueueRole(nameof(DiscordManager.RolesEtumrepDump))]
        public async Task EtumrepDumpAsync(int code)
        {
            var sig = Context.User.GetFavor();
            await QueueHelper<T>.AddToQueueAsync(Context, code, Context.User.Username, sig, new T(), PokeRoutineType.EtumrepDump, PokeTradeType.EtumrepDump).ConfigureAwait(false);
        }

        [Command("etumrepDump")]
        [Alias("ed", "edump")]
        [Summary("Dumps the Pokémon you show via Link Trade, with the option to run EtumrepMMO and PermuteMMO.")]
        [RequireQueueRole(nameof(DiscordManager.RolesEtumrepDump))]
        public async Task EtumrepDumpAsync([Summary("Trade Code")][Remainder] string code)
        {
            int tradeCode = Util.ToInt32(code);
            var sig = Context.User.GetFavor();
            await QueueHelper<T>.AddToQueueAsync(Context, tradeCode == 0 ? Info.GetRandomTradeCode() : tradeCode, Context.User.Username, sig, new T(), PokeRoutineType.EtumrepDump, PokeTradeType.EtumrepDump).ConfigureAwait(false);
        }

        [Command("etumrepDump")]
        [Alias("ed", "edump")]
        [Summary("Dumps the Pokémon you show via Link Trade, with the option to run EtumrepMMO and PermuteMMO.")]
        [RequireQueueRole(nameof(DiscordManager.RolesEtumrepDump))]
        public async Task EtumrepDumpAsync()
        {
            var code = Info.GetRandomTradeCode();
            await EtumrepDumpAsync(code).ConfigureAwait(false);
        }

        [Command("etumrepDumpList")]
        [Alias("edl", "edq")]
        [Summary("Prints the users in the Etumrep Dump queue.")]
        [RequireSudo]
        public async Task GetListAsync()
        {
            string msg = Info.GetTradeList(PokeRoutineType.EtumrepDump);
            var embed = new EmbedBuilder();
            embed.AddField(x =>
            {
                x.Name = "Pending Trades";
                x.Value = msg;
                x.IsInline = false;
            });
            await ReplyAsync("These are the users who are currently waiting:", embed: embed.Build()).ConfigureAwait(false);
        }

        [Command("etumrepTest")]
        [Alias("et", "etest")]
        [Summary("Dumps the Pokémon you show via Link Trade.")]
        [RequireQueueRole(nameof(DiscordManager.RolesDump))]
        public async Task TestEtumrep()
        {
            var b1 = System.IO.File.ReadAllBytes("test/test1.pa8");
            var b2 = System.IO.File.ReadAllBytes("test/test2.pa8");
            var b3 = System.IO.File.ReadAllBytes("test/test3.pa8");
            var b4 = System.IO.File.ReadAllBytes("test/test4.pa8");

            var pk1 = (PA8)EntityFormat.GetFromBytes(b1)!;
            var pk2 = (PA8)EntityFormat.GetFromBytes(b2)!;
            var pk3 = (PA8)EntityFormat.GetFromBytes(b3)!;
            var pk4 = (PA8)EntityFormat.GetFromBytes(b4)!;

            var b5 = System.IO.File.ReadAllBytes("test5/test1.pa8");
            var b6 = System.IO.File.ReadAllBytes("test5/test2.pa8");
            var b7 = System.IO.File.ReadAllBytes("test5/test3.pa8");
            var b8 = System.IO.File.ReadAllBytes("test5/test4.pa8");

            var pk5 = (PA8)EntityFormat.GetFromBytes(b5)!;
            var pk6 = (PA8)EntityFormat.GetFromBytes(b6)!;
            var pk7 = (PA8)EntityFormat.GetFromBytes(b7)!;
            var pk8 = (PA8)EntityFormat.GetFromBytes(b8)!;

            PA8[] pks = new[] { pk1, pk2, pk3, pk4 };
            PA8[] pks2 = new[] { pk5, pk6, pk7, pk8 };

            await EtumrepUtil.SendEtumrepEmbedAsync(Context.User, pks).ConfigureAwait(false);
            await EtumrepUtil.SendEtumrepEmbedAsync(Context.User, pks2).ConfigureAwait(false);
        }
    }
}
