using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using SysBot.Base;
using PermuteMMO.Lib;
using PKHeX.Core;

namespace SysBot.Pokemon.Discord;

public static class PermuteUtil
{
    // Thanks to Zyro for his initial implementation of multis!
    // https://github.com/zyro670/NotForkBot.NET/commit/b64bbfc344fbbdaba9982a2b111c116698c830fe

    private static SlotDetail[][] FakeSlots = [];

    private static void InitializeSlots()
    {
        FakeSlots =
        [
            // Fieldlands: 87F39EBD429BF90C; Not Night; Sunny, Cloudy, Intense Sun, Fog, Rainstorm (2-2)
            [
                new(100, "Bidoof", false, [3,  6], 0),
                new(  2, "Bidoof", true,  [18, 21], 3),
                new( 20, "Eevee",  false, [3,  6], 0),
                new(  1, "Eevee",  true,  [18, 21], 3),
            ],
            // Mirelands: 0D8EAAF095C52F35, E1ABFA12F052870F; Not Night; All weather (2-2)
            [
                new(100, "Combee", false, [17, 20], 0),
                new(  2, "Combee", true,  [32, 35], 3),
            ],
            // Coastlands: 98377DDDC282361F, A6E9A110EBF37C0D, BFDB454F1B881B09, 3B2E318D82A87165; Not Night; All weather (3-3)
            [
                new(100, "Qwilfish-1", false, [41, 44], 0),
                new(  1, "Qwilfish-1", true,  [56, 59], 3),
            ],
            // Fieldlands: B829961431D954A4; Not Night; All weather (2-2)
            [
                new(100, "Abra",    false, [12, 15], 0),
                new(  2, "Abra",    true,  [27, 30], 3),
                new( 30, "Kadabra", false, [16, 19], 0),
                new(  1, "Kadabra", true,  [31, 34], 3),
            ],
            // Coastlands: D8AB0E11253761F3, 2630CA408C60BE57, 617CED822144D04A; Not Night; All weather (2-2)
            [
                new(100, "Basculin-2", false, [41, 44], 0),
                new(  1, "Basculin-2", true,  [56, 59], 3),
            ],
            // Fieldlands: A677C613CF0236A7; All times; Sunny, Cloudy, Fog (2-2)
            [
                new(100, "Magikarp", false, [16, 19], 0),
                new(  2, "Magikarp", true,  [31, 34], 3),
                new( 30, "Gyarados", false, [53, 56], 0),
                new(  1, "Gyarados", true,  [68, 71], 3),
            ],
            // Fieldlands: 264C1AE897749DE3; All times; All weather (2-2)
            [
                new(100, "Shellos",   false, [26, 29], 0),
                new(  2, "Shellos",   true,  [41, 44], 3),
                new( 50, "Gastrodon", false, [33, 36], 0),
                new(  1, "Gastrodon", true,  [48, 51], 3),
            ],
            // Mirelands: DFDE7E3B3FFEF5C9; Not Night; Cloudy, Rain, Rainstorm (2-2)
            [
                new( 40, "Ralts",   false, [19, 22], 0),
                new(  1, "Ralts",   true,  [34, 37], 3),
                new(100, "Budew",   false, [19, 22], 0),
                new(  2, "Budew",   true,  [34, 37], 3),
                new( 50, "Roselia", false, [19, 22], 0),
                new(  1, "Roselia", true,  [34, 37], 3),
            ],
            // Mirelands: 3402DE384DC3A82A; Not Night; All weather (2-2)
            [
                new(100, "Hippopotas", false, [21, 24], 0),
                new(  2, "Hippopotas", true,  [36, 39], 3),
                new( 30, "Hippowdon",  false, [34, 37], 0),
                new(  1, "Hippowdon",  true,  [49, 52], 3),
            ],
            // Fieldlands: 0E4264E9B8F2E85F; Not Night; All weather (2-2)
            [
                new(100, "Aipom", false, [24, 27], 0),
                new(  2, "Aipom", true,  [39, 42], 3),
            ],
            // Fieldlands: 7F13E7A756EAECEA; Not Night; All weather (2-2)
            [
                new( 20, "Pikachu",   false, [9, 12], 0),
                new(  1, "Pikachu",   true,  [24, 27], 3),
                new( 10, "Pichu",     false, [9, 12], 0),
                new(  1, "Pichu",     true,  [24, 27], 3),
                new(100, "Kricketot", false, [6,  9], 0),
                new(  2, "Kricketot", true,  [21, 24], 3),
            ],
            // Fieldlands: 371EA9452422392A; All times; Sunny, Cloudy, Fog (2-2)
            [
                new(100, "Psyduck", false, [13, 16], 0),
                new( 25, "Psyduck", false, [13, 16], 0),
                new(  2, "Psyduck", true,  [28, 31], 3),
                new(100, "Buneary", false, [13, 16], 0),
                new( 25, "Buneary", false, [13, 16], 0),
                new(  2, "Buneary", true,  [28, 31], 3),
            ],
            // Mirelands: 90B205067060D0BC; Not Night; Not Sunny (3-3)
            [
                new(100, "Petilil", false, [33, 36], 0),
                new(  2, "Petilil", true,  [48, 51], 3),
            ],
            // Mirelands: 90B205067060D0BC; Night; Not Sunny (3-3)
            [
                new(100, "Petilil", false, [33, 36], 0),
                new(  2, "Petilil", true,  [48, 51], 3),
                new( 50, "Gastly",  false, [21, 24], 0),
                new(  2, "Gastly",  true,  [36, 39], 3),
                new( 30, "Haunter", false, [33, 36], 0),
                new(  1, "Haunter", true,  [48, 51], 3),
            ],
            // Coastlands: 3BC31AA6F5337A4D; All times; All weather (2-2)
            [
                new(100, "Glameow", false, [34, 37], 0),
                new(  1, "Glameow", true,  [49, 52], 3),
                new( 40, "Purugly", false, [41, 44], 0),
                new(  1, "Purugly", true,  [56, 59], 3),
            ],
            // Highlands: 8A628649D2AB899C; All times; All weather (2-2)
            [
                new( 30, "Teddiursa", false, [26, 29], 0),
                new(  2, "Teddiursa", true,  [41, 44], 3),
                new(100, "Ursaring",  false, [37, 40], 0),
                new(  1, "Ursaring",  true,  [52, 55], 3),
            ],
            // Fieldlands: A366202BD3643B7E; Not Night; All weather (2-2)
            [
                new(100, "Beautifly", false, [15, 18], 0),
                new(  1, "Beautifly", true,  [30, 33], 3),
                new(100, "Mothim",    false, [20, 23], 0),
                new(  1, "Mothim",    true,  [35, 38], 3),
            ],
            // Fieldlands: A366202BD3643B7E; Night; All weather (2-2)
            [
                new(100, "Dustox", false, [15, 18], 0),
                new(  1, "Dustox", true,  [30, 33], 3),
                new(100, "Mothim", false, [20, 23], 0),
                new(  1, "Mothim", true,  [35, 38], 3),
            ],
            // Coastlands: 1FB2A7A1C1FBFEBD; Night; All weather (2-2)
            [
                new(100, "Murkrow", false, [31, 34], 0),
                new(  2, "Murkrow", true,  [46, 49], 3),
            ],
            // Icelands: 81AB7DC29C2E5AB3; Not Night; Sunny, Cloudy (2-2)
            [
                new(100, "Swinub",    false, [29, 32], 0),
                new(  2, "Swinub",    true,  [44, 47], 3),
                new( 50, "Piloswine", false, [47, 50], 0),
                new(  1, "Piloswine", true,  [62, 65], 3),
            ],
            // Mirelands: A3C27A3A01165FE6; Not Night; Cloudy, Rain, Fog, Rainstorm (2-2)
            [
                new(100, "Paras",    false, [20, 23], 0),
                new(  2, "Paras",    true,  [35, 38], 3),
                new( 50, "Parasect", false, [25, 28], 0),
                new(  1, "Parasect", true,  [40, 43], 3),
            ],
            // Mirelands: A3C27A3A01165FE6; Night; Cloudy, Rain, Fog, Rainstorm (2-2)
            [
                new(100, "Paras",    false, [20, 23], 0),
                new(  2, "Paras",    true,  [35, 38], 3),
                new( 50, "Parasect", false, [25, 28], 0),
                new(  1, "Parasect", true,  [40, 43], 3),
                new( 70, "Zubat",    false, [18, 21], 0),
                new(  2, "Zubat",    true,  [33, 36], 3),
                new( 30, "Golbat",   false, [25, 28], 0),
                new(  1, "Golbat",   true,  [40, 43], 3),
            ],
            // Icelands: F4A47B912D22A05A, F91217341C696F1F; All times; All weather (2-2)
            [
                new(100, "Rufflet", false, [55, 58], 0),
                new(  1, "Rufflet", true,  [70, 73], 3),
            ],
            // Fieldlands: No number: spawner close to the crescent-moon shaped lake in the North-East, right above the Alpha Floatzel; Not Night; Any weather (2-2)
            [
                new(100, "Buizel", false, [2, 4], 0),
                new(  2, "Buizel", true,  [17, 19], 3),
            ],
            // Coastlands: No number: spawner in the water just south of the Molten Arena; Night; Sunny, Cloudy, Drought, Fog (3-3)
            [
                new(100, "Tentacool",    false, [24, 29], 0),
                new(  2, "Tentacool",    true,  [39, 44], 3),
                new( 60, "Tentacruel", false, [31, 34], 0),
                new(  1, "Tentacruel", true,  [46, 49], 3),
                new(100, "Drifloon",    false, [24, 27], 0),
                new(  2, "Drifloon",    true,  [39, 42], 3),
                new( 60, "Drifblim",   false, [31, 34], 0),
                new(  1, "Drifblim",   true,  [46, 49], 3),
            ],
            // Icelands: 81AB7DC29C2E5AB3; Night; Sunny, Cloudy (2-2)
            [
                new(100, "Swinub",    false, [29, 32], 0),
                new(  2, "Swinub",    true,  [44, 47], 3),
                new( 50, "Piloswine", false, [47, 50], 0),
                new(  1, "Piloswine", true,  [62, 65], 3),
                new( 20, "Duskull",    false, [33, 36], 0),
                new(  2, "Duskull",    true,  [48, 51], 3),
                new(100, "Dusclops", false, [47, 50], 0),
                new(  1, "Dusclops", true,  [62, 65], 3),
            ],
        ];

        for (int s = 0; s < FakeSlots.Length; s++)
        {
            for (int i = 0; i < FakeSlots[s].Length; i++)
                FakeSlots[s][i].SetSpecies();

            var key = FakeSlots[s].Any(x => x.Species == (ushort)Species.Basculin)
                ? 0x123456B0
                : (ulong)(0x694201020 + s);

            SpawnGenerator.EncounterTables.Add(key, FakeSlots[s]);
        }
    }

