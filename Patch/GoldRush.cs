using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class GoldRush
{
    [HarmonyPatch(typeof(StoreGui), nameof(StoreGui.BuySelectedItem))] [HarmonyPostfix]
    private static void ModifyFallDamage(StoreGui __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        // if (!Player.m_localPlayer || __instance != Player.m_localPlayer) return;
        if (__instance.m_selectedItem == null || !__instance.CanAfford(__instance.m_selectedItem)) return;

        Achs.AddCustomProgress(nameof(GoldRush), __instance.m_selectedItem.m_price);
    }
}