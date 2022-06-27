using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using SysBot.Base;
using PermuteMMO.Lib;

namespace SysBot.Pokemon.Discord
{
    public class PermuteUtil
    {
        public static async Task HandlePermuteRequestAsync(SocketMessageComponent component, string id)
        {
            if (id.Contains("permute_yes"))
            {
                var msg = $"{component.User.Username}#{component.User.Discriminator} ({component.User.Id}) wants to use PermuteMMO. Waiting for JSON input...";
                LogUtil.LogInfo(msg, "[PermuteMMO Request]");

                var emb = component.Message.Embeds.First();
                await UpdatePermuteEmbed(component.Message, emb.Description, Color.Gold, null, MessageFlags.None, emb.Author?.Name).ConfigureAwait(false);

                var seed = id.Split(';')[1];
                var buttonReady = new ButtonBuilder() { CustomId = $"permute_ready;{seed}", Label = "Ready", Style = ButtonStyle.Success };
                var components = new ComponentBuilder().WithButton(buttonReady);
                var desc = "Please configure and generate your JSON by clicking [this link](https://shinyhunter.club/tools/permutemmo-spawners). Once done, click the button to let me know you're ready!";

                var embed = new EmbedBuilder
                {
                    Color = Color.Blue,
                    Description = desc,
                }.WithAuthor(x => { x.Name = "PermuteMMO Service"; });

                await component.Message.ReplyAsync(null, false, embed.Build(), null, null, components.Build()).ConfigureAwait(false);
            }
            else if (id.Contains("permute_ready"))
            {
                var seed = id.Split(';')[1];
                var box = new TextInputBuilder() { CustomId = $"permute_json;{seed}", Label = "PermuteMMO JSON", Placeholder = "Paste the JSON output here...", Required = true, MinLength = 165 }.WithStyle(TextInputStyle.Paragraph);
                var mod = new ModalBuilder() { Title = "PermuteMMO Service", CustomId = $"permute_json;{seed}" }.AddTextInput(box);
                await component.RespondWithModalAsync(mod.Build()).ConfigureAwait(false);
            }
            else if (id.Contains("permute_no"))
            {
                var msg = component.Message.Embeds.First().Description.Split('\n').First();
                await UpdatePermuteEmbed(component.Message, msg, Color.LightOrange).ConfigureAwait(false);

                var username = $"{component.User.Username}#{component.User.Discriminator} ({component.User.Id})";
                LogUtil.LogInfo($"{username} did not wish to use PermuteMMO.", "[PermuteMMO Request]");
            }
        }

        public static async Task DoPermutationsAsync(SocketModal modal)
        {
            var id = modal.Data.CustomId;
            var seed = ulong.Parse(id.Split(';')[1]);
            var json = modal.Data.Components.First().Value;

            var name = $"{modal.User.Username}#{modal.User.Discriminator} ({modal.User.Id})";
            var msg = $"{name} has submitted their JSON. Running PermuteMMO...";
            LogUtil.LogInfo(msg, "[PermuteMMO]");

            UserEnteredSpawnInfo? info;
            try
            {
                info = JsonConvert.DeserializeObject<UserEnteredSpawnInfo>(json);
            }
            catch
            {
                info = null;
            }

            if (info is null)
            {
                msg = "Provided JSON is invalid.";
                LogUtil.LogInfo($"{name}: {msg}", "[PermuteMMO]");
                await ModalEmbedFollowup(modal, msg, Color.Red);
                return;
            }

            // Split into another interaction for customizable filters?
            PermuteMeta.SatisfyCriteria = (result, advances) => result.IsShiny;
            var spawner = info.GetSpawn();
            var meta = Permuter.Permute(spawner, seed);            
            if (!meta.HasResults)
            {
                msg = "No shiny path results found.";
                LogUtil.LogInfo($"{name}: {msg}", "[PermuteMMO]");

                var embedNoRes = new EmbedBuilder
                {
                    Color = Color.Red,
                    Description = msg,
                }.WithAuthor(x => { x.Name = "PermuteMMO Service"; });

                await modal.ModifyOriginalResponseAsync(x =>
                {
                    x.Embed = embedNoRes.Build();
                    x.Components = new ComponentBuilder().Build();
                }).ConfigureAwait(false);
                return;
            }

            msg = $"Permutation complete! Sending {name} their results!";
            LogUtil.LogInfo(msg, "[PermuteMMO]");

            var embed = new EmbedBuilder
            {
                Color = Color.Gold,
                Description = "**Here are your shiny path results!**",
            }.WithAuthor(x => { x.Name = "PermuteMMO Service"; });

            var res = string.Join("\n", meta.GetLines());
            var bytes = Encoding.UTF8.GetBytes(res);
            var ms = new MemoryStream(bytes);
            var att = new FileAttachment[] { new FileAttachment(ms, $"PermuteMMO_{seed}.txt") };

            await modal.ModifyOriginalResponseAsync(x =>
            {
                x.Attachments = att;
                x.Embed = embed.Build();
                x.Components = new ComponentBuilder().Build();
            }).ConfigureAwait(false);
        }

        private static async Task UpdatePermuteEmbed(SocketUserMessage message, string desc, Color color, MessageComponent? components = null, MessageFlags flag = MessageFlags.None, string? authorName = null)
        {
            var embed = new EmbedBuilder
            {
                Color = color,
                Description = desc,
            }.WithAuthor(x => { x.Name = authorName ?? "PermuteMMO Service"; });

            await message.ModifyAsync(x =>
            {
                x.Embed = embed.Build();
                x.Flags = flag;
                x.Components = components;
            }).ConfigureAwait(false);
        }

        private static async Task ModalEmbedFollowup(SocketModal modal, string desc, Color color)
        {
            var embed = new EmbedBuilder
            {
                Color = color,
                Description = desc,
            }.WithAuthor(x => { x.Name = "PermuteMMO Service"; });

            await modal.FollowupAsync(null, null, false, false, null, null, embed: embed.Build()).ConfigureAwait(false);
        }
    }
}
