using UnityEngine.SceneManagement;

namespace Achievements;

[HarmonyPatch]
public class AllSkills60
{
    [HarmonyPatch(typeof(Skills), nameof(Skills.RaiseSkill))] [HarmonyPostfix]
    private static void RaiseSkill(Skills __instance) { Check(__instance); }

    [HarmonyPatch(typeof(Skills), nameof(Skills.CheatRaiseSkill))] [HarmonyPostfix]
    private static void CheatRaiseSkill(Skills __instance) { Check(__instance); }

    [HarmonyPatch(typeof(Skills), nameof(Skills.LowerAllSkills))] [HarmonyPostfix]
    private static void LowerAllSkills(Skills __instance) { Check(__instance); }

    private static void Check(Skills __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || __instance.m_player != Player.m_localPlayer) return;
        if (__instance.m_skillData.Any(x => x.Value.m_level < 60)) return;

        Achs.GetAchievement(nameof(AllSkills60))?.Complete();
    }
}