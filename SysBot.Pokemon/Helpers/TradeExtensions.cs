﻿using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using PKHeX.Core;
using PKHeX.Core.AutoMod;
using Newtonsoft.Json;

namespace SysBot.Pokemon
{
    public class TradeExtensions
    {
        public static Random Random = new Random();
        public static byte[] Data = new byte[] { };
        public static bool TCInitialized;
        private static bool TCRWLockEnable;
        private static bool NewUserLock = false;
        public static readonly List<ulong> CommandInProgress = new();
        private static readonly List<ulong> GiftInProgress = new();
        public static List<string> TradeCordPath = new();
        public static List<string> TradeCordCooldown = new();
        private static readonly string InfoPath = "TradeCord\\UserInfo.json";
        public static TCUserInfoRoot UserInfo = GetRoot<TCUserInfoRoot>(InfoPath);
        public static int XCoordStart = 0;
        public static int YCoordStart = 0;

        public static uint AlcremieDecoration { get => BitConverter.ToUInt32(Data, 0xE4); set => BitConverter.GetBytes(value).CopyTo(Data, 0xE4); }
        public static int[] ValidEgg =
                { 1, 4, 7, 10, 27, 29, 32, 37, 41, 43, 50, 52, 54, 58, 60, 63, 66, 72,
                  77, 79, 81, 83, 90, 92, 95, 98, 102, 104, 108, 109, 111, 114, 115, 116,
                  118, 120, 122, 123, 127, 128, 129, 131, 133, 137, 138, 140, 142, 147, 163,
                  170, 172, 173, 174, 175, 177, 194, 206, 211, 213, 214, 215, 220, 222, 223,
                  225, 227, 236, 238, 239, 240, 241, 246, 252, 255, 258, 263, 270, 273, 278,
                  280, 290, 293, 298, 302, 303, 304, 309, 318, 320, 324, 328, 333, 337, 338,
                  339, 341, 343, 345, 347, 349, 355, 359, 360, 361, 363, 369, 371, 374, 403,
                  406, 415, 420, 422, 425, 427, 434, 436, 438, 439, 440, 442, 443, 446, 447,
                  449, 451, 453, 458, 459, 479, 506, 509, 517, 519, 524, 527, 529, 531, 532,
                  535, 538, 539, 543, 546, 548, 550, 551, 554, 556, 557, 559, 561, 562, 564,
                  566, 568, 570, 572, 574, 577, 582, 587, 588, 590, 592, 595, 597, 599, 605,
                  607, 610, 613, 615, 616, 618, 619, 621, 622, 624, 626, 627, 629, 631, 632,
                  633, 636, 659, 661, 674, 677, 679, 682, 684, 686, 688, 690, 692, 694, 696,
                  698, 701, 702, 703, 704, 707, 708, 710, 712, 714, 722, 725, 728, 736, 742,
                  744, 746, 747, 749, 751, 753, 755, 757, 759, 761, 764, 765, 766, 767, 769,
                  771, 776, 777, 778, 780, 781, 782, 810, 813, 816, 819, 821, 824, 827, 829,
                  831, 833, 835, 837, 840, 843, 845, 846, 848, 850, 852, 854, 856, 859, 868,
                  870, 871, 872, 874, 875, 876, 877, 878, 884, 885 };

        public static int[] GenderDependent = { 3, 12, 19, 20, 25, 26, 41, 42, 44, 45, 64, 65, 84, 85, 97, 111, 112, 118, 119, 123, 129, 130, 133,
                                                178, 185, 186, 194, 195, 202, 208, 212, 214, 215, 221, 224,
                                                255, 256, 257, 272, 274, 275, 315, 350, 369,
                                                403, 404, 405, 407, 415, 443, 444, 445, 449, 450, 453, 454, 459, 460, 461, 464, 465, 473,
                                                521, 592, 593,
                                                668 };

        public static int[] Legends = { 144, 145, 146, 150, 151,
                                        243, 244, 245, 249, 250, 251,
                                        377, 378, 379, 380, 381, 382, 383, 384, 385,
                                        480, 481, 482, 483, 484, 485, 486, 487, 488,
                                        494, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 649,
                                        716, 717, 718, 719, 721,
                                        772, 773, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809,
                                        888, 889, 890, 891, 892, 893, 894, 895, 896, 897, 898 };

