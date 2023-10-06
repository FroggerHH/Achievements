using UnityEngine.SceneManagement;
using static ItemDrop.ItemData.ItemType;
using static Achievements.AchievementCompleteWay;

namespace Achievements;

[HarmonyPatch]
public class Gourmet
{
    [HarmonyPatch(typeof(Game), nameof(Game.Start))] [HarmonyPostfix]
    private static void InitGourmet(Game __instance)
    {
        var achievement = Achs.GetAchievement(nameof(Gourmet));
        achievement.requirements = new List<(AchievementCompleteWay, object)>();
        var allFood = ObjectDB.instance.GetAllItems(Consumable, string.Empty);
        var count = allFood.Count;
        for (var i = 0; i < count; i++)
            achievement.requirements.Add((UsedItem, allFood[i].m_itemData.m_shared.m_name));
    }

    [HarmonyPatch(typeof(Player), nameof(Player.ConsumeItem))] [HarmonyPostfix]
    private static void InitGourmet(Player __instance, ItemData item, ref bool __result)
    {
        if (!__result) return;
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || __instance != Player.m_localPlayer) return;

        Achs.AddProgress(UsedItem, item.m_shared.m_name);
    }
}