    public static async Task HandlePermuteRequestAsync(SocketMessageComponent component, string service, string id)
    {
        if (FakeSlots.Length is 0)
            InitializeSlots();

        if (id is "permute_yes")
        {
            var msg = $"{component.User.Username}#{component.User.Discriminator} ({component.User.Id}) wants to use PermuteMMO. Waiting for the user to choose a service...";
            LogUtil.LogInfo(msg, "[PermuteMMO Request]");

            var emb = component.Message.Embeds.First();
            var selectMenuBuilder = GetPermuteServiceSelectMenu();
            var menu = new ComponentBuilder().WithSelectMenu(selectMenuBuilder).Build();
            var embed = new EmbedBuilder
            {
                Color = Color.Gold,
                Description = "Please select PermuteMMO service you would like to use!",
            }.WithAuthor(x => { x.Name = emb.Author?.Name ?? "PermuteMMO Service"; }).Build();

            await component.Message.ReplyAsync(null, false, embed, null, null, menu).ConfigureAwait(false);
        }
        else if (id.Contains("permute_json_select"))
        {
            var spawnerVal = service is "multi" && component.Data.Values.First() is not "multi" ? component.Data.Values.First() : "";
            var msg = $"{component.User.Username}#{component.User.Discriminator} ({component.User.Id}) chose {(spawnerVal is not "" ? spawnerVal : service)}. {(service is "multi" && spawnerVal is "" ? "Waiting for the user to choose a spawner..." : "Waiting for the user to choose a filter...")}";
            LogUtil.LogInfo(msg, "[PermuteMMO Request]");

            var selectMenuBuilder = HandlePermuteSelectMenu(service, spawnerVal);
            var menu = new ComponentBuilder().WithSelectMenu(selectMenuBuilder).Build();
            var embed = new EmbedBuilder
            {
                Color = Color.Gold,
                Description = $"{(service is "multi" && spawnerVal is "" ? "Please select a spawner you would like to permute!" : "Please select your shiny path filter for PermuteMMO!")}",
            }.WithAuthor(x => x.Name = "PermuteMMO Service").Build();

            await component.Message.ModifyAsync(x => { x.Embed = embed; x.Components = menu; }).ConfigureAwait(false);
        }
        else if (id.Contains("permute_ready"))
        {
            var spawner = component.Data.CustomId.Split(';')[2];
            var filter = component.Data.CustomId.Split(';')[3];

            if (service is "mmo")
                await PromptPermuteAsync(component, service, filter).ConfigureAwait(false);
            else await PromptPermuteMultiAsync(component, service, spawner, filter).ConfigureAwait(false);
        }
        else if (id is "permute_no")
        {
            var msg = component.Message.Embeds.First().Description.Split('\n').First();
            await UpdatePermuteEmbed(component.Message, msg, Color.LightOrange).ConfigureAwait(false);

            var username = $"{component.User.Username}#{component.User.Discriminator} ({component.User.Id})";
            LogUtil.LogInfo($"{username} did not wish to use PermuteMMO.", "[PermuteMMO Request]");
        }
    }

