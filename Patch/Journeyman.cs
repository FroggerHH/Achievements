using System.Linq;
using Extensions.Valheim;
using HarmonyLib;
using UnityEngine.SceneManagement;
using static Heightmap;
using static Heightmap.Biome;

namespace Achievements;

[HarmonyPatch]
public class Journeyman
{
    [HarmonyPatch(typeof(Player), nameof(Player.AddKnownBiome))] [HarmonyPostfix]
    private static void AddKnownBiome(Player __instance)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || __instance != Player.m_localPlayer) return;
        if (!KnowBiome(None) || !KnowBiome(Meadows) ||
            !KnowBiome(Swamp) || !KnowBiome(Mountain) ||
            !KnowBiome(BlackForest) || !KnowBiome(Plains) ||
            !KnowBiome(AshLands) || !KnowBiome(DeepNorth) ||
            !KnowBiome(Ocean) || !KnowBiome(Mistlands)) return;

        Achs.GetAchievement(nameof(Journeyman)).Complete();
    }

    private static bool KnowBiome(Biome biome) => Player.m_localPlayer.m_knownBiome.Contains(biome);
}