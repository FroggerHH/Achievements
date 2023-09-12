using System.Linq;
using Extensions.Valheim;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class IntrusiveThoughtsWon
{
    [HarmonyPatch(typeof(Player), nameof(Player.EdgeOfWorldKill))] [HarmonyPostfix]
    private static void AddKnownBiome(Player __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || __instance != Player.m_localPlayer) return;
        float v = Utils.DistanceXZ(Vector3.zero, __instance.transform.position);
        float l = 10420f;
        if (v <= l || __instance.transform.position.y >= ZoneSystem.instance.m_waterLevel - 40.0)
            return;

        Achs.GetAchievement(nameof(IntrusiveThoughtsWon)).Complete();
    }
}