        public static int[] PikaClones = { 25, 26, 172, 587, 702, 777, 877 };

        public static int[] ShinyLock = { (int)Species.Victini, (int)Species.Keldeo, (int)Species.Volcanion, (int)Species.Cosmog, (int)Species.Cosmoem, (int)Species.Magearna,
                                          (int)Species.Marshadow, (int)Species.Zacian, (int)Species.Zamazenta, (int)Species.Eternatus, (int)Species.Kubfu, (int)Species.Urshifu,
                                          (int)Species.Zarude, (int)Species.Glastrier, (int)Species.Spectrier, (int)Species.Calyrex };

        public static int[] Foreign = { 150, 151, 243, 244, 245, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260, 380, 381, 382, 383, 384, 385, 480, 481, 482, 483, 484, 485, 486, 487, 488, 494,
                                        641, 642, 643, 644, 645, 646, 647, 649, 716, 717, 718, 719, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 795,
                                        796, 797, 798, 799, 800, 801, 802, 803, 804, 805, 806, 807, 808, 809 };

        public static int[] TradeEvo = { (int)Species.Machoke, (int)Species.Haunter, (int)Species.Boldore, (int)Species.Gurdurr, (int)Species.Phantump, (int)Species.Gourgeist };
        public static string[] PartnerPikachuHeadache = { "-Original", "-Partner", "-Hoenn", "-Sinnoh", "-Unova", "-Alola", "-Kalos", "-World" };
        public static string[] LGPEBalls = { "Poke", "Premier", "Great", "Ultra", "Master" };
        public static int[] CherishOnly = { 251, 385, 494, 649, 719, 721, 801, 802, 807, 893 };
        public static int[] Pokeball = { 151, 722, 723, 724, 725, 726, 727, 728, 729, 730, 772, 773, 789, 790, 810, 811, 812, 813, 814, 815, 816, 817, 818, 891, 892 };
        public static int[] GalarFossils = { 880, 881, 882, 883 };
        public static int[] SilvallyMemory = { 0, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920 };
        public static int[] GenesectDrives = { 0, 116, 117, 118, 119 };
        public static int[] Amped = { 3, 4, 2, 8, 9, 19, 22, 11, 13, 14, 0, 6, 24 };
        public static int[] LowKey = { 1, 5, 7, 10, 12, 15, 16, 17, 18, 20, 21, 23 };
        public static readonly string[] Characteristics =
        {
            "Takes plenty of siestas",
            "Likes to thrash about",
            "Capable of taking hits",
            "Alert to sounds",
            "Mischievous",
            "Somewhat vain",
        };

        public class TCRng
        {
            private readonly int catchRng = Random.Next(101);
            private int shinyRng = Random.Next(101);
            private int eggShinyRng = Random.Next(101);
            private readonly int eggRng = Random.Next(101);
            private readonly int gmaxRng = Random.Next(101);
            private int cherishRng = Random.Next(101);
            private int speciesRng = 0;
            private PK8 catchPKM = new();
            private PK8 eggPKM = new();

            public int CatchRNG { get => catchRng; }
            public int ShinyRNG { get => shinyRng; set => shinyRng = value; }
            public int EggRNG { get => eggRng; }
            public int EggShinyRNG { get => eggShinyRng; set => eggShinyRng = value; }
            public int GmaxRNG { get => gmaxRng; }
            public int CherishRng { get => cherishRng; set => cherishRng = value; }
            public int SpeciesRNG { get => speciesRng; set => speciesRng = value; }
            public PK8 CatchPKM { get => catchPKM; set => catchPKM = value; }
            public PK8 EggPKM { get => eggPKM; set => eggPKM = value; }
        }

        public class TCUserInfoRoot
        {
            public HashSet<TCUserInfo> Users { get; set; } = new();

