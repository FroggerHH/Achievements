using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class HeadInClouds
{
    [HarmonyPatch(typeof(SEMan), nameof(SEMan.ModifyFallDamage))] [HarmonyPostfix]
    private static void ModifyFallDamage(SEMan __instance, ref float damage)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || __instance.m_character != Player.m_localPlayer) return;
        if (Player.m_localPlayer.GetHealth() > damage + 0.1f) return;

        Achs.AddCustomProgress(nameof(HeadInClouds));
    }
}