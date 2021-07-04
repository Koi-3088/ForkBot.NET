using Discord;
using Discord.Rest;
using Discord.Commands;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SysBot.Pokemon.Discord
{
    public class ExtraCommandUtil
    {
        public async Task ListUtil(SocketCommandContext ctx, string nameMsg, string entry)
        {
            List<string> pageContent = TradeExtensions.ListUtilPrep(entry);
            bool canReact = ctx.Guild.CurrentUser.GetPermissions(ctx.Channel as IGuildChannel).AddReactions;
            var embed = new EmbedBuilder { Color = Color.DarkBlue }.AddField(x =>
            {
                x.Name = nameMsg;
                x.Value = pageContent[0];
                x.IsInline = false;
            }).WithFooter(x =>
            {
                x.IconUrl = "https://i.imgur.com/nXNBrlr.png";
                x.Text = $"Page 1 of {pageContent.Count}";
            });

            if (!canReact && pageContent.Count > 1)
            {
                embed.AddField(x =>
                {
                    x.Name = "Missing \"Add Reactions\" Permission";
                    x.Value = "Displaying only the first page of the list due to embed field limits.";
                });
            }

            var msg = await ctx.Message.Channel.SendMessageAsync(embed: embed.Build()).ConfigureAwait(false);
            if (pageContent.Count > 1 && canReact)
                _ = Task.Run(async () => await ReactionAwait(ctx, msg, nameMsg, pageContent).ConfigureAwait(false));
        }

        private async Task ReactionAwait(SocketCommandContext ctx, RestUserMessage msg, string nameMsg, List<string> pageContent)
        {
            int page = 0;
            var userId = ctx.User.Id;
            IEmote[] reactions = { new Emoji("⬅️"), new Emoji("➡️"), new Emoji("⬆️"), new Emoji("⬇️") };
            await msg.AddReactionsAsync(reactions).ConfigureAwait(false);
            var embed = new EmbedBuilder { Color = Color.DarkBlue }.AddField(x => { x.Name = nameMsg; x.IsInline = false; }).WithFooter(x => { x.IconUrl = "https://i.imgur.com/nXNBrlr.png"; });
            var sw = new Stopwatch();
            sw.Start();

            while (sw.ElapsedMilliseconds < 20_000)
            {
                await msg.UpdateAsync().ConfigureAwait(false);
                var react = msg.Reactions.FirstOrDefault(x => x.Value.ReactionCount > 1 && x.Value.IsMe);
                if (react.Key == default)
                    continue;

                if (react.Key.Name == reactions[0].Name || react.Key.Name == reactions[1].Name)
                {
                    var reactUsers = await msg.GetReactionUsersAsync(reactions[react.Key.Name == reactions[0].Name ? 0 : 1], 100).FlattenAsync().ConfigureAwait(false);
                    var usr = reactUsers.FirstOrDefault(x => x.Id == userId && !x.IsBot);
                    if (usr == default)
                        continue;

                    if (react.Key.Name == reactions[0].Name)
                    {
                        if (page == 0)
                            page = pageContent.Count - 1;
                        else page--;
                    }
                    else
                    {
                        if (page + 1 == pageContent.Count)
                            page = 0;
                        else page++;
                    }

                    embed.Fields[0].Value = pageContent[page];
                    embed.Footer.Text = $"Page {page + 1} of {pageContent.Count}";
                    await msg.RemoveReactionAsync(reactions[react.Key.Name == reactions[0].Name ? 0 : 1], usr);
                    await msg.ModifyAsync(msg => msg.Embed = embed.Build()).ConfigureAwait(false);
                    sw.Restart();
                }
                else if (react.Key.Name == reactions[2].Name || react.Key.Name == reactions[3].Name)
                {
                    var reactUsers = await msg.GetReactionUsersAsync(reactions[react.Key.Name == reactions[2].Name ? 2 : 3], 100).FlattenAsync().ConfigureAwait(false);
                    var usr = reactUsers.FirstOrDefault(x => x.Id == userId && !x.IsBot);
                    if (usr == default)
                        continue;

                    List<string> tempList = new();
                    foreach (var p in pageContent)
                    {
                        var split = p.Replace(", ", ",").Split(',');
                        tempList.AddRange(split);
                    }

                    var tempEntry = string.Join(", ", react.Key.Name == reactions[2].Name ? tempList.OrderBy(x => x.Split(' ')[1]) : tempList.OrderByDescending(x => x.Split(' ')[1]));
                    pageContent = TradeExtensions.ListUtilPrep(tempEntry);
                    embed.Fields[0].Value = pageContent[page];
                    embed.Footer.Text = $"Page {page + 1} of {pageContent.Count}";
                    await msg.RemoveReactionAsync(reactions[react.Key.Name == reactions[2].Name ? 2 : 3], usr);
                    await msg.ModifyAsync(msg => msg.Embed = embed.Build()).ConfigureAwait(false);
                    sw.Restart();
                }
            }
            await msg.RemoveAllReactionsAsync().ConfigureAwait(false);
        }

        public async Task<bool> ReactionVerification(SocketCommandContext ctx)
        {
            var sw = new Stopwatch();
            IEmote[] reaction = { new Emoji("👍") };
            var msg = await ctx.Channel.SendMessageAsync($"{ctx.User.Username}, please react to the attached emoji in order to confirm you're not using a script.").ConfigureAwait(false);
            await msg.AddReactionsAsync(reaction).ConfigureAwait(false);

            sw.Start();
            while (sw.ElapsedMilliseconds < 20_000)
            {
                await msg.UpdateAsync().ConfigureAwait(false);
                var react = msg.Reactions.FirstOrDefault(x => x.Value.ReactionCount > 1 && x.Value.IsMe);
                if (react.Key == default)
                    continue;

                if (react.Key.Name == reaction[0].Name)
                {
                    var reactUsers = await msg.GetReactionUsersAsync(reaction[0], 100).FlattenAsync().ConfigureAwait(false);
                    var usr = reactUsers.FirstOrDefault(x => x.Id == ctx.User.Id && !x.IsBot);
                    if (usr == default)
                        continue;

                    await msg.AddReactionAsync(new Emoji("✅")).ConfigureAwait(false);
                    return false;
                }
            }
            await msg.AddReactionAsync(new Emoji("❌")).ConfigureAwait(false);
            TradeExtensions.MuteList.Add(ctx.User.Id);
            return true;
        }

        public async Task<int> EventVoteCalc(SocketCommandContext ctx, List<PokeEventType> events)
        {
            IEmote[] reactions = { new Emoji("1️⃣"), new Emoji("2️⃣"), new Emoji("3️⃣"), new Emoji("4️⃣"), new Emoji("5️⃣") };
            string text = "The community vote has started! You have 30 seconds to vote for the next event!\n";
            for (int i = 0; i < events.Count; i++)
                text += $"{i + 1}. {events[i]}\n";

            var embed = new EmbedBuilder { Color = Color.DarkBlue }.AddField(x =>
            {
                x.Name = "Community Event Vote";
                x.Value = text;
                x.IsInline = false;
            });

            var msg = await ctx.Message.Channel.SendMessageAsync(embed: embed.Build()).ConfigureAwait(false);
            await msg.AddReactionsAsync(reactions).ConfigureAwait(false);

            await Task.Delay(30_000).ConfigureAwait(false);
            await msg.UpdateAsync().ConfigureAwait(false);
            List<int> reactList = new();
            for (int i = 0; i < 5; i++)
                reactList.Add(msg.Reactions.Values.ToArray()[i].ReactionCount);
            return reactList.IndexOf(reactList.Max());
        }

        public async Task EmbedUtil(SocketCommandContext ctx, string name, string value, EmbedBuilder? embed = null)
        {
            if (embed == null)
                embed = new EmbedBuilder { Color = Color.DarkBlue };

            var splitName = name.Split(new string[] { "&^&" }, StringSplitOptions.None);
            var splitValue = value.Split(new string[] { "&^&" }, StringSplitOptions.None);
            for (int i = 0; i < splitName.Length; i++)
            {
                embed.AddField(x =>
                {
                    x.Name = splitName[i];
                    x.Value = splitValue[i];
                    x.IsInline = false;
                });
            }
            await ctx.Message.Channel.SendMessageAsync(embed: embed.Build()).ConfigureAwait(false);
        }
    }
}