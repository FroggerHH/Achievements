namespace Achievements;

[HarmonyPatch]
public class Cheater
{
    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Update))] [HarmonyPostfix]
    private static void InventoryChanged() { Achs.profiles = FejdStartup.instance.m_profiles; }
}