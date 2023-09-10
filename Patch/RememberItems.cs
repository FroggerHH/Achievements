using System.Linq;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class RememberItems
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.Changed))] [HarmonyPostfix]
    private static void InventoryChanged(Inventory __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || Player.m_localPlayer.GetInventory() != __instance) return;
        foreach (var item in __instance.GetAllItems().Select(x => x.m_shared.m_name))
            Achs.AddProgress(AchievementCompleteWay.KnowItem, item);

        Achs.SaveAchievementsProgress();
    }
}