using System.Threading.Tasks;

namespace Achievements;

[HarmonyPatch]
public static class Map50
{
    private static decimal currentClearPixelsProcent;
    private static bool startedGlobalCheck;
    private static bool checking;
    private static Texture2D texture;
    private static int textureSize;

    [HarmonyPatch(typeof(Minimap), nameof(Minimap.LoadMapData))] [HarmonyPostfix]
    private static void LoadMapData()
    {
        if (startedGlobalCheck) return;
        StartCheck();
    }

    private static async void StartCheck()
    {
        startedGlobalCheck = true;
        while (true) await CheckExplored();
    }

    private static async Task CheckExplored()
    {
        //if (Minimap.instance) ModBase.Debug($"Started checking");
        checking = true;

        await Task.Delay(2 * 1000);
        var minimap = Minimap.instance;
        if (!minimap || !minimap.m_fogTexture) return;
        texture = minimap.m_fogTexture;
        textureSize = minimap.m_textureSize;
        var achievement = Achs.GetAchievement(nameof(Map50));
        if (achievement.IsComplete()) return;

        //ModBase.Debug($"Starting task...");
        var (allPixelsCount, clearPixels, clearPixelsProcent) = await Task.Run(() =>
        {
            var allPixels = texture.GetPixels().Select(x => x.r);
            var allPixelsCount = allPixels.Count();
            var clearPixels = allPixels.Where(x => x == 0.0f).Count();
            var clearPixelsProcent =
                Math.Min(Math.Round(clearPixels * 2 / (decimal)allPixelsCount * 100, 2), 100);

            return (allPixelsCount, clearPixels, clearPixelsProcent);
        });
        currentClearPixelsProcent = clearPixelsProcent;

        if (clearPixelsProcent >= 45) achievement?.Complete();
        texture = null;
        textureSize = 0;
        checking = false;
        await Task.Delay(2 * 1000);
        //ModBase.Debug($"Finished checking");
    }
}