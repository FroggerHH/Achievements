using System.Linq;
using Extensions;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class TakeASleep
{
    [HarmonyPatch(typeof(Bed), nameof(Bed.Interact))] [HarmonyPostfix]
    private static void InventoryChanged(Bed __instance, Humanoid human, bool repeat, bool alt)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || human != Player.m_localPlayer) return;
        if(__instance.GetOwner() == 0L || !__instance.IsMine() || !__instance.IsCurrent()) return;
        if(__instance.GetPrefabName() != "piece_bed02") return;
        
        Achs.GetAchievement("TakeASleep").Complete();
    }
}