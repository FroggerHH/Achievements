using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class FireworksFestival
{
    [HarmonyPatch(typeof(Fireplace), nameof(Fireplace.UseItem))] [HarmonyPostfix]
    private static void InventoryChanged(Fireplace __instance, Humanoid user, ItemData item)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || user != Player.m_localPlayer) return;

        Achs.AddProgress(AchievementCompleteWay.UsedItem, item.m_shared.m_name);
    }
}