    public static async Task VerifyAndRunPermuteAsync(SocketModal modal, string service)
    {
        if (service is "multi")
            await RunPermuteMultiAsync(modal).ConfigureAwait(false);
        else await RunPermuteMmoAsync(modal).ConfigureAwait(false);
    }

    private static async Task RunPermuteMmoAsync(SocketModal modal)
    {
        var data = modal.Data;
        var json = data.Components.First().Value;

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

        msg = "Provided JSON is invalid: ";
        if (info is null)
        {
            msg += "Invalid JSON format.";
            LogUtil.LogInfo($"{name}: {msg}", "[PermuteMMO]");
            await ModalEmbedFollowupAsync(modal, msg, Color.Red).ConfigureAwait(false);
            return;
        }

        bool invalidBonus = (info.BonusCount is not 0 and not 6 and not 7) || (info.BonusTable == "0x0000000000000000" && info.BonusCount != 0);
        if (invalidBonus)
        {
            msg += "Incorrect second wave spawn count specified, or no second wave provided with a non-zero second wave count.";
            LogUtil.LogInfo($"{name}: {msg}", "[PermuteMMO]");
            await ModalEmbedFollowupAsync(modal, msg, Color.Red).ConfigureAwait(false);
            return;
        }

        bool invalidBase = info.BaseCount is < 8 or > 15;
        if (invalidBase)
        {
            msg += "Incorrect first wave count specified.";
            LogUtil.LogInfo($"{name}: {msg}", "[PermuteMMO]");
            await ModalEmbedFollowupAsync(modal, msg, Color.Red).ConfigureAwait(false);
            return;
        }

        var filter = modal.Data.CustomId.Split(';')[2];
        await DoPermutationsAsync(modal, info!, filter, name).ConfigureAwait(false);
    }

