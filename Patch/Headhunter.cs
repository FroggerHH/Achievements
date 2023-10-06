using static ItemDrop.ItemData.ItemType;
using static Achievements.AchievementCompleteWay;

namespace Achievements;

[HarmonyPatch]
public class Headhunter
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start))] [HarmonyPostfix]
    private static void ModifyFallDamage(Game __instance)
    {
        var achievement = Achs.GetAchievement(nameof(Headhunter));
        achievement.requirements = new List<(AchievementCompleteWay, object)>();
        var trophies = ObjectDB.instance.GetAllItems(Trophy, string.Empty);
        var count = trophies.Count;
        for (var i = 0; i < count; i++)
            achievement.requirements.Add((KnowItem, trophies[i].m_itemData.m_shared.m_name));
    }
}