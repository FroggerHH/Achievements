﻿using System.Collections.Generic;
using System.Linq;
using Extensions;
using Extensions.Valheim;
using HarmonyLib;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Object;

namespace Achievements;

[HarmonyPatch]
public class AddMenuInMainMenu
{
    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.SetupGui))] [HarmonyPrefix]
    private static void InventoryChanged(FejdStartup __instance)
    {
        Achs.InitAssets();
        var creditsButton = __instance.m_menuList.transform.FindChildByName("Credits");
        var newButton = Instantiate(creditsButton, creditsButton.parent).GetComponent<Button>();
        newButton.name = "Achievements";
        newButton.transform.FindChildByName("Text").GetComponentInChildren<TextMeshProUGUI>().text =
            "$Achievements_button";
        var buttons = new List<Button>(__instance.m_menuButtons);
        buttons.Add(newButton);
        __instance.m_menuButtons = buttons.ToArray();
        newButton.onClick = new();
        newButton.transform.SetSiblingIndex(3);
        newButton.onClick.AddListener(() => Achs.ShowAchievementsMenu(true));
        
        Achs.TorgeAchievementsMenu();
        Achs.TorgeAchievementsMenu();
    }
}