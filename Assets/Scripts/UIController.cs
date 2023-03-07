using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Layout
{
    [HideInInspector]
    public Vector3 originalPosition;

    public string layoutName;
    public GameObject layout;

    public bool alwaysActive;

    private bool _state;
    public bool State
    {
        get { return _state; }
        set
        {
            _state = value;
            layout.transform.position = originalPosition;
            layout.SetActive(_state);
        }
    }

    public Vector3 intendedLocation;
}

public class UIController : MonoBehaviour
{
    private UiUtility uiUtility;

    public GameObject mainCanvas;

    public TMP_Text bananaCountText;
    public TMP_Text bananaProduceText;
    public TMP_Text prestigeCountText;
    public TMP_Text prestigeCostText;
    public TMP_Text blackBananaCountText;
    public TMP_Text statusText;

    public TMP_Text totalActiveUpgradesText;
    public TMP_Text totalIdleUpgradesText;
    public TMP_Text critChanceText, critMultiplierText;
    public TMP_Text totalPrestigesText, prestigeBonusText;

    public Transform activeUpgradeContainer;
    public Transform idleUpgradeContainer;
    public Transform statUpgradeContianer;
    public Transform buffUpgradeContainer;

    public Transform HUD;

    public GameObject prestigeMenu;
    public GameObject prestigeButton;

    public Layout previousLayout, currentLayout;
    public List<Layout> layoutList;

    public List<AudioClip> audioClips;
    private AudioSource audioSource;

    private Game game;
    private Banana banana;
    private Prestige prestige;
    private DataManager dataManager;

    private void Start()
    {
        uiUtility = GetComponent<UiUtility>();
        audioSource = GetComponent<AudioSource>();
        game = Manager.Instance.game;
        banana = Manager.Instance.banana;
        prestige = Manager.Instance.prestige;
        dataManager = Manager.Instance.dataManager;

        foreach(Layout layout in layoutList) 
        {
            layout.originalPosition = layout.layout.transform.position;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowLayout("Default");
        }
    }

    public void UpdateBananaCount(BucketNumber value)
    {
        //print($"Set balance text to {value}");
        bananaCountText.text = value.ToString();
    }

    public void UpdateBananaProduce(BucketNumber value)
    {
        //print($"Set produce text to /s {value}");
        bananaProduceText.text = value.ToString();
    }

    public void UpdatePrestigeCount(double value)
    {
        //print($"Prestige +{value}");
        prestigeCountText.text = "Prestige +" + value.ToString();
    }

    public void UpdatePrestigeCost(BucketNumber value)
    {
        print($"Prestige cost {value}");
        prestigeCostText.text = value.ToString();

        prestigeCostText.color = game.BananaCount >= value ? Color.white : Color.red;
    }

    public void UpdateBlackBananaCount(BucketNumber value)
    {
        //print($"Black bananas {value}");
        blackBananaCountText.text = value.ToString();
    }

    public void ShowLayout(string layoutName)
    {
        if (currentLayout.layoutName == layoutName) { return; }

        previousLayout = currentLayout;

        layoutList.ForEach(layout =>
        {
            if (layout.layoutName == layoutName)
            {
                currentLayout = layout;
                layout.State = true;
                uiUtility.SlideTo(currentLayout.layout, layout.intendedLocation, 0.75f);
            }
            else if(!layout.alwaysActive)
            {
                layout.State = false;
            }
        });

        audioSource.PlayOneShot(audioClips[0]);
    }

    public void SoftRefreshAllUpgrades()
    {
        foreach(Transform child in activeUpgradeContainer)
        {
            if(child.GetComponent<UpgradePrefab>() != null)
            {
                child.GetComponent<UpgradePrefab>().CostRefresh();
            }
        }
    }

    public void RefreshStatsMenu()
    {
        BucketNumber activeUpgrades = BucketNumber.Zero;
        BucketNumber idleUpgrades = BucketNumber.Zero;

        foreach (KeyValuePair<string, GenericUpgrade> pair in game.upgrades)
        {
            if (dataManager.defaultUpgradeData[pair.Key].upgradeType.Equals(UpgradeScriptableObject.UpgradeType.Active))
            {
                activeUpgrades += pair.Value.count;
            }

            else if(dataManager.defaultUpgradeData[pair.Key].upgradeType.Equals(UpgradeScriptableObject.UpgradeType.Idle))
            {
                idleUpgrades += pair.Value.count;
            }
        }

        totalActiveUpgradesText.text = "Active Upgrades: " + activeUpgrades.ToString();
        totalIdleUpgradesText.text = "Idle Upgrades: " + idleUpgrades.ToString();

        critChanceText.text = "Crit Chance: " + banana.critChance.ToString() + "%";
        critMultiplierText.text = "Crit Multiplier: " + $"({banana.critMultiplierRange.x}x, {banana.critMultiplierRange.y}x)";

        totalPrestigesText.text = "Total Prestiges: " + prestige.totalPrestiges.ToString("##,##0");
        prestigeBonusText.text = "Prestige Bonus: " + $"{prestige.BlackBananas.GetValue() * 0.05f}%";
    }
}
