using System.Linq;
using Extensions.Valheim;
using HarmonyLib;
using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class CutDownATree
{
    [HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))] [HarmonyPostfix]
    private static void DamageTree(TreeBase __instance, HitData hit)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || hit == null || hit.GetAttacker() != Player.m_localPlayer) return;
        if(__instance.m_nview.GetZDO() != null) return;
        
        Achs.GetAchievement(nameof(CutDownATree)).Complete();
    }
}