            public class TCUserInfo
            {
                public ulong UserID { get; set; }
                public int CatchCount { get; set; }
                public Daycare1 Daycare1 { get; set; } = new();
                public Daycare2 Daycare2 { get; set; } = new();
                public string OTName { get; set; } = "";
                public string OTGender { get; set; } = "";
                public int TID { get; set; }
                public int SID { get; set; }
                public string Language { get; set; } = "";
                public HashSet<int> Dex { get; set; } = new();
                public int DexCompletionCount { get; set; }
                public HashSet<int> Favorites { get; set; } = new();
                public HashSet<Catch> Catches { get; set; } = new();
            }

            public class Catch
            {
                public bool Shiny { get; set; }
                public int ID { get; set; }
                public string Ball { get; set; } = "None";
                public string Species { get; set; } = "None";
                public string Form { get; set; } = "";
                public bool Egg { get; set; }
                public string Path { get; set; } = "";
                public bool Traded { get; set; }
            }

            public class Daycare1
            {
                public bool Shiny { get; set; }
                public int ID { get; set; }
                public int Species { get; set; }
                public string Form { get; set; } = "";
                public int Ball { get; set; }
            }

            public class Daycare2
            {
                public bool Shiny { get; set; }
                public int ID { get; set; }
                public int Species { get; set; }
                public string Form { get; set; } = "";
                public int Ball { get; set; }
            }
        }

        private static async Task SerializationMonitor(int interval)
        {
            var sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                if (sw.ElapsedMilliseconds / 1000 >= interval && !TCRWLockEnable)
                {
                    SerializeInfo(UserInfo, InfoPath, true);
                    sw.Restart();
                }
                else await Task.Delay(15_000).ConfigureAwait(false);
            }
        }

        public static bool ShinyLockCheck(int species, string ball, bool form)
        {
            if (ShinyLock.Contains(species))
                return true;
            else if (form && (species == (int)Species.Zapdos || species == (int)Species.Moltres || species == (int)Species.Articuno))
                return true;
            else if (ball.Contains("Beast") && (species == (int)Species.Poipole || species == (int)Species.Naganadel))
                return true;
            else return false;
        }

        public static PKM RngRoutine(PKM pkm, IBattleTemplate template, Shiny shiny)
        {
            pkm.Form = pkm.Species == (int)Species.Silvally || pkm.Species == (int)Species.Genesect ? Random.Next(pkm.PersonalInfo.FormCount) : pkm.Form;
            if (pkm.Species == (int)Species.Alcremie)
            {
                Data = pkm.Data;
                AlcremieDecoration = (uint)Random.Next(7);
                pkm = PKMConverter.GetPKMfromBytes(Data) ?? pkm;
            }
            else if (pkm.Form > 0 && pkm.Species == (int)Species.Silvally || pkm.Species == (int)Species.Genesect)
            {
                switch (pkm.Species)
                {
                    case 649: pkm.HeldItem = GenesectDrives[pkm.Form]; break;
                    case 773: pkm.HeldItem = SilvallyMemory[pkm.Form]; break;
                };
            }

            pkm.Nature = pkm.Species == (int)Species.Toxtricity && pkm.Form > 0 ? LowKey[Random.Next(LowKey.Length)] : pkm.Species == (int)Species.Toxtricity && pkm.Form == 0 ? Amped[Random.Next(Amped.Length)] : pkm.FatefulEncounter ? pkm.Nature : Random.Next(25);
            pkm.StatNature = pkm.Nature;
            pkm.Move1_PPUps = pkm.Move2_PPUps = pkm.Move3_PPUps = pkm.Move4_PPUps = 0;
            pkm.SetMaximumPPCurrent(pkm.Moves);
            pkm.ClearHyperTraining();

            var la = new LegalityAnalysis(pkm);
            var enc = la.Info.EncounterMatch;
            if (!GalarFossils.Contains(pkm.Species) && pkm.Species != 487 && !pkm.FatefulEncounter && la.Valid)
            {
                while (true)
                {
                    pkm.SetAbilityIndex(Random.Next(3));
                    if (new LegalityAnalysis(pkm).Valid)
                        break;
                }
            }

            bool goMew = pkm.Species == (int)Species.Mew && enc.Version == GameVersion.GO && pkm.IsShiny;
            pkm.IVs = goMew || pkm.FatefulEncounter ? pkm.IVs : enc.Version == GameVersion.GO ? pkm.SetRandomIVsGO() : enc is EncounterStatic8N && enc.LevelMin >= 35 ? pkm.SetRandomIVs(5) : enc is EncounterSlot8 || enc is EncounterStatic8U ? pkm.SetRandomIVs(4) : pkm.SetRandomIVs(3);
            if (enc is EncounterStatic8)
            {
                while (!new LegalityAnalysis(pkm).Valid)
                {
                    if (!SimpleEdits.TryApplyHardcodedSeedWild8((PK8)pkm, enc, pkm.IVs, shiny))
                    {
                        var criteria = EncounterCriteria.GetCriteria(template);
                        Overworld8RNG.ApplyDetails(pkm, criteria, shiny, pkm.FlawlessIVCount);
                    }
                }
            }

            var ballRng = Random.Next(3);
            if (ballRng == 2)
                BallApplicator.ApplyBallLegalByColor(pkm);
            else BallApplicator.ApplyBallLegalRandom(pkm);

            pkm = TrashBytes(pkm);
            return pkm;
        }

