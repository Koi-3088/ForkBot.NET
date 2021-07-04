﻿namespace SysBot.Pokemon
{
    public enum BoolMode
    {
        DexRecSkipper,
        DexRecSpeciesInjector,
        DexRecLocationInjector,
        ResetLegendaryLairFlags,
    }
    public enum DexRecLoc : ulong
    {
        None = 0x00,
        SlumberingWeald = 0x8F67CD45F405D66E,
        Route1 = 0x078BC1FF1A657844,
        Route2 = 0x10355EFF1F4DB0B5,
        Route3 = 0x194B97FF2492111A,
        Route4 = 0xDBCF5CFF0180B073,
        Route5 = 0xE4E595FF06C510D8,
        Route6 = 0xEDFC32FF0C0A1B29,
        Route7 = 0xF55F6BFF0FDCE70E,
        Route8 = 0x4BFDF9FF40EC6CFC,
        Route9 = 0x4BFDFCFF40EC7215,
        Route10 = 0xB332930807F9D48A,
        Motostoke = 0xE0D6E5E78C91F4A7,
        MotostokeRiverbank = 0x776E81717EAA799D,
        MotostrokeOutskirts = 0x7D3B7745E97D4538,
        GalarMine = 0x75D83E45E5AA7953,
        GalarMine2 = 0x7D3B7A45E97D4A51,
        Hulbury = 0x1C7150C0594994E5,
        GlimwoodTangle = 0xA88AC04602050B95,
        GiantsSeat = 0x77676D717EA438F3,
        GiantsMirror = 0x7771E7717EAD5CC6,
        GiantsBed = 0x87E14E7187BC21DA,
        GiantsCap = 0x7771E9717EAD602C,
        GiantsFoot = 0x87E4507187BE5B17,
        BridgeField = 0x776E7E717EAA7484,
        HammerlockeHills = 0x7771EA717EAD61DF,
        WatchtowerRuins = 0x776778717EA44BA4,
        AxewsEye = 0x77677B717EA450BD,
        EastLakeAxewell = 0x776779717EA44D57,
        WestLakeAxewell = 0x77677A717EA44F0A,
        DappledGrove = 0x776777717EA449F1,
        FieldsOfHonor = 0x908A64718CA374E6,
        TrainingLowlands = 0x908A6C718CA3827E,
        CourageousCavern = 0x908A66718CA3784C,
        SoothingWetlands = 0x908A63718CA37333,
        ForestOfFocus = 0x908A62718CA37180,
        WorkoutSea = 0x909170718CA9A7F8,
        SteppingStoneSea = 0x909173718CA9AD11,
        SlipperySlope = 0x87E14B7187BC1CC1,
        FrigidSea = 0x87E4527187BE5E7D,
        RoaringSeaCaves = 0x87E44F7187BE5964,
        FrostPointField = 0x87E1497187BC195B,
        SnowslideSlope = 0x87E14C7187BC1E74,
        OldCemetery = 0x87E14F7187BC238D,
        TunnelToTheTop = 0x87E14D7187BC2027,
        PathToThePeak = 0x87E1427187BC0D76,
        BallimereLake = 0x87DA3F7187B5E9AF,
        BrawlersCave = 0x908A68718CA37BB2,
        RollingFields = 0x776776717EA4483E,
        NorthLakeMiloch = 0x776AFA717EA75E61,
        SouthLakeMiloch = 0x77676C717EA43740,
        ChallengeBeach = 0x908A69718CA37D65,
        LakeOfOutrage = 0x7771EC717EAD6545,
        LakeSideCave = 0x87DA407187B5EB62,
        PotBottomDesert = 0x908760718CA13843,
        StonyWilderness = 0x7771E5717EAD5960,
        DustyBowl = 0x7771E8717EAD5E79,
        ChallengeRoad = 0x908A67718CA379FF,
        WarmUpTunnel = 0x90875F718CA13690,
        HoneyCalmIsland = 0x908DEC718CA691D5,
        InsularSea = 0x909172718CA9AB5E,
        LoopLagoon = 0x908A6D718CA38431,
        ThreePointPass = 0x87E4517187BE5CCA,
    }
    public enum Regis : ushort
    {
        Regirock = 377,
        Regice = 378,
        Registeel = 379,
        Regieleki = 894,
        Regidrago = 895,
    }
    public enum Birds : ushort
    {
        Moltres = 146,
        Zapdos = 145,
        Articuno = 144,
    }
    public enum OverworldBeasts : ushort
    {
        Cobalion = 638,
        Terrakion = 639,
        Virizion = 640,
        Keldeo = 647,
        Spiritomb = 442,
        Wailord = 321,
    }
    public enum DexRecSpecies : ushort
    {
        None = 0,
        Caterpie = 10,
        Metapod = 11,
        Butterfree = 12,
        Pikachu = 25,
        Raichu = 26,
        Sandshrew = 27,
        Sandslash = 28,
        NidoranF = 29,
        Nidorina = 30,
        Nidoqueen = 31,
        NidoranM = 32,
        Nidorino = 33,
        Nidoking = 34,
        Clefairy = 35,
        Clefable = 36,
        Vulpix = 37,
        Ninetales = 38,
        Jigglypuff = 39,
        Wigglytuff = 40,
        Zubat = 41,
        Golbat = 42,
        Oddish = 43,
        Gloom = 44,
        Vileplume = 45,
        Diglett = 50,
        Dugtrio = 51,
        Meowth = 52,
        Psyduck = 54,
        Golduck = 55,
        Growlithe = 58,
        Arcanine = 59,
        Poliwag = 60,
        Poliwhirl = 61,
        Poliwrath = 62,
        Abra = 63,
        Kadabra = 64,
        Alakazam = 65,
        Machop = 66,
        Machoke = 67,
        Machamp = 68,
        Tentacool = 72,
        Tentacruel = 73,
        Ponyta = 77,
        Rapidash = 78,
        Slowpoke = 79,
        Magnemite = 81,
        Magneton = 82,
        Farfetchd = 83,
        Shellder = 90,
        Cloyster = 91,
        Gastly = 92,
        Haunter = 93,
        Gengar = 94,
        Onix = 95,
        Krabby = 98,
        Kingler = 99,
        Exeggcute = 102,
        Exeggutor = 103,
        Cubone = 104,
        Marowak = 105,
        Hitmonlee = 106,
        Hitmonchan = 107,
        Lickitung = 108,
        Koffing = 109,
        Weezing = 110,
        Rhyhorn = 111,
        Rhydon = 112,
        Chansey = 113,
        Tangela = 114,
        Kangaskhan = 115,
        Horsea = 116,
        Seadra = 117,
        Goldeen = 118,
        Seaking = 119,
        Staryu = 120,
        Starmie = 121,
        MrMime = 122,
        Scyther = 123,
        Jynx = 124,
        Electabuzz = 125,
        Magmar = 126,
        Pinsir = 127,
        Tauros = 128,
        Magikarp = 129,
        Gyarados = 130,
        Lapras = 131,
        Ditto = 132,
        Eevee = 133,
        Vaporeon = 134,
        Jolteon = 135,
        Flareon = 136,
        Porygon = 137,
        Omanyte = 138,
        Omastar = 139,
        Kabuto = 140,
        Kabutops = 141,
        Aerodactyl = 142,
        Snorlax = 143,
        Dratini = 147,
        Dragonair = 148,
        Dragonite = 149,
        Hoothoot = 163,
        Noctowl = 164,
        Chinchou = 170,
        Lanturn = 171,
        Pichu = 172,
        Cleffa = 173,
        Igglybuff = 174,
        Togepi = 175,
        Togetic = 176,
        Natu = 177,
        Xatu = 178,
        Bellossom = 182,
        Marill = 183,
        Azumarill = 184,
        Sudowoodo = 185,
        Politoed = 186,
        Wooper = 194,
        Quagsire = 195,
        Espeon = 196,
        Umbreon = 197,
        Wobbuffet = 202,
        Dunsparce = 206,
        Steelix = 208,
        Qwilfish = 211,
        Scizor = 212,
        Shuckle = 213,
        Heracross = 214,
        Sneasel = 215,
        Swinub = 220,
        Piloswine = 221,
        Corsola = 222,
        Remoraid = 223,
        Octillery = 224,
        Delibird = 225,
        Mantine = 226,
        Skarmory = 227,
        Kingdra = 230,
        Tyrogue = 236,
        Hitmontop = 237,
        Smoochum = 238,
        Elekid = 239,
        Magby = 240,
        Miltank = 241,
        Blissey = 242,
        Larvitar = 246,
        Pupitar = 247,
        Tyranitar = 248,
        Zigzagoon = 263,
        Linoone = 264,       
        Lotad = 270,
        Lombre = 271,
        Ludicolo = 272,
        Seedot = 273,
        Nuzleaf = 274,
        Shiftry = 275,
        Wingull = 278,
        Pelipper = 279,
        Ralts = 280,
        Kirlia = 281,
        Gardevoir = 282,
        Nincada = 290,
        Ninjask = 291,
        Whismur = 293,
        Loudred = 294,
        Exploud = 295,
        Azurill = 298,
        Sableye = 302,
        Mawile = 303,
        Aron = 304,
        Lairon = 305,
        Aggron = 306,
        Electrike = 309,
        Manectric = 310,
        Roselia = 315,
        Swalot = 317,
        Carvanha = 318,
        Sharpedo = 319,
        Wailmer = 320,
        Wailord = 321,
        Torkoal = 324,
        Trapinch = 328,
        Flygon = 330,
        Swablu = 333,
        Altaria = 334,
        Lunatone = 337,
        Solrock = 338,
        Barboach = 339,
        Whiscash = 340,
        Corphish = 341,
        Crawdaunt = 342,
        Baltoy = 343,
        Claydol = 344,
        Lileep = 345,
        Anorith = 347,
        Armaldo = 348,
        Feebas = 349,
        Milotic = 350,
        Duskull = 355,
        Dusclops = 356,
        Absol = 359,
        Wynaut = 360,
        Snorunt = 361,
        Glalie = 362,
        Spheal = 363,
        Sealeo = 364,
        Walrein = 365,
        Relicanth = 369,
        Bagon = 371,
        Salamence = 373,
        Beldum = 374,
        Metang = 375,
        Metagross = 376,
        Shinx = 403,
        Luxio = 404,
        Luxray = 405,
        Budew = 406,
        Roserade = 407,
        Combee = 415,
        Vespiquen = 416,
        Cherubi = 420,
        Cherrim = 421,
        Shellos = 422,
        Gastrodon = 423,
        Drifloon = 425,
        Drifblim = 426,
        Buneary = 427,
        Lopunny = 428,
        Glameow = 431,
        Purugly = 432,
        Stunky = 434,
        Skuntank = 435,
        Bronzor = 436,
        Bronzong = 437,
        Bonsly = 438,
        MimeJr = 439,
        Happiny = 440,
        Gible = 443,
        Garchomp = 445,
        Munchlax = 446,
        Riolu = 447,
        Lucario = 448,
        Hippopotas = 449,
        Hippowdon = 450,
        Skorupi = 451,
        Drapion = 452,
        Croagunk = 453,
        Toxicroak = 454,
        Mantyke = 458,
        Snover = 459,
        Abomasnow = 460,
        Weavile = 461,
        Magnezone = 462,
        Lickilicky = 463,
        Rhyperior = 464,
        Tangrowth = 465,
        Electivire = 466,
        Magmortar = 467,
        Togekiss = 468,
        Leafeon = 470,
        Glaceon = 471,
        Mamoswine = 473,
        Gallade = 475,
        Dusknoir = 477,
        Froslass = 478,
        Rotom = 479,
        Lillipup = 506,
        Herdier = 507,
        Stoutland = 508,
        Purrloin = 509,
        Liepard = 510,
        Munna = 517,
        Musharna = 518,
        Pidove = 519,
        Tranquill = 520,
        Unfezant = 521,
        Roggenrola = 524,
        Boldore = 525,
        Gigalith = 526,
        Woobat = 527,
        Swoobat = 528,
        Drilbur = 529,
        Excadrill = 530,
        Audino = 531,
        Timburr = 532,
        Gurdurr = 533,
        Conkeldurr = 534,
        Tympole = 535,
        Palpitoad = 536,
        Seismitoad = 537,
        Throh = 538,
        Sawk = 539,
        Venipede = 543,
        Whirlipede = 544,
        Scolipede = 545,
        Cottonee = 546,
        Whimsicott = 547,
        Petilil = 548,
        Lilligant = 549,
        Basculin = 550,
        Sandile = 551,
        Krokorok = 552,
        Krookodile = 553,
        Darumaka = 554,
        Darmanitan = 555,
        Maractus = 556,
        Dwebble = 557,
        Crustle = 558,
        Scraggy = 559,
        Scrafty = 560,
        Sigilyph = 561,
        Yamask = 562,
        Cofagrigus = 563,
        Tirtouga = 564,
        Carracosta = 565,
        Archen = 566,
        Archeops = 567,
        Trubbish = 568,
        Garbodor = 569,
        Zorua = 570,
        Zoroark = 571,
        Minccino = 572,
        Cinccino = 573,
        Gothita = 574,
        Gothorita = 575,
        Gothitelle = 576,
        Solosis = 577,
        Duosion = 578,
        Reuniclus = 579,
        Vanillite = 582,
        Vanillish = 583,
        Vanilluxe = 584,
        Emolga = 587,
        Karrablast = 588,
        Escavalier = 589,
        Foongus = 590,
        Amoonguss = 591,
        Frillish = 592,
        Jellicent = 593,
        Joltik = 595,
        Galvantula = 596,
        Ferroseed = 597,
        Ferrothorn = 598,
        Klink = 599,
        Klang = 600,
        Klinklang = 601,
        Elgyem = 605,
        Beheeyem = 606,
        Litwick = 607,
        Lampent = 608,
        Chandelure = 609,
        Axew = 610,
        Fraxure = 611,
        Haxorus = 612,
        Cubchoo = 613,
        Beartic = 614,
        Cryogonal = 615,
        Shelmet = 616,
        Accelgor = 617,
        Stunfisk = 618,
        Mienfoo = 619,
        Mienshao = 620,
        Druddigon = 621,
        Golett = 622,
        Golurk = 623,
        Pawniard = 624,
        Bisharp = 625,
        Bouffalant = 626,
        Rufflet = 627,
        Braviary = 628,
        Vullaby = 629,
        Mandibuzz = 630,
        Heatmor = 631,
        Durant = 632,
        Deino = 633,
        Zweilous = 634,
        Hydreigon = 635,
        Larvesta = 636,
        Volcarona = 637,        
        Bunnelby = 659,
        Diggersby = 660,
        Fletchling = 661,
        Fletchinder = 662,
        Talonflame = 663,
        Pancham = 674,
        Pangoro = 675,
        Espurr = 677,
        Meowstic = 678,
        Honedge = 679,
        Doublade = 680,
        Aegislash = 681,
        Spritzee = 682,
        Aromatisse = 683,
        Swirlix = 684,
        Slurpuff = 685,
        Inkay = 686,
        Malamar = 687,
        Binacle = 688,
        Barbaracle = 689,
        Skrelp = 690,
        Dragalge = 691,
        Clauncher = 692,
        Clawitzer = 693,
        Helioptile = 694,
        Heliolisk = 695,
        Tyrunt = 696,
        Tyrantrum = 697,
        Amaura = 698,
        Aurorus = 699,
        Sylveon = 700,
        Hawlucha = 701,
        Dedenne = 702,
        Carbink = 703,
        Goomy = 704,
        Sliggoo = 705,
        Klefki = 707,
        Phantump = 708,
        Trevenant = 709,
        Pumpkaboo = 710,
        Gourgeist = 711,
        Bergmite = 712,
        Avalugg = 713,
        Noibat = 714,
        Noivern = 715,        
        Grubbin = 736,
        Charjabug = 737,
        Vikavolt = 738,
        Cutiefly = 742,
        Ribombee = 743,
        Rockruff = 744,
        Lycanroc = 745,
        Wishiwashi = 746,
        Mareanie = 747,
        Toxapex = 748,
        Mudbray = 749,
        Mudsdale = 750,
        Dewpider = 751,
        Araquanid = 752,
        Fomantis = 753,
        Lurantis = 754,
        Morelull = 755,
        Shiinotic = 756,
        Salandit = 757,
        Salazzle = 758,
        Stufful = 759,
        Bewear = 760,
        Bounsweet = 761,
        Steenee = 762,
        Tsareena = 763,
        Comfey = 764,
        Oranguru = 765,
        Passimian = 766,
        Wimpod = 767,
        Golisopod = 768,
        Sandygast = 769,
        Palossand = 770,
        Pyukumuku = 771,
        Turtonator = 776,
        Togedemaru = 777,
        Mimikyu = 778,
        Drampa = 780,
        Dhelmise = 781,
        Jangmoo = 782,
        Hakamoo = 783,
        Kommoo = 784,        
        Skwovet = 819,
        Greedent = 820,
        Rookidee = 821,
        Corvisquire = 822,
        Corviknight = 823,
        Blipbug = 824,
        Dottler = 825,
        Orbeetle = 826,
        Nickit = 827,
        Thievul = 828,
        Gossifleur = 829,
        Eldegoss = 830,
        Wooloo = 831,
        Dubwool = 832,
        Chewtle = 833,
        Drednaw = 834,
        Yamper = 835,
        Boltund = 836,
        Rolycoly = 837,
        Carkol = 838,
        Coalossal = 839,
        Applin = 840,
        Flapple = 841,
        Appletun = 842,
        Silicobra = 843,
        Sandaconda = 844,
        Cramorant = 845,
        Arrokuda = 846,
        Barraskewda = 847,
        Toxel = 848,
        Toxtricity = 849,
        Sizzlipede = 850,
        Centiskorch = 851,
        Clobbopus = 852,
        Grapploct = 853,
        Sinistea = 854,
        Polteageist = 855,
        Hatenna = 856,
        Hattrem = 857,
        Hatterene = 858,
        Impidimp = 859,
        Morgrem = 860,
        Grimmsnarl = 861,
        Obstagoon = 862,
        Perrserker = 863,
        Cursola = 864,
        Sirfetchd = 865,
        MrRime = 866,
        Runerigus = 867,
        Milcery = 868,
        Alcremie = 869,
        Falinks = 870,
        Pincurchin = 871,
        Snom = 872,
        Frosmoth = 873,
        Stonjourner = 874,
        Eiscue = 875,
        IndeedeeM = 876,
        IndeedeeF = 876,
        Morpeko = 877,
        Cufant = 878,
        Copperajah = 879,        
        Duraludon = 884,
        Dreepy = 885,
        Drakloak = 886,
        Dragapult = 887,
    }    
}