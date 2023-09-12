using System.Linq;
using Extensions;
using Extensions.Valheim;
using HarmonyLib;
using UnityEngine.SceneManagement;
using static Heightmap;
using static Heightmap.Biome;

namespace Achievements;

[HarmonyPatch]
public class MushroomMan
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Interact))] [HarmonyPostfix]
    private static void PlacePiece(Pickable __instance, Humanoid character)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || character != Player.m_localPlayer) return;
        var prefabName = __instance.GetPrefabName();
        if (prefabName != "Pickable_Mushroom" && prefabName != "Pickable_MushroomYellow") return;

        Achs.AddCustomProgress(nameof(MushroomMan));
    }

    private static bool KnowBiome(Biome biome) => Player.m_localPlayer.m_knownBiome.Contains(biome);
}