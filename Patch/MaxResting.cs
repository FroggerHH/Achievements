using System;
using System.Linq;
using Extensions.Valheim;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class MaxResting
{
    [HarmonyPatch(typeof(SE_Rested), nameof(SE_Rested.CalculateComfortLevel), new Type[] { typeof(Player) })]
    [HarmonyPostfix]
    private static void CalculateComfortLevel(ref int __result, Player player)
    {
        if (__result < 18) return;
        if (!Player.m_localPlayer || player != Player.m_localPlayer) return;
        if (SceneManager.GetActiveScene().name != "main") return;

        Achs.GetAchievement("MaxResting").Complete();
    }
}