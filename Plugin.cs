using BepInEx;
using LocalizationManager;
using static Achievements.AchievementCompleteWay;

namespace Achievements;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings", DependencyFlags.SoftDependency)]
internal class Plugin : BaseUnityPlugin
{
    internal const string ModName = "Achievements",
        ModVersion = "1.2.0",
        ModGUID = $"com.{ModAuthor}.{ModName}",
        ModAuthor = "Frogger";

    private void Awake()
    {
        CreateMod(this, ModName, ModAuthor, ModVersion);
        mod.OnConfigurationChanged += UpdateConfiguration;
        CreateAchievements();
        InvokeRepeating(nameof(CheckAllAchievements), 3, 3);
        Localizer.Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Achs.IsMenuActive()) Achs.ShowAchievementsMenu(false);
        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) Achs.TorgeAchievementsMenu();
        if (Input.GetKeyDown(KeyCode.KeypadMinus) && Input.GetKeyDown(KeyCode.LeftShift))
            Achs.AllAchievements.Where(x => x.IsComplete() == false).ToList()?.ForEach(x => x.Complete(0));
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            Achs.AllAchievements.Where(x => x.IsComplete() == false).ToList().FirstOrDefault()?.Complete(0);

        if (Input.GetKeyDown(KeyCode.KeypadEnter)) Achs.ResetAllAchievements();
    }

    private static void CreateAchievements()
    {
        new Achievement("GetWoodAndStone")
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (KnowItem, "$item_wood"),
                (KnowItem, "$item_stone")
            }
        };
        new Achievement("FineWood")
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (KnowItem, "$item_finewood")
            }
        };
        new Achievement("FireworksFestival")
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (UsedItem, "$item_fireworkrocket_blue"),
                (UsedItem, "$item_fireworkrocket_cyan"),
                (UsedItem, "$item_fireworkrocket_green"),
                (UsedItem, "$item_fireworkrocket_purple"),
                (UsedItem, "$item_fireworkrocket_red"),
                (UsedItem, "$item_fireworkrocket_white"),
                (UsedItem, "$item_fireworkrocket_yellow")
            }
        };
        var fishingMaster = new Achievement("FishingMaster");
        for (var i = 0; i < 12; i++)
            fishingMaster.requirements.Add((KnowItem, $"$animal_fish{i + 1}"));

        new Achievement("CutDownATree", true);
        new Achievement("TakeASleep", true);
        new Achievement("MaxResting", true);
        new Achievement("IntrusiveThoughtsWon", true);
        new Achievement("Horticulturist", withCustomRequirements: true)
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (CustomProgress, 1000)
            }
        };
        new Achievement("Journeyman", true);
        new Achievement("MushroomMan", withCustomRequirements: true)
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (CustomProgress, 1000)
            }
        };
        new Achievement("HeadInClouds", withCustomRequirements: true)
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (CustomProgress, 50)
            }
        };
        new Achievement(nameof(AllSkills60), true);
        new Achievement(nameof(AllSkills100), true);
        new Achievement(nameof(MasterBuilder), withCustomRequirements: true)
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (CustomProgress, 100000)
            }
        };
        new Achievement(nameof(GoldRush), withCustomRequirements: true)
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (CustomProgress, 10000)
            }
        };
        new Achievement("Wormhole")
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (PlayerStat, (PlayerStatType.PortalsUsed, 1000))
            }
        };
        new Achievement(nameof(Headhunter))
        {
            requirements = null
        };
        new Achievement(nameof(Gourmet))
        {
            requirements = null
        };
        new Achievement("Killer")
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (PlayerStat, (PlayerStatType.EnemyKills, 100000))
            }
        };
        new Achievement("WonTheGame")
        {
            requirements = new List<(AchievementCompleteWay, object)>
            {
                (KilledSpecificCreature, ("Eikthyr", 1)),
                (KilledSpecificCreature, ("gd_king", 1)),
                (KilledSpecificCreature, ("Bonemass", 1)),
                (KilledSpecificCreature, ("Dragon", 1)),
                (KilledSpecificCreature, ("GoblinKing", 1)),
                (KilledSpecificCreature, ("SeekerQueen", 1))
            }
        };
        new Achievement("Map50", true);

        //AllSkills60//TODO: Versatile warrior - Reach lvl 60 in every weapon skill
        //AllSkills100//TODO: Weapon master - Reach lvl 100 in any weapon skill
        //MasterBuilder//TODO: Master builder - Build 200.000 pieces
        //GoldRush//TODO: GoldRush - потратить 1000 монет
        //Wormhole//TODO: Wormhole - use teleporters  1000 times
        //Headhunter//TODO: Headhunter - Collect the heads of all creatures.
        //Gourmet//TODO: Valhalla Gourmet - Prepare and eat one of each type of food.
        //Killer//TODO: Killer - kill 100000
        //WonTheGame//TODO: WonTheGame - Kill all bosses
        //Map50//TODO: Reborn Geographer - Explore 70% of the world map.
        //TODO: Marksmanship Mastery - Kill 100 creatures using a bow or crossbow.
        //TODO: Odin's Pride - Defeat each of the bosses without dying during the battle.
        //TODO: Firebrand Viking - Incinerate 1000 enemies using a fire staff or fire arrows.
        //TODO: Legendary Blacksmith - Craft and upgrade every type of weapon and armor in the game to maximum level.
        //TODO: Master Crossbowman - Defeat each boss in the game using only a crossbow.
        //TODO: Cool-headed Viking - Traverse each biome without wearing armor and without using mead, even in extreme conditions.
        //TODO: Night owl - Doesn't sleep for 100 in-game days
        //TODO: Like a dragon - Gather 10000 coins
    }

    private void CheckAllAchievements()
    {
        try
        {
            Achs.CheckAllAchievements();
        }
        catch (Exception e)
        {
            Debug(e.Message);
        }
    }

    private static void UpdateConfiguration() { Debug("UpdateConfiguration"); }
}