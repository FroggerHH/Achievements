using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Extensions.Valheim.ModBase;
using static UnityEngine.Object;

namespace Achievements;

public static class Achs
{
    public const string AC_PROS = nameof(AchievementsProgress);
    public const string AC_COMPL = nameof(CompletedAchievements);
    public static List<string> CompletedAchievements = new();
    private static GameObject achievementsUI;
    private static VerticalLayoutGroup achievementsGroup;
    private static GridLayoutGroup achievementsMenuHolder;
    private static GameObject achievementsMenu;
    private static GameObject achievementPopUp;
    private static GameObject menuAchievement;
    private static Transform startAchievementPosition;
    private static readonly float achievementMovingDuration = 0.5f;
    private static readonly float achievementLifeDuration = 2;

    static Achs()
    {
        CompletedAchievements = PlayerPrefs.GetString(AC_COMPL).Split_();
        AchievementsProgress = PlayerPrefs.GetString(AC_PROS).Split_();
    }

    public static List<string> AchievementsProgress { get; private set; } = new();

    public static List<Achievement> AllAchievements { get; } = new();

    public static async void InitAssets()
    {
        var bundle = Utilities.GetAssetBundleFromResources("achievements");
        achievementsUI = bundle.LoadAsset<GameObject>("Achievements_UI");
        achievementPopUp = bundle.LoadAsset<GameObject>("AchievementPopUp");
        menuAchievement = bundle.LoadAsset<GameObject>("MenuAchievement");
        bundle.LoadAsset<Material>("litpanel").shader = Shader.Find("Custom/LitGui");
        bundle.Unload(false);

        achievementsUI = Instantiate(achievementsUI);
        DontDestroyOnLoad(achievementsUI);
        achievementsGroup = achievementsUI.transform.Find("Group").GetComponent<VerticalLayoutGroup>();
        achievementsMenu = achievementsUI.transform.Find("Menu").gameObject;
        startAchievementPosition = achievementsUI.transform.Find("StartPos");
        achievementsMenuHolder = achievementsUI.transform.FindChildByName("Achievements_Menu_Holder")
            .GetComponent<GridLayoutGroup>();

        achievementsMenu.transform.FindChildByName("CloseButton").GetComponent<Button>().onClick.AddListener(() =>
            ShowAchievementsMenu(false));
        achievementsMenu.transform.FindChildByName("CloseButton_InMenu").GetComponent<Button>().onClick.AddListener(
            () =>
                ShowAchievementsMenu(false));
        UpdateLayout();
        Localization.OnLanguageChange += () =>
        {
            Localization.instance.ReLocalizeAll(achievementsUI.transform);
            UpdateMenuAchievements();
        };
        Localization.instance.Localize(achievementsUI.transform);

        achievementsMenu.SetActive(true);
        UpdateMenuAchievements();
        await Task.Delay(50);
        achievementsMenu.SetActive(false);
        await Task.Delay(50);
        UpdateMenuAchievements();
        await Task.Delay(50);
        achievementsMenu.SetActive(true);
        await Task.Delay(50);
        achievementsMenu.SetActive(false);
    }

