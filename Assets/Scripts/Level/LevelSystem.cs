using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSystem : MonoBehaviour
{
    public int XPNow;
    public int Level;
    public int xpToNext;

    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject lvlWindowPrefab;

    private Slider slider;
    private TextMeshProUGUI xpText;
    public TextMeshProUGUI lvlText;
    private Image starImage;

    private static bool initialized;
    private static Dictionary<int, int> xpToNextLevel = new Dictionary<int, int>();
    private static Dictionary<int, int[]> lvlReward = new Dictionary<int, int[]>();

    private void Awake()
    {
        // Add debug checks to identify null references
        if (levelPanel == null)
        {
            Debug.LogError("levelPanel is not assigned in the inspector.");
            return;
        }

        slider = levelPanel.GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider component not found on levelPanel.");
            return;
        }

        xpText = levelPanel.transform.Find("XP text")?.GetComponent<TextMeshProUGUI>();
        if (xpText == null)
        {
            Debug.LogError("XP text object or TextMeshProUGUI component not found.");
            return;
        }

        starImage = levelPanel.transform.Find("Star")?.GetComponent<Image>();
        if (starImage == null)
        {
            Debug.LogError("Star object or Image component not found.");
            return;
        }

        lvlText = starImage.transform.GetChild(0)?.GetComponent<TextMeshProUGUI>();
        if (lvlText == null)
        {
            Debug.LogError("lvlText not found on Star object.");
            return;
        }

        if (!initialized)
        {
            Initialize();
        }

        if (!xpToNextLevel.TryGetValue(Level, out xpToNext))
        {
            Debug.LogError($"No XP to next level found for Level {Level}.");
        }
    }

    private static void Initialize()
    {
        try
        {
            string path = "levelsXP";
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
            {
                Debug.LogError("Failed to load levelsXP file.");
                return;
            }
            string[] lines = textAsset.text.Split('\n');
            xpToNextLevel = new Dictionary<int, int>(lines.Length - 1);

            for (int i = 1; i < lines.Length - 1; i++)
            {
                string[] columns = lines[i].Split(';'); // CSV file uses ';' as delimiter.

                int lvl = -1;
                int xp = -1;
                int curr1 = -1;
                int curr2 = -1;

                int.TryParse(columns[0], out lvl);
                int.TryParse(columns[1], out xp);
                int.TryParse(columns[2], out curr1);
                int.TryParse(columns[3], out curr2);

                if (lvl >= 0 && xp > 0)
                {
                    if (!xpToNextLevel.ContainsKey(lvl))
                    {
                        xpToNextLevel.Add(lvl, xp);
                        lvlReward.Add(lvl, new[] { curr1, curr2 });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error initializing level system: {ex.Message}");
        }

        initialized = true;
    }

    private void Start()
    {
        EventManager.Instance.AddListener<XPAddedGameEvent>(OnXPAdded);
        EventManager.Instance.AddListener<LevelChangedGameEvent>(OnLevelChanged);

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (xpToNext == 0) return;

        float fill = (float)XPNow / xpToNext;
        slider.value = 1 - fill; // Inverting the slider direction
        xpText.text = $"{XPNow}/{xpToNext}";
    }

    private void OnXPAdded(XPAddedGameEvent info)
    {
        XPNow += info.amount;
        UpdateUI();

        if (XPNow >= xpToNext)
        {
            Level++;
            LevelChangedGameEvent levelChange = new LevelChangedGameEvent(Level);
            EventManager.Instance.QueueEvent(levelChange);
        }
    }

    private void OnLevelChanged(LevelChangedGameEvent info)
    {
        XPNow -= xpToNext;
        xpToNext = xpToNextLevel[info.newLvl];
        lvlText.text = (info.newLvl + 1).ToString();
        UpdateUI();

        GameObject window = Instantiate(lvlWindowPrefab, GameManager.current.canvas.transform);

        window.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(window);
            CurrencyChangeGameEvent currencyInfo = new CurrencyChangeGameEvent(lvlReward[info.newLvl][0], CurrencyType.Coins);
            EventManager.Instance.QueueEvent(currencyInfo);

            currencyInfo = new CurrencyChangeGameEvent(lvlReward[info.newLvl][1], CurrencyType.Bucks);
            EventManager.Instance.QueueEvent(currencyInfo);
        });
    }
}


