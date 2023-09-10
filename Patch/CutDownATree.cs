using System.Linq;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class CutDownATree
{
    [HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))] [HarmonyPostfix]
    private static void InventoryChanged(TreeBase __instance, HitData hit)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || hit.GetAttacker() != Player.m_localPlayer) return;
        if (__instance.m_nview.GetZDO().GetFloat(ZDOVars.s_health) > 0) return;
        Achs.TryCompleteAchievement("CutDownATree");
        Achs.SaveCompletedAchievements();
    }
}