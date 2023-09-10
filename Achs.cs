using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Extensions.Valheim.ModBase;

namespace Achievements;

public static class Achs
{
    public const string AC_PROS = nameof(AchievementsProgress);
    public const string AC_COMPL = nameof(CompletedAchievements);
    public static List<string> CompletedAchievements = new();

    static Achs()
    {
        CompletedAchievements = PlayerPrefs.GetString(AC_COMPL).Split_();
        AchievementsProgress = PlayerPrefs.GetString(AC_PROS).Split_();
    }

    public static List<string> AchievementsProgress { get; private set; } = new();

    public static List<Achievement> AllAchievements { get; } = new();

    public static async void SaveCompletedAchievements()
    {
        PlayerPrefs.SetString(AC_COMPL, CompletedAchievements.GetString());
    }

    public static async void SaveAchievementsProgress() { PlayerPrefs.SetString(AC_PROS, AchievementsProgress.GetString()); }

    public static void AddProgress(AchievementCompleteWay completeWay, string progressKey)
    {
        switch (completeWay)
        {
            case AchievementCompleteWay.KnowItem:
                progressKey = $"HasItem_{progressKey}";
                break;
            case AchievementCompleteWay.UsedItem:
                progressKey = $"UsedItem_{progressKey}";
                break;
        }
        
        if (AchievementsProgress.Contains(progressKey)) return;
        AchievementsProgress.Add(progressKey);
        SaveAchievementsProgress();
    }

    public static void RegisterAchievement(Achievement achievement)
    {
        if (AllAchievements.Contains(achievement)) return;
        AllAchievements.Add(achievement);
    }

    public static bool UnRegisterAchievement(Achievement achievement)
    {
        if (AllAchievements.Any(x => x.name == achievement.name))
        {
            AllAchievements.Remove(AllAchievements.Find(x => x.name == achievement.name));
            return true;
        }

        return false;
    }

    public static Achievement GetAchievement(string name) { return AllAchievements.Find(x => x.name == name); }

    public static async void CheckAllAchievements()
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        AllAchievements.ForEach(x => TryCompleteAchievement(x));
    }

    public static bool CanCompleteAchievement(string name)
    {
        var achievement = GetAchievement(name);
        if (!achievement) return false;
        return achievement.MatchRequirments();
    }

    public static bool TryCompleteAchievement(Achievement achievement)
    {
        if (achievement.IsComplete()) return false;
        if (!achievement.MatchRequirments()) return false;
        achievement.Complete();
        return true;
    }

    public static bool TryCompleteAchievement(string name)
    {
        var achievement = GetAchievement(name);
        if (!achievement) return false;
        return TryCompleteAchievement(achievement);
    }

    public static void ResetAllAchievements()
    {
        PlayerPrefs.DeleteKey(AC_COMPL);
        PlayerPrefs.DeleteKey(AC_PROS);
        AchievementsProgress.Clear();
        CompletedAchievements.Clear();
        Debug("Achievements reseted");
    }

    public static bool HasProgress(AchievementCompleteWay completeWay, string name)
    {
        var key = string.Empty;
        switch (completeWay)
        {
            case AchievementCompleteWay.KnowItem:
                key = $"HasItem_{name}";
                break;
            case AchievementCompleteWay.UsedItem:
                key = $"UsedItem_{name}";
                break;
        }

        return AchievementsProgress.Contains(key);
    }
}