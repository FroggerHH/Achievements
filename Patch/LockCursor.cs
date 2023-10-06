namespace Achievements;

[HarmonyPatch]
public class LockCursor
{
    [HarmonyPatch(typeof(GameCamera), nameof(GameCamera.UpdateMouseCapture))] [HarmonyPostfix]
    private static void Postfix()
    {
        if (Achs.IsMenuActive())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = ZInput.IsMouseActive();
        }
    }

    [HarmonyPatch(typeof(Player), nameof(Character.TakeInput))] [HarmonyPostfix]
    private static void Postfix(Player __instance, ref bool __result)
    {
        if (!__instance.IsPlayer() || __instance != Player.m_localPlayer) return;
        if (Achs.IsMenuActive()) __result = false;
    }

    [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.TakeInput))] [HarmonyPostfix]
    private static void Postfix(PlayerController __instance, ref bool __result)
    {
        if (Achs.IsMenuActive()) __result = false;
    }
}