        public static PKM EggRngRoutine(TCUserInfoRoot.TCUserInfo info, string trainerInfo, int evo1, int evo2, bool star, bool square)
        {
            var pkm1 = PKMConverter.GetPKMfromBytes(File.ReadAllBytes(info.Catches.FirstOrDefault(x => x.ID == info.Daycare1.ID).Path));
            var pkm2 = PKMConverter.GetPKMfromBytes(File.ReadAllBytes(info.Catches.FirstOrDefault(x => x.ID == info.Daycare2.ID).Path));
            if (pkm1 == null || pkm2 == null)
                return new PK8();

            var ballRng = $"\nBall: {(Ball)Random.Next(2, 27)}";
            var ballRngDC = Random.Next(1, 3);
            bool specificEgg = (evo1 == evo2 && ValidEgg.Contains(evo1)) || ((evo1 == 132 || evo2 == 132) && (ValidEgg.Contains(evo1) || ValidEgg.Contains(evo2))) || ((evo1 == 29 || evo1 == 32) && (evo2 == 29 || evo2 == 32));
            var dittoLoc = DittoSlot(evo1, evo2);
            var speciesRng = specificEgg ? SpeciesName.GetSpeciesNameGeneration(dittoLoc == 1 ? evo2 : evo1, 2, 8) : SpeciesName.GetSpeciesNameGeneration(ValidEgg[Random.Next(ValidEgg.Length)], 2, 8);
            var speciesRngID = SpeciesName.GetSpeciesID(speciesRng);
            FormOutput(speciesRngID, 0, out string[] forms);

            if (speciesRng.Contains("Nidoran"))
                speciesRng = speciesRng.Remove(speciesRng.Length - 1);

            string formHelper = speciesRng switch
            {
                "Indeedee" => _ = specificEgg && dittoLoc == 1 ? FormOutput(876, pkm2.Form, out _) : specificEgg && dittoLoc == 2 ? FormOutput(876, pkm1.Form, out _) : FormOutput(876, Random.Next(2), out _),
                "Nidoran" => _ = specificEgg && dittoLoc == 1 ? (evo2 == 32 ? "-M" : "-F") : specificEgg && dittoLoc == 2 ? (evo1 == 32 ? "-M" : "-F") : (Random.Next(2) == 0 ? "-M" : "-F"),
                "Meowth" => _ = FormOutput(speciesRngID, specificEgg && (pkm1.Species == 863 || pkm2.Species == 863) ? 2 : specificEgg && dittoLoc == 1 ? pkm2.Form : specificEgg && dittoLoc == 2 ? pkm1.Form : Random.Next(forms.Length), out _),
                "Yamask" => FormOutput(speciesRngID, specificEgg && (pkm1.Species == 867 || pkm2.Species == 867) ? 1 : specificEgg && dittoLoc == 1 ? pkm2.Form : specificEgg && dittoLoc == 2 ? pkm1.Form : Random.Next(forms.Length), out _),
                "Zigzagoon" => _ = FormOutput(speciesRngID, specificEgg && (pkm1.Species == 862 || pkm2.Species == 862) ? 1 : specificEgg && dittoLoc == 1 ? pkm2.Form : specificEgg && dittoLoc == 2 ? pkm1.Form : Random.Next(forms.Length), out _),
                "Farfetch’d" => _ = FormOutput(speciesRngID, specificEgg && (pkm1.Species == 865 || pkm2.Species == 865) ? 1 : specificEgg && dittoLoc == 1 ? pkm2.Form : specificEgg && dittoLoc == 2 ? pkm1.Form : Random.Next(forms.Length), out _),
                //"Slowpoke" => _ = FormOutput(speciesRngID, specificEgg && (pkm1.Species == 80 && pkm1.Form > 0 || pkm2.Species == 80 && pkm2.Form > 0 || pkm1.Species == 199 && pkm1.Form > 0 || pkm2.Species == 199 && pkm2.Form > 0) ? 1 : specificEgg && dittoLoc == 1 ? pkm2.Form : specificEgg && dittoLoc == 2 ? pkm1.Form : Random.Next(forms.Length), out _),
                "Sinistea" or "Milcery" => "",
                _ => FormOutput(speciesRngID, specificEgg && pkm1.Form == pkm2.Form ? pkm1.Form : specificEgg && dittoLoc == 1 ? pkm2.Form : specificEgg && dittoLoc == 2 ? pkm1.Form : Random.Next(forms.Length), out _),
            };

            var set = new ShowdownSet($"Egg({speciesRng}{formHelper}){(ballRng.Contains("Cherish") || Pokeball.Contains(SpeciesName.GetSpeciesID(speciesRng)) ? "\nBall: Poke" : ballRng)}\n{trainerInfo}");
            var template = AutoLegalityWrapper.GetTemplate(set);
            var sav = AutoLegalityWrapper.GetTrainerInfo(8);
            var pkm = sav.GetLegal(template, out _);

            if (!specificEgg && pkm.PersonalInfo.HasForms && pkm.Species != (int)Species.Sinistea && pkm.Species != (int)Species.Indeedee)
                pkm.Form = Random.Next(pkm.PersonalInfo.FormCount);
            if (pkm.Species == (int)Species.Rotom)
                pkm.Form = 0;
            if (FormInfo.IsBattleOnlyForm(pkm.Species, pkm.Form, pkm.Format))
                pkm.Form = FormInfo.GetOutOfBattleForm(pkm.Species, pkm.Form, pkm.Format);

            if (ballRngDC == 1)
                pkm.Ball = info.Daycare1.Ball;
            else if (ballRngDC == 2)
                pkm.Ball = info.Daycare2.Ball;

            EggTrade((PK8)pkm);
            pkm.SetAbilityIndex(Random.Next(3));
            pkm.Nature = Random.Next(25);
            pkm.StatNature = pkm.Nature;
            pkm.IVs = pkm.SetRandomIVs(4);

            if (!pkm.ValidBall())
                BallApplicator.ApplyBallLegalRandom(pkm);

            if (square)
                CommonEdits.SetShiny(pkm, Shiny.AlwaysSquare);
            else if (star)
                CommonEdits.SetShiny(pkm, Shiny.AlwaysStar);

            return pkm;
        }

