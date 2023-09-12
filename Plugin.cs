using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Extensions;
using LocalizationManager;
using UnityEngine;
using static Extensions.Valheim.ModBase;
using static Achievements.AchievementCompleteWay;

namespace Achievements;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency("com.Frogger.NoUselessWarnings", BepInDependency.DependencyFlags.SoftDependency)]
internal class Plugin : BaseUnityPlugin
{
    internal const string ModName = "Achievements",
        ModVersion = "1.1.0",
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
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            Achs.AllAchievements.Where(x => x.IsComplete() == false).ToList().FirstOrDefault()?.Complete(0);

        if (Input.GetKeyDown(KeyCode.KeypadEnter)) Achs.ResetAllAchievements();
    }

    private static void CreateAchievements()
    {
        new Achievement("GetWoodAndStone")
        {
            requirements = new()
            {
                (KnowItem, "$item_wood"),
                (KnowItem, "$item_stone")
            }
        };
        new Achievement("FineWood")
        {
            requirements = new()
            {
                (KnowItem, "$item_finewood")
            }
        };
        new Achievement("FireworksFestival")
        {
            requirements = new()
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
            requirements = new()
            {
                (CustomProgress, 1000)
            }
        };
        new Achievement("Journeyman", true);
        new Achievement("MushroomMan", withCustomRequirements: true)
        {
            requirements = new()
            {
                (CustomProgress, 1000)
            }
        };
        new Achievement("HeadInClouds", withCustomRequirements: true)
        {
            requirements = new()
            {
                (CustomProgress, 50)
            }
        };

        //TODO: Weapon master - Reach lvl 100 in any weapon skill
        //TODO: Versatile warrior - Reach lvl 60 in every weapon skill
        //TODO: Master builder - Build 200.000 pieces
        //TODO: Like a dragon - Gather 10000 coins
        //TODO: Night owl - Doesn't sleep for 100 in-game days
        //TODO: Headhunter - Collect the heads of all creatures.
        //TODO: Valhalla Gourmet - Prepare and eat one of each type of food.
        //TODO: Marksmanship Mastery - Kill 100 creatures using a bow or crossbow.
        //TODO: Odin's Pride - Defeat each of the bosses without dying during the battle.
        //TODO: Firebrand Viking - Incinerate 1000 enemies using a fire staff or fire arrows.
        //TODO: Reborn Geographer - Explore 70% of the world map.
        //TODO: Legendary Blacksmith - Craft and upgrade every type of weapon and armor in the game to maximum level.
        //TODO: Master Crossbowman - Defeat each boss in the game using only a crossbow.
        //TODO: Cool-headed Viking - Traverse each biome without wearing armor and without using mead, even in extreme conditions.
        //TODO: Wormhole -  use teleporters  1000 times
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