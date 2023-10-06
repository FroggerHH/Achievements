using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class MasterBuilder
{
    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))] [HarmonyPostfix]
    private static void ModifyFallDamage(Player __instance, ref bool __result)
    {
        if (!__result) return;
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || __instance != Player.m_localPlayer) return;

        Achs.AddCustomProgress(nameof(MasterBuilder));
    }
}