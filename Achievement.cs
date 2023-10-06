using System.Threading.Tasks;

namespace Achievements;

[Serializable]
public class Achievement
{
    private const string IsCompletedKeyword = "Achievements_Completed";
    public string name;
    public bool manualCompletion;
    public bool withCustomRequirements;
    public List<(AchievementCompleteWay, object)> requirements = new();

    public Achievement(string name)
    {
        this.name = name;
        Achs.RegisterAchievement(this);
        manualCompletion = false;
        withCustomRequirements = false;
    }

    public Achievement(string name, bool manualCompletion = false, bool withCustomRequirements = false) : this(name)
    {
        this.name = name;
        Achs.RegisterAchievement(this);
        this.manualCompletion = manualCompletion;
        this.withCustomRequirements = withCustomRequirements;
    }

    public static implicit operator bool(Achievement achievement) { return achievement != null; }

    public bool MatchRequirments()
    {
        if (manualCompletion) return false;
        if (IsComplete()) return true;

        return Achs.HasAllProgress(this);
    }

    public async void Complete(float delay = -1)
    {
        if (delay == -1) delay = Random.Range(1f, 3f);
        await Task.Delay(delay > 0 ? (int)delay * 1000 : 1);
        if (IsComplete()) return;
        Debug($"Achievement {name} completed");
        Achs.CompletedAchievements.TryAdd(name);
        Achs.SaveCompletedAchievements();
        Achs.CreateAchievementUIElement(this);
        Achs.UpdateMenuAchievements();
    }

    public bool IsComplete() { return Achs.CompletedAchievements.Contains(name); }

    public void Reset()
    {
        if (Achs.CompletedAchievements.Contains(name))
            Achs.CompletedAchievements.Remove(name);
    }

    public string GetName() { return $"$Achievement_{name}_name".Localize(); }

    public string GetDescription() { return $"$Achievement_{name}_desc".Localize(); }

    public override string ToString() { return $"Name: {name}"; }
}