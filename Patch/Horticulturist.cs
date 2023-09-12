using System.Linq;
using Extensions;
using Extensions.Valheim;
using HarmonyLib;
using UnityEngine.SceneManagement;
using static Heightmap;
using static Heightmap.Biome;

namespace Achievements;

[HarmonyPatch]
public class Horticulturist
{
    private static PieceTable сultivatorPieceTable;

    [HarmonyPatch(typeof(Player), nameof(Player.Start))] [HarmonyPostfix]
    private static void AddKnownBiome(Player __instance)
    {
        if (SceneManager.GetActiveScene().name == "main") return;
            сultivatorPieceTable = ObjectDB.instance.GetItem("Cultivator").m_itemData.m_shared.m_buildPieces;
    }

    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))] [HarmonyPostfix]
    private static void PlacePiece(Player __instance, Piece piece)
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        if (!Player.m_localPlayer || __instance != Player.m_localPlayer) return;
        var prefabName = piece.GetPrefabName();
        if (prefabName == "cultivate_v2" ||
            prefabName == "replant_v2") return;
        if (сultivatorPieceTable.m_pieces.All(x => x.GetPrefabName() != prefabName))
            return;

        Achs.AddCustomProgress(nameof(Horticulturist));
    }

    private static bool KnowBiome(Biome biome) => Player.m_localPlayer.m_knownBiome.Contains(biome);
}