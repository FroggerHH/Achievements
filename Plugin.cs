using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Extensions;
using LocalizationManager;
using UnityEngine;
using static Extensions.Valheim.ModBase;

namespace Achievements;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings", BepInDependency.DependencyFlags.SoftDependency)]
internal class Plugin : BaseUnityPlugin
{
    internal const string ModName = "Achievements",
        ModVersion = "1.0.0",
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
        if (Input.GetKeyDown(KeyCode.T)) Achs.TorgeAchievementsMenu();
        if (Input.GetKeyDown(KeyCode.I)) Achs.AllAchievements.Random().Complete();
    }

    private static void CreateAchievements()
    {
        new Achievement("GetWoodAndStone")
        {
            requirements = new List<(AchievementCompleteWay, string)>
            {
                (AchievementCompleteWay.KnowItem, "$item_wood"),
                (AchievementCompleteWay.KnowItem, "$item_stone")
            }
        };
        new Achievement("FineWood")
        {
            requirements = new List<(AchievementCompleteWay, string)>
            {
                (AchievementCompleteWay.KnowItem, "$item_finewood")
            }
        };
        new Achievement("FireworksFestival")
        {
            requirements = new()
            {
                (AchievementCompleteWay.UsedItem, "$item_fireworkrocket_blue"),
                (AchievementCompleteWay.UsedItem, "$item_fireworkrocket_cyan"),
                (AchievementCompleteWay.UsedItem, "$item_fireworkrocket_green"),
                (AchievementCompleteWay.UsedItem, "$item_fireworkrocket_purple"),
                (AchievementCompleteWay.UsedItem, "$item_fireworkrocket_red"),
                (AchievementCompleteWay.UsedItem, "$item_fireworkrocket_white"),
                (AchievementCompleteWay.UsedItem, "$item_fireworkrocket_yellow")
            }
        };
        var fishingMaster = new Achievement("FishingMaster");
        for (int i = 0; i < 12; i++)
            fishingMaster.requirements.Add((AchievementCompleteWay.KnowItem, $"$animal_fish{i + 1}"));

        new Achievement("CutDownATree", true);
        new Achievement("TakeASleep", true);
        new Achievement("MaxResting", true);
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