        public static void DittoTrade(PKM pkm)
        {
            var dittoStats = new string[] { "atk", "spe", "spa" };
            var nickname = pkm.Nickname.ToLower();
            pkm.StatNature = pkm.Nature;
            pkm.Met_Location = 162;
            pkm.Ball = 21;
            pkm.IVs = new int[] { 31, nickname.Contains(dittoStats[0]) ? 0 : 31, 31, nickname.Contains(dittoStats[1]) ? 0 : 31, nickname.Contains(dittoStats[2]) ? 0 : 31, 31 };
            pkm.ClearHyperTraining();
            _ = TrashBytes(pkm, new LegalityAnalysis(pkm));
        }

        public static void EggTrade(PK8 pk)
        {
            pk = (PK8)TrashBytes(pk);
            pk.IsNicknamed = true;
            pk.Nickname = pk.Language switch
            {
                1 => "タマゴ",
                3 => "Œuf",
                4 => "Uovo",
                5 => "Ei",
                7 => "Huevo",
                8 => "알",
                9 or 10 => "蛋",
                _ => "Egg",
            };

            pk.IsEgg = true;
            pk.Egg_Location = 60002;
            pk.MetDate = DateTime.Parse("2020/10/20");
            pk.EggMetDate = pk.MetDate;
            pk.HeldItem = 0;
            pk.CurrentLevel = 1;
            pk.EXP = 0;
            pk.DynamaxLevel = 0;
            pk.Met_Level = 1;
            pk.Met_Location = 30002;
            pk.CurrentHandler = 0;
            pk.OT_Friendship = 1;
            pk.HT_Name = "";
            pk.HT_Friendship = 0;
            pk.HT_Language = 0;
            pk.HT_Gender = 0;
            pk.HT_Memory = 0;
            pk.HT_Feeling = 0;
            pk.HT_Intensity = 0;
            pk.StatNature = pk.Nature;
            pk.EVs = new int[] { 0, 0, 0, 0, 0, 0 };
            pk.Markings = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            pk.ClearRecordFlags();
            pk.ClearRelearnMoves();
            pk.Moves = new int[] { 0, 0, 0, 0 };
            var la = new LegalityAnalysis(pk);
            pk.SetRelearnMoves(MoveSetApplicator.GetSuggestedRelearnMoves(la));
            pk.Moves = pk.RelearnMoves;
            pk.Move1_PPUps = pk.Move2_PPUps = pk.Move3_PPUps = pk.Move4_PPUps = 0;
            pk.SetMaximumPPCurrent(pk.Moves);
            pk.SetSuggestedHyperTrainingData();
            pk.SetSuggestedRibbons(la.EncounterMatch);
        }