    private static void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(achievementsUI.transform as RectTransform);
    }

    private static async void UpdateMenuAchievements()
    {
        var isActive = IsMenuActive();
        achievementsMenu.SetActive(false);
        achievementsMenu.SetActive(true);
        achievementsMenu.SetActive(false);
        achievementsMenu.SetActive(true);
        achievementsMenu.SetActive(isActive);

        for (var i = 0; i < achievementsMenuHolder.transform.childCount; i++)
            Destroy(achievementsMenuHolder.transform.GetChild(i).gameObject);

        foreach (var name in CompletedAchievements)
        {
            var achievement = GetAchievement(name);
            if (!achievement)
            {
                DebugWarning("Found unknown achievement " + name);
                continue;
            }

            var achievementObject = Instantiate(menuAchievement, achievementsMenuHolder.transform).transform;
            achievementObject.Find("Name").GetComponent<Text>().text = achievement.GetName();
            achievementObject.Find("Description").GetComponent<Text>().text = achievement.GetDescription();
        }

        UpdateLayout();
    }

    public static async Task CreateAchievementUIElement(Achievement achievement)
    {
        var go = Instantiate(achievementPopUp, achievementsGroup.transform).transform;
        go.name = $"Achievement_{achievement.name}";
        go.Find("Name").GetComponent<Text>().text = achievement.GetName();
        go.Find("Desc").GetComponent<Text>().text = achievement.GetDescription();
        LayoutRebuilder.ForceRebuildLayoutImmediate(achievementsUI.transform as RectTransform);
        Vector2 endPosition = go.transform.position;
        Vector2 startPosition = startAchievementPosition.position;
        var uiElement = go as RectTransform;

        go.transform.SetParent(achievementsUI.transform);
        go.transform.position = startAchievementPosition.position;

        float elapsedTime = 0;
        while (elapsedTime < achievementMovingDuration)
        {
            uiElement.position =
                Vector2.Lerp(startPosition, endPosition, elapsedTime / achievementMovingDuration);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        go.transform.SetParent(achievementsGroup.transform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(achievementsUI.transform as RectTransform);
        Destroy(go.gameObject, achievementLifeDuration);
    }

    public static async void SaveCompletedAchievements()
    {
        PlayerPrefs.SetString(AC_COMPL, CompletedAchievements.GetString());
    }

    public static async void SaveAchievementsProgress()
    {
        PlayerPrefs.SetString(AC_PROS, AchievementsProgress.GetString());
    }

    public static void AddProgress(AchievementCompleteWay completeWay, string progressKey)
    {
        switch (completeWay)
        {
            case AchievementCompleteWay.KnowItem:
                progressKey = $"HasItem_{progressKey}";
                break;
            case AchievementCompleteWay.UsedItem:
                progressKey = $"UsedItem_{progressKey}";
                break;
        }

        if (AchievementsProgress.Contains(progressKey)) return;
        AchievementsProgress.Add(progressKey);
        SaveAchievementsProgress();
    }

    public static void RegisterAchievement(Achievement achievement)
    {
        if (AllAchievements.Contains(achievement)) return;
        AllAchievements.Add(achievement);
    }

    public static bool UnRegisterAchievement(Achievement achievement)
    {
        if (AllAchievements.Any(x => x.name == achievement.name))
        {
            AllAchievements.Remove(AllAchievements.Find(x => x.name == achievement.name));
            return true;
        }

        return false;
    }

    public static Achievement GetAchievement(string name) { return AllAchievements.Find(x => x.name == name); }

    public static void CheckAllAchievements()
    {
        if (SceneManager.GetActiveScene().name != "main") return;
        AllAchievements.ForEach(x => TryCompleteAchievement(x));
    }

    public static bool CanCompleteAchievement(string name)
    {
        var achievement = GetAchievement(name);
        if (!achievement) return false;
        return achievement.MatchRequirments();
    }

    public static bool TryCompleteAchievement(Achievement achievement)
    {
        if (!achievement.MatchRequirments()) return false;
        achievement.Complete();
        return true;
    }

    public static bool TryCompleteAchievement(string name)
    {
        var achievement = GetAchievement(name);
        if (!achievement) return false;
        return TryCompleteAchievement(achievement);
    }

    public static void ResetAllAchievements()
    {
        PlayerPrefs.DeleteKey(AC_COMPL);
        PlayerPrefs.DeleteKey(AC_PROS);
        AchievementsProgress.Clear();
        CompletedAchievements.Clear();
        Debug("Achievements reseted");
    }

    public static bool HasProgress(AchievementCompleteWay completeWay, string name)
    {
        var key = string.Empty;
        switch (completeWay)
        {
            case AchievementCompleteWay.KnowItem:
                key = $"HasItem_{name}";
                break;
            case AchievementCompleteWay.UsedItem:
                key = $"UsedItem_{name}";
                break;
        }

        return AchievementsProgress.Contains(key);
    }

    public static void TorgeAchievementsMenu() => ShowAchievementsMenu(!achievementsMenu.activeSelf);

    public static void ShowAchievementsMenu(bool flag)
    {
        UpdateMenuAchievements();

        achievementsMenu.SetActive(true);
        achievementsMenu.SetActive(false);
        achievementsMenu.SetActive(true);
        achievementsMenu.SetActive(flag);

        UpdateLayout();
    }

    public static bool IsMenuActive() { return achievementsMenu.activeSelf; }
}