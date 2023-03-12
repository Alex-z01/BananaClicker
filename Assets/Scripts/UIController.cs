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

    public List<Transform> shopTabs;
    public List<Transform> upgradeContainers;

    public Transform HUD;

    public GameObject prestigeMenu;
    public GameObject prestigeButton;

    public Layout previousLayout, currentLayout;
    public List<Layout> layoutList;

    private AudioSource audioSource;

    private Game game;
    private Banana banana;
    private Prestige prestige;
    private DataManager dataManager;
    private AudioControl audioControl;

    private void Start()
    {
        uiUtility = GetComponent<UiUtility>();
        audioSource = GetComponent<AudioSource>();
        game = Manager.Instance.game;
        banana = Manager.Instance.banana;
        prestige = Manager.Instance.prestige;
        dataManager = Manager.Instance.dataManager;
        audioControl = Manager.Instance.audioControl;

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

        audioSource.PlayOneShot(audioControl.UI_Clips[2]);
    }

    public void ShopTab(int index)
    {
        switch (index)
        {
            case 0: // Actives
                if (!Stats.ActiveUnlocked)
                {
                    shopTabs[index].GetComponent<UiUtility>().Shake(0.5f, 1, shopTabs[index].transform.localPosition);
                    GetComponent<AudioSource>().PlayOneShot(audioControl.UI_Clips[1]);
                    return;
                }
                break; 
            
            case 1: // Idle
                if (!Stats.IdleUnlocked) 
                {
                    shopTabs[index].GetComponent<UiUtility>().Shake(0.5f, 1, shopTabs[index].transform.localPosition);
                    GetComponent<AudioSource>().PlayOneShot(audioControl.UI_Clips[1]);
                    return;
                }
                break;

            case 2:
                if (!Stats.StatUnlocked)
                {
                    shopTabs[index].GetComponent<UiUtility>().Shake(0.5f, 1, shopTabs[index].transform.localPosition);
                    GetComponent<AudioSource>().PlayOneShot(audioControl.UI_Clips[1]);
                    return;
                }
                break;

            case 3:
                if (!Stats.BuffUnlocked)
                {
                    shopTabs[index].GetComponent<UiUtility>().Shake(0.5f, 1, shopTabs[index].transform.localPosition);
                    GetComponent<AudioSource>().PlayOneShot(audioControl.UI_Clips[1]);
                    return;
                }
                break;

        }

        foreach (Transform tab in upgradeContainers)
        {
            tab.gameObject.SetActive(false);
        }
        upgradeContainers[index].gameObject.SetActive(true);
    }

    public void SoftRefreshAllUpgrades()
    {
        foreach(Transform tab in upgradeContainers)
        {
            foreach(Transform child in tab)
            {
                if (child.GetComponent<UpgradePrefab>() != null)
                {
                    child.GetComponent<UpgradePrefab>().CostRefresh();
                }
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
        prestigeBonusText.text = "Prestige Bonus: " + $"{prestige.BlackBananas.GetValue() * 0.15f}%";
    }
}