    private static async Task RunPermuteMultiAsync(SocketModal modal)
    {
        var data = modal.Data;
        var name = $"{modal.User.Username}#{modal.User.Discriminator} ({modal.User.Id})";
        var msg = $"{name} has submitted their multispawner modal. Running PermuteMMO...";
        LogUtil.LogInfo(msg, "[PermuteMMO]");

        msg = "Invalid input: ";
        var seedInput = data.Components.FirstOrDefault(x => x.CustomId == "seed")?.Value;
        if (seedInput == default || !ulong.TryParse(seedInput, out var seed))
        {
            msg += "Invalid seed.";
            LogUtil.LogInfo($"{name}: {msg}", "[PermuteMMO]");
            await ModalEmbedFollowupAsync(modal, msg, Color.Red).ConfigureAwait(false);
            return;
        }

        var filter = modal.Data.CustomId.Split(';')[3];
        var info = new UserEnteredSpawnInfo { Seed = seedInput, };
        await DoPermutationsAsync(modal, info, filter, name, "multi").ConfigureAwait(false);
    }

    public static async Task HandlePermuteButtonAsync(SocketMessageComponent component, string service)
    {
        var filter = component.Data.Values.First();
        var spawner = service is "multi" ? component.Data.CustomId.Split(';')[2] : "";
        var msg = $"{component.User.Username}#{component.User.Discriminator} ({component.User.Id}) has selected a filter: {filter}. {(service is "multi" ? "Waiting for seed input..." : "Waiting for JSON input...")}";
        LogUtil.LogInfo(msg, "[PermuteMMO Request]");

        var buttonReady = new ButtonBuilder() { CustomId = $"permute_ready;{service};{spawner};{filter}", Label = "Ready", Style = ButtonStyle.Success };
        var components = new ComponentBuilder().WithButton(buttonReady);
        var desc = $"{(service is "mmo" ? "Please configure and generate your JSON by clicking [this link](https://shinyhunter.club/tools/permutemmo-spawners). Once done, click the button to let me know you're ready!" : "Click the button once you're ready to input your seed.")}";

        var embed = new EmbedBuilder
        {
            Color = Color.Blue,
            Description = desc,
        }.WithAuthor(x => { x.Name = "PermuteMMO Service"; });

        await component.Message.ModifyAsync(x =>
        {
            x.Embed = embed.Build();
            x.Components = components.Build();
        }).ConfigureAwait(false);
    }

    private static async Task PromptPermuteAsync(SocketMessageComponent component, string service, string filter)
    {
        var box = new TextInputBuilder() { CustomId = $"permute_json;{service};{filter}", Label = "PermuteMMO JSON", Placeholder = "Paste the JSON output here...", Required = true }.WithStyle(TextInputStyle.Paragraph);
        var mod = new ModalBuilder() { Title = "PermuteMMO Service", CustomId = $"permute_json;{service};{filter}" }.AddTextInput(box);
        await component.RespondWithModalAsync(mod.Build()).ConfigureAwait(false);
    }

    private static async Task PromptPermuteMultiAsync(SocketMessageComponent component, string service, string spawner, string filter)
    {
        var box1 = new TextInputBuilder() { CustomId = "seed", Label = "Seed", Placeholder = "Paste your seed here...", Required = true }.WithStyle(TextInputStyle.Short);
        var mod = new ModalBuilder() { Title = "PermuteMMO Service", CustomId = $"permute_json;{service};{spawner};{filter}" }.AddTextInput(box1);
        await component.RespondWithModalAsync(mod.Build()).ConfigureAwait(false);
    }

    private static async Task DoPermutationsAsync(SocketModal modal, UserEnteredSpawnInfo info, string filter, string name, string service = "mmo")
    {
        string msg = string.Empty;
        PermuteMeta.SatisfyCriteria = (result, advances) => filter switch
        {
            "shiny" => result.IsShiny,
            "shalpha" => result.IsShiny && result.IsAlpha,
            "alpha" => result.IsAlpha || (result.IsShiny && result.IsAlpha),
            _ => result.IsShiny,
        };

        string path = filter switch
        {
            "shiny" => "all shiny paths",
            "shalpha" => "all shiny alpha paths",
            "alpha" => "all alpha paths",
            _ => "all shiny paths",
        };

        ulong seed;
        PermuteMeta meta;
        try
        {
            if (service is "mmo")
            {
                var spawner = info.GetSpawn();
                seed = info.GetSeed();
                meta = Permuter.Permute(spawner, seed, 20);
            }
            else
            {
                var spawnerInput = int.Parse(modal.Data.CustomId.Split(';')[2]);
                var key = SpawnGenerator.EncounterTables.FirstOrDefault(x => x.Value == FakeSlots[spawnerInput]).Key;
                int count = spawnerInput is (2 or 12 or 13 or 24) ? 3 : 2;
                var details = new SpawnCount(count, count);
                var set = new SpawnSet(key, count);
                var spawner = SpawnInfo.GetLoop(details, set, SpawnType.Regular);
                var advances = count is 3 ? 13 : 20;
                seed = info.GetSeed();
                meta = Permuter.Permute(spawner, seed, advances);
            }
        }
        catch
        {
            msg += "Failed to calculate shiny paths due to an unexpected error: is the provided info filled out with valid parameters?";
            LogUtil.LogInfo($"{name}: {msg}", "[PermuteMMO]");
            await ModalEmbedFollowupAsync(modal, msg, Color.Red).ConfigureAwait(false);
            return;
        }

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

        var interpretUrl = "https://github.com/kwsch/PermuteMMO/wiki#interpreting-output";
        var shinyRollUrl = "https://cdn.discordapp.com/attachments/958046779750875146/998925407745220678/IMG_4793-1.png";
        var embed = new EmbedBuilder
        {
            Color = Color.Gold,
            Description = $"**Here are your results for {path}!**\n[Check this link]({interpretUrl}) to learn more about path notations and how to find the minimum shiny rolls you'll need! Make sure you have enough for the path you choose!",
            ImageUrl = shinyRollUrl,
        }.WithAuthor(x => { x.Name = "PermuteMMO Service"; });

        var res = string.Join("\n", meta.GetLines());
        var bytes = Encoding.UTF8.GetBytes(res);
        var ms = new MemoryStream(bytes);
        var att = new FileAttachment[] { new(ms, $"PermuteMMO_{seed}.txt") };

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

        int retryCount = 5;
        const int retryTime = 5_000;

        while (retryCount > 0)
        {
            try
            {
                await message.ModifyAsync(x =>
                {
                    x.Embed = embed.Build();
                    x.Flags = flag;
                    x.Components = components;
                }).ConfigureAwait(false);

                break;
            }
            catch
            {
                retryCount--;
                if (retryCount is not 0)
                    await Task.Delay(retryTime).ConfigureAwait(false);
            }
        }
    }

    private static async Task ModalEmbedFollowupAsync(SocketModal modal, string desc, Color color)
    {
        var embed = new EmbedBuilder
        {
            Color = color,
            Description = desc,
        }.WithAuthor(x => { x.Name = "PermuteMMO Service"; });

        await modal.FollowupAsync(null, null, false, false, null, null, embed: embed.Build()).ConfigureAwait(false);
    }

    private static SelectMenuBuilder HandlePermuteSelectMenu(string service, string value) => service switch
    {
        "multi" when value is not "" => GetPathFilterMenu(service, value),
        "multi" => GetSpawnerSelectMenu(service),
        "mmo" => GetPathFilterMenu(service, value),
        _ => GetPathFilterMenu(service, value),
    };

    private static SelectMenuBuilder GetPathFilterMenu(string service, string value) => new()
    {
        CustomId = $"permute_json_filter;{service};{value}",
        MinValues = 1,
        MaxValues = 1,
        Placeholder = "Select your shiny path filter for PermuteMMO...",
        Options =
        [
            new SelectMenuOptionBuilder("Shiny", "shiny", "All shiny paths."),
            new SelectMenuOptionBuilder("Shiny AND alpha", "shalpha", "Only shiny alpha paths."),
            new SelectMenuOptionBuilder("Alpha", "alpha", "Only alpha paths, including non-shiny."),
        ]
    };

    public static SelectMenuBuilder GetPermuteServiceSelectMenu() => new()
    {
        CustomId = "permute_json_select",
        MinValues = 1,
        MaxValues = 1,
        Placeholder = "Select which service you would like to use...",
        Options =
        [
            new SelectMenuOptionBuilder("MO/MMO", "mmo", "Massive Outbreak or Massive Mass Outbreak"),
            new SelectMenuOptionBuilder("Multi-Spawner", "multi", "Multi-Spawner for regular spawns."),
        ]
    };

    private static SelectMenuBuilder GetSpawnerSelectMenu(string service) => new()
    {
        CustomId = $"permute_json_select;{service}",
        MinValues = 1,
        MaxValues = 1,
        Placeholder = "Select your species...",
        Options =
        [
            new SelectMenuOptionBuilder("Eevee/Bidoof", "0", "All shiny Eevee/Bidoof paths"),
            new SelectMenuOptionBuilder("Combee", "1", "All shiny Combee paths"),
            new SelectMenuOptionBuilder("Qwilfish", "2", "All shiny Qwilfish paths"),
            new SelectMenuOptionBuilder("Abra", "3", "All shiny Abra paths"),
            new SelectMenuOptionBuilder("Basculin", "4", "All shiny Basculin paths"),
            new SelectMenuOptionBuilder("Magikarp", "5", "All shiny Magikarp paths"),
            new SelectMenuOptionBuilder("Shellos", "6", "All shiny Shellos paths"),
            new SelectMenuOptionBuilder("Ralts", "7", "All shiny Ralts paths"),
            new SelectMenuOptionBuilder("Hippopotas", "8", "All shiny Hippopotas paths"),
            new SelectMenuOptionBuilder("Aipom", "9", "All shiny Aipom paths"),
            new SelectMenuOptionBuilder("Kricketot/Pichu/Pikachu", "10", "All shiny Kricketot/Pichu/Pikachu paths"),
            new SelectMenuOptionBuilder("Psyduck/Buneary", "11", "All shiny Psyduck/Buneary paths"),
            new SelectMenuOptionBuilder("Petilil", "12", "All shiny Petilil paths"),
            new SelectMenuOptionBuilder("Petilil/Gastly/Haunter", "13", "All shiny Petilil/Gastly/Haunter paths"),
            new SelectMenuOptionBuilder("Glameow/Purugly", "14", "All shiny Glameow/Purugly paths"),
            new SelectMenuOptionBuilder("Teddiursa/Ursaring", "15", "All shiny Teddiursa/Ursaring paths"),
            new SelectMenuOptionBuilder("Beautifly/Mothim", "16", "All shiny Beautifly/Mothim paths"),
            new SelectMenuOptionBuilder("Dustox/Mothim", "17", "All shiny Dustox/Mothim paths"),
            new SelectMenuOptionBuilder("Murkrow", "18", "All shiny Murkrow paths"),
            new SelectMenuOptionBuilder("Swinub/Piloswine", "19", "All shiny Swinub/Piloswine paths"),
            new SelectMenuOptionBuilder("Paras/Parasect", "20", "All shiny Paras/Parasect paths"),
            new SelectMenuOptionBuilder("Paras/Parasect/Zubat/Golbat", "21", "All shiny Paras/Parasect/Zubat/Golbat paths"),
            new SelectMenuOptionBuilder("Rufflet", "22", "All shiny Rufflet paths"),
            new SelectMenuOptionBuilder("Buizel", "23", "All shiny Buizel paths"),
            new SelectMenuOptionBuilder("Tentacool/Tentacruel/Drifloon/Drifblim", "24", "All shiny Tentacool/Tentacruel/Drifloon/Drifblim paths"),
            new SelectMenuOptionBuilder("Swinub/Piloswine/Duskull/Dusclops", "25", "All shiny Swinub/Piloswine/Duskull/Dusclops paths")
        ]
    };
}
