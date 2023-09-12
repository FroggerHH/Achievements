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
    internal static GridLayoutGroup achievementsMenuHolder;
    private static GameObject achievementsMenu;
    private static GameObject achievementPopUp;
    internal static GameObject menuAchievement;
    private static Transform startAchievementPosition;
    private static readonly float achievementMovingDuration = 0.5f;
    private static readonly float achievementLifeDuration = 2;
    private static readonly float menuAchievementSpawnDuration = 0.1f;

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

        ShowAchievementsMenu(false, true);
        // achievementsMenu.SetActive(true);
        // UpdateMenuAchievements();
        // await Task.Delay(50);
        // achievementsMenu.SetActive(false);
        // await Task.Delay(50);
        // UpdateMenuAchievements();
        // await Task.Delay(50);
        // achievementsMenu.SetActive(true);
        // await Task.Delay(50);
        // achievementsMenu.SetActive(false);
    }

    internal static void UpdateLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(achievementsUI.transform as RectTransform);
    }

    internal static async void UpdateMenuAchievements()
    {
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
            achievementObject.FindChildByName("Name").GetComponent<Text>().text = achievement.GetName();
            achievementObject.FindChildByName("Description").GetComponent<Text>().text = achievement.GetDescription();
            var scalingObject = achievementObject.GetChild(0) as RectTransform;
            await Task.Yield();
            await TweetMenuAchievementSpawn(scalingObject);
        }

        if (achievementsMenuHolder.transform.childCount % 6 == 1 ||
            achievementsMenuHolder.transform.childCount == 1) await SpawnTest();
    }

    private static async Task TweetMenuAchievementSpawn(Transform scalingObject)
    {
        if (!scalingObject || !IsMenuActive()) return;
        float elapsedTime = 0;
        while (elapsedTime < achievementMovingDuration)
        {
            scalingObject.localScale =
                Vector3.Lerp(Vector3.zero, Vector3.one, elapsedTime / menuAchievementSpawnDuration);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }

    internal static async Task SpawnTest()
    {
        var test = Instantiate(menuAchievement, achievementsMenuHolder.transform);
        UpdateLayout();
        await Task.Yield();
        UpdateLayout();
        Destroy(test);
        UpdateLayout();
    }

    public static async Task CreateAchievementUIElement(Achievement achievement)
    {
        var go = Instantiate(achievementPopUp, achievementsGroup.transform).transform;
        go.name = $"Achievement_{achievement.name}";
        go.Find("Name").GetComponent<Text>().text = achievement.GetName();
        go.Find("Desc").GetComponent<Text>().text = achievement.GetDescription();
        UpdateLayout();
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

    public static void AddCustomProgress(string progressKey)
    {
        progressKey = $"CustomProgress_{progressKey}";

        if (!AchievementsProgress.Contains(progressKey)) AchievementsProgress.Add($"{progressKey} 0");
        var find = AchievementsProgress.Find(x => x.StartsWith(progressKey));
        if (!find.IsGood())
        {
            DebugError($"The progress {progressKey} is not found");
            return;
        }

        var split = find.Split(' ');
        split[1] = (int.Parse(split[1]) + 1).ToString();
        AchievementsProgress[AchievementsProgress.IndexOf(find)] = $"{split[0]} {split[1]}";
        Debug($"Progress {progressKey} added. Result is {AchievementsProgress.Find(x => x.StartsWith(progressKey))}");

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
        UpdateMenuAchievements();
    }

    public static bool HasProgress(AchievementCompleteWay completeWay, object name)
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

    public static bool HasAllProgress(Achievement achievement)
    {
        var key = string.Empty;
        foreach (var (completeWay, whatToDo) in achievement.requirements)
        {
            switch (completeWay)
            {
                case AchievementCompleteWay.KnowItem:
                    key = $"HasItem_{whatToDo}";
                    if (!AchievementsProgress.Contains(key)) return false;
                    break;
                case AchievementCompleteWay.UsedItem:
                    key = $"UsedItem_{whatToDo}";
                    if (!AchievementsProgress.Contains(key)) return false;
                    break;
                case AchievementCompleteWay.CustomProgress:
                    var find = AchievementsProgress.Find(x => x.StartsWith($"CustomProgress_{achievement.name}"));
                    if (!find.IsGood()) return false;
                    var progress = int.Parse(find.Split(' ')[1]);
                    if (progress < (int)whatToDo) return false;
                    break;
            }
        }

        return true;
    }

    public static void TorgeAchievementsMenu() => ShowAchievementsMenu(!achievementsMenu.activeSelf);

    public static async void ShowAchievementsMenu(bool flag, bool shake = true)
    {
        UpdateMenuAchievements();
        achievementsMenu.SetActive(flag);
        UpdateLayout();
    }

    public static bool IsMenuActive() { return achievementsMenu.activeSelf; }
}