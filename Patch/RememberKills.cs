using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class RememberKills
{
    [HarmonyPatch(typeof(Character), nameof(Character.OnDestroy))] [HarmonyPostfix]
    private static void InventoryChanged(Character __instance)
    {
        if (__instance.IsPlayer()) return;
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || Player.m_localPlayer != __instance) return;

        if (__instance.m_localPlayerHasHit)
            Achs.AddCustomProgress($"KilledSpecificCreature_{__instance.GetPrefabName()}");
    }
}