        public static List<string> SpliceAtWord(string entry, int start, int length)
        {
            int counter = 0;
            var temp = entry.Contains(",") ? entry.Split(',').Skip(start) : entry.Split('\n').Skip(start);
            List<string> list = new();

            if (entry.Length < length)
            {
                list.Add(entry ?? "");
                return list;
            }

            foreach (var line in temp)
            {
                counter += line.Length + 2;
                if (counter < length)
                    list.Add(line.Trim());
                else break;
            }
            return list;
        }

        public static string FormOutput(int species, int form, out string[] formString)
        {
            var strings = GameInfo.GetStrings(LanguageID.English.GetLanguage2CharName());
            formString = FormConverter.GetFormList(species, strings.Types, strings.forms, GameInfo.GenderSymbolASCII, 8);
            _ = formString.Length == form && form != 0 ? form -= 1 : form;

            if (formString[form] == "Normal" || formString[form].Contains("-") && species != (int)Species.Zygarde || formString[form] == "")
                return "";
            else return "-" + formString[form];
        }

        private static int DittoSlot(int species1, int species2)
        {
            if (species1 == 132 && species2 != 132)
                return 1;
            else if (species2 == 132 && species1 != 132)
                return 2;
            else return 0;
        }

        public static void EncounterLogs(PK8 pk)
        {
            if (!File.Exists("EncounterLog.txt"))
            {
                var blank = "Total: 0 Pokémon, 0 Eggs, 0 Shiny\n--------------------------------------\n";
                File.Create("EncounterLog.txt").Close();
                File.WriteAllText("EncounterLog.txt", blank);
            }

            var content = File.ReadAllText("EncounterLog.txt").Split('\n').ToList();
            var form = FormOutput(pk.Species, pk.Form, out _);
            var speciesName = SpeciesName.GetSpeciesNameGeneration(pk.Species, pk.Language, 8) + (pk.Species == (int)Species.Sinistea ? "" : form);
            var index = content.FindIndex(2, x => x.Contains(speciesName));
            var split = index != -1 ? content[index].Split('_') : new string[] { };
            var splitTotal = content[0].Split(',');

            if (index == -1 && !speciesName.Contains("Sinistea"))
                content.Add($"{speciesName}_1_★{(pk.IsShiny ? 1 : 0)}");
            else if (index == -1 && speciesName.Contains("Sinistea"))
                content.Add($"{speciesName}_1_{(pk.Form > 0 ? 1 : 0)}_★{(pk.IsShiny ? 1 : 0)}");
            else if (index != -1 && !speciesName.Contains("Sinistea"))
                content[index] = $"{split[0]}_{int.Parse(split[1]) + 1}_{(pk.IsShiny ? "★" + (int.Parse(split[2].Replace("★", "")) + 1).ToString() : split[2])}";
            else if (index != -1 && speciesName.Contains("Sinistea"))
                content[index] = $"{split[0]}_{int.Parse(split[1]) + 1}_{(pk.Form > 0 ? (int.Parse(split[2]) + 1).ToString() : split[2])}_{(pk.IsShiny ? "★" + (int.Parse(split[3].Replace("★", "")) + 1).ToString() : split[3])}";

            content[0] = "Total: " + $"{int.Parse(splitTotal[0].Split(':')[1].Replace(" Pokémon", "")) + 1} Pokémon, " +
                (pk.IsEgg ? $"{int.Parse(splitTotal[1].Replace(" Eggs", "")) + 1} Eggs, " : splitTotal[1].Trim() + ", ") +
                (pk.IsShiny ? $"{int.Parse(splitTotal[2].Replace(" Shiny", "")) + 1} Shiny, " : splitTotal[2].Trim());
            File.WriteAllText("EncounterLog.txt", string.Join("\n", content));
        }

