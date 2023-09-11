using System;
using System.Collections.Generic;
using Extensions;
using Extensions.Valheim;

namespace Achievements;

[Serializable]
public class Achievement
{
    private const string IsCompletedKeyword = "Achievements_Completed";
    public string name;
    public AchievementCompleteWay completeWay;
    public List<(AchievementCompleteWay, string)> requirements = new();
    public bool manualCompletion;

    public Achievement(string name, bool manualCompletion = false)
    {
        this.name = name;
        Achs.RegisterAchievement(this);
        this.manualCompletion = manualCompletion;
    }

    public static implicit operator bool(Achievement achievement) { return achievement != null; }

    public bool MatchRequirments()
    {
        if (manualCompletion) return false;

        foreach (var (achievementCompleteWay, name) in requirements)
            if (!Achs.HasProgress(achievementCompleteWay, name))
                return false;

        return true;
    }

    public void Complete()
    {
        if (IsComplete()) return;
        ModBase.Debug($"Achievement {name} completed");
        Achs.CompletedAchievements.TryAdd(name);
        Achs.SaveCompletedAchievements();
        Achs.CreateAchievementUIElement(this);
    }

    public bool IsComplete() { return Achs.CompletedAchievements.Contains(name); }

    public void Reset()
    {
        if (Achs.CompletedAchievements.Contains(name))
            Achs.CompletedAchievements.Remove(name);
    }

    public string GetName() => $"$Achievement_{name}_name".Localize();
    public string GetDescription() => $"$Achievement_{name}_desc".Localize();

    public override string ToString() => $"Name: {name}";
}