        public static PKM TrashBytes(PKM pkm, LegalityAnalysis? la = null)
        {
            pkm.Nickname = "KOIKOIKOIKOI";
            pkm.IsNicknamed = true;
            if (pkm.Version != (int)GameVersion.GO && !pkm.FatefulEncounter)
                pkm.MetDate = DateTime.Parse("2020/10/20");
            pkm.SetDefaultNickname(la ?? new LegalityAnalysis(pkm));
            return pkm;
        }

        public static string DexFlavor(int species)
        {
            var resourcePath = "SysBot.Pokemon.Helpers.DexFlavor.txt";
            using StreamReader reader = new(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath));
            return reader.ReadToEnd().Split('\n')[species];
        }

        public static PK8 CherishHandler(MysteryGift mg, ITrainerInfo info)
        {
            var mgPkm = mg.ConvertToPKM(info);
            mgPkm = PKMConverter.IsConvertibleToFormat(mgPkm, 8) ? PKMConverter.ConvertToType(mgPkm, typeof(PK8), out _) : mgPkm;
            if (mgPkm != null)
                mgPkm.SetHandlerandMemory(info);
            else return new();

            var la = new LegalityAnalysis(mgPkm);
            if (!la.Valid)
            {
                mgPkm.SetRandomIVs(6);
                return (PK8)AutoLegalityWrapper.GetLegal(info, new ShowdownSet(ShowdownParsing.GetShowdownText(mgPkm)), out _);
            }
            else return (PK8)mgPkm;
        }

        public static T GetRoot<T>(string file, TextReader? textReader = null) where T : new()
        {
            if (textReader == null)
            {
                if (!File.Exists(file))
                {
                    Directory.CreateDirectory(file.Split('\\')[0]);
                    File.Create(file).Close();
                }

                T root;
                using StreamReader sr = File.OpenText(file);
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    JsonSerializer serializer = new();
                    root = (T)serializer.Deserialize(reader, typeof(T));
                }
                return root ?? new();
            }
            else
            {
                JsonSerializer serializer = new();
                T root = (T)serializer.Deserialize(textReader, typeof(T));
                return root ?? new();
            }
        }

        public static async Task<TCUserInfoRoot.TCUserInfo> GetUserInfo(ulong id, int interval = 0, bool gift = false)
        {
            if (!TCInitialized)
            {
                TCInitialized = true;
                _ = Task.Run(() => SerializationMonitor(interval));
            }

            if (!gift)
                CommandInProgress.Add(id);
            else if (gift)
                GiftInProgress.Add(id);

            while (TCRWLockEnable || NewUserLock || GiftInProgress.Contains(id) && !gift || CommandInProgress.FindAll(x => x == id).Count > 1 && !gift)
                await Task.Delay(0_100).ConfigureAwait(false);

            var user = UserInfo.Users.FirstOrDefault(x => x.UserID == id);
            if (user == null)
            {
                NewUserLock = true;
                user = new TCUserInfoRoot.TCUserInfo { UserID = id };
                await UpdateUserInfo(user).ConfigureAwait(false);
            }
            return user;
        }

        public static async Task UpdateUserInfo(TCUserInfoRoot.TCUserInfo info, bool remove = true, bool gift = false)
        {
            while (TCRWLockEnable)
                await Task.Delay(0_100).ConfigureAwait(false);

            UserInfo.Users.RemoveWhere(x => x.UserID == info.UserID);
            UserInfo.Users.Add(info);
            NewUserLock = false;
            if (remove)
                CommandInProgress.RemoveAll(x => x == info.UserID);
            if (gift)
                GiftInProgress.Remove(info.UserID);
        }

        public static void SerializeInfo(object? root, string filePath, bool tc = false)
        {
            while (TCRWLockEnable && tc)
                Thread.Sleep(0_100);

            if (tc)
                TCRWLockEnable = true;

            bool success = false;
            while (!success)
            {
                try
                {
                    JsonSerializer serializer = new();
                    using StreamWriter writer = File.CreateText(filePath);
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, root);
                    success = true;
                }
                catch (IOException)
                {
                    Thread.Sleep(0_100);
                }

                if (success)
                    break;
            }

            if (tc)
                TCRWLockEnable = false;
        }

        public static void TradeStatusUpdate(string id, bool cancelled = false)
        {
            var origPath = TradeCordPath.FirstOrDefault(x => x.Contains(id));
            if (!cancelled && origPath != default)
            {
                var tradedPath = Path.Combine($"TradeCord\\Backup\\{id}", origPath.Split('\\')[2]);
                try
                {
                    File.Move(origPath, tradedPath);
                }
                catch (IOException)
                {
                    File.Move(origPath, tradedPath.Insert(tradedPath.IndexOf(".") - 1, "ex"));
                }
            }

            if (TradeCordPath.FirstOrDefault(x => x.Contains(id)) != default)
            {
                var entries = TradeCordPath.FindAll(x => x.Contains(id));
                for (int i = 0; i < entries.Count; i++)
                    TradeCordPath.Remove(entries[i]);
            }
        }

        public static string PokeImg(PKM pkm, bool canGmax)
        {
            var alcremieDeco = (uint)(pkm.Species == (int)Species.Alcremie ? pkm.Data[0xE4] : 0);
            bool md = false;
            bool fd = false;
            var baseLink = "https://projectpokemon.org/images/sprites-models/homeimg/poke_capture_0001_000_mf_n_00000000_f_n.png".Split('_');
            if (GenderDependent.Contains(pkm.Species) && !canGmax && pkm.Form == 0)
            {
                if (pkm.Gender == 0)
                    md = true;
                else fd = true;
            }

            baseLink[2] = pkm.Species < 10 ? $"000{pkm.Species}" : pkm.Species < 100 && pkm.Species > 9 ? $"00{pkm.Species}" : $"0{pkm.Species}";
            baseLink[3] = pkm.Form < 10 ? $"00{pkm.Form}" : $"0{pkm.Form}";
            baseLink[4] = pkm.PersonalInfo.OnlyFemale ? "fo" : pkm.PersonalInfo.OnlyMale ? "mo" : pkm.PersonalInfo.Genderless ? "uk" : fd ? "fd" : md ? "md" : "mf";
            baseLink[5] = canGmax ? "g" : "n";
            baseLink[6] = "0000000" + (pkm.Species == (int)Species.Alcremie ? alcremieDeco : 0);
            baseLink[8] = pkm.IsShiny ? "r.png" : "n.png";
            return string.Join("_", baseLink);
        }
    }
}