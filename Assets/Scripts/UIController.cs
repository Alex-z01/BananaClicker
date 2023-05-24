using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class Layout
{
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
            if(!_state) { layout.transform.localPosition = originalPosition; }
            layout.SetActive(_state);
        }
    }

    public Vector3 intendedLocation;
    public int track;
}

public class UIController : MonoBehaviour
{
    public GameObject mainCanvas;

    public Image BananaImage;
    public Sprite[] PeelStages;
    public Sprite[] CurrencySprites;

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
    public List<Transform> menuButtons;
    public List<Transform> upgradeContainers;

    public Transform HUD;

    public GameObject prestigeMenu;
    public GameObject prestigeButton;

    public Layout previousLayout, currentLayout;
    public List<Layout> layoutList;

    private AudioControl _audioControl;
    private Game _game;
    private Banana _banana;
    private Prestige _prestige;
    private DataManager _dataManager;

    private void Start()
    {
        _game = Manager.Instance.game;
        _banana = Manager.Instance.banana;
        _prestige = Manager.Instance.prestige;
        _dataManager = Manager.Instance.dataManager;
        _audioControl = Manager.Instance.audioControl;

        foreach(Layout layout in layoutList) 
        {
            layout.originalPosition = layout.layout.GetComponent<RectTransform>().localPosition;
        }

        Subscriptions();
    }

    private void Subscriptions()
    {
        _prestige.OnPrestige += OnPrestige;
    }

    private void OnPrestige(object sender, EventArgs args)
    {
        ResetShopTabs();
        ResetMenuTabs();
    }

    private void ResetShopTabs()
    {
        for(int i = 2; i < shopTabs.Count; i++)
        {
            shopTabs[i].Find("lock").GetComponent<Image>().enabled = true;
            shopTabs[i].Find("icon").GetComponent<Image>().enabled = false;
        }
    }

    private void ResetMenuTabs()
    {
        for (int i = 1; i < shopTabs.Count; i++)
        {
            menuButtons[i].Find("lock").GetComponent<Image>().enabled = true;
            menuButtons[i].Find("icon").GetComponent<Image>().enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowLayout("Default");
        }
    }

    public void OpenShop()
    {
        ShowLayout("Upgrades");
        ShopTab(0);

        RectTransform rectTransform = layoutList[1].layout.GetComponent<RectTransform>();

        StartCoroutine(AnimUtility.Slide(rectTransform, rectTransform.localPosition, layoutList[1].intendedLocation, 0.35f));
    }

    public void OpenSettings()
    {
        ShowLayout("Settings");
        RefreshStatsMenu();

        RectTransform rectTransform = layoutList[5].layout.GetComponent<RectTransform>();

        StartCoroutine(AnimUtility.Slide(rectTransform, rectTransform.localPosition, layoutList[5].intendedLocation, 0.35f));
    }

    public void OpenVideoSettings()
    {
        ShowLayout("VideoSettings");

        RectTransform rectTransform = layoutList[6].layout.GetComponent<RectTransform>();

        StartCoroutine(AnimUtility.Slide(rectTransform, rectTransform.localPosition, layoutList[6].intendedLocation, 0.35f));
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

        prestigeCostText.color = _game.BananaCount >= value ? Color.white : Color.red;
    }

    public void UpdateBlackBananaCount(BucketNumber value)
    {
        //print($"Black bananas {value}");
        blackBananaCountText.text = value.ToString();
    }

    public void PeelUI(int stage)
    {
        if(BananaImage.sprite == PeelStages[stage]) { return; }

        BananaImage.sprite = PeelStages[stage];

        if(stage == 5)
        {
            GameObject peeledBanana = Instantiate(BananaImage.gameObject, BananaImage.gameObject.transform.parent);
            GameObject bananaGainContainer = Instantiate(Manager.Instance.banana.gainTextPrefab, peeledBanana.transform);

            bananaGainContainer.GetComponent<TMP_Text>().text = "+" + Calculations.PeelReward;
            bananaGainContainer.GetComponent<TextUtility>().text = bananaGainContainer.GetComponent<TMP_Text>();
            bananaGainContainer.GetComponent<TextUtility>().EnableOutline();
            bananaGainContainer.GetComponent<TMP_Text>().fontSize = 85f;
            bananaGainContainer.GetComponent<TMP_Text>().fontSharedMaterial.SetFloat("_OutlineWidth", 0.1f);
            bananaGainContainer.GetComponent<TMP_Text>().color = UnityEngine.Random.ColorHSV();

            Destroy(peeledBanana.GetComponent<Button>());
            Destroy(peeledBanana.GetComponent<Banana>());

            peeledBanana.transform.localPosition = BananaImage.transform.localPosition + new Vector3(0, 0, -5f);

            peeledBanana.GetComponent<Image>().raycastTarget = false;

            peeledBanana.GetComponent<UiUtility>().Pop(0.85f, 8f, Vector2.zero, peeledBanana.transform.localScale);
            peeledBanana.GetComponent<UiUtility>().SlideTo(peeledBanana, peeledBanana.transform.localPosition + new Vector3(0, 200f, 0), 1f);
            peeledBanana.GetComponent<UiUtility>().FadeOut(0.85f, true);

            _audioControl.sfx.PlayOneShot(_audioControl.Banana_Clip[2]);
        }
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
            }
            else if(!layout.alwaysActive)
            {
                layout.State = false;
            }
        });

        _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[2]);

        if (_audioControl.music.clip.name != _audioControl.Music[currentLayout.track].name) 
        {
            _audioControl.music.clip = _audioControl.Music[currentLayout.track];
            _audioControl.music.Stop();
            _audioControl.music.Play(0);
        }
        
    }

    public void ShopTab(int index)
    {
        switch (index)
        {
            // Stat menu
            case 2:
                if (!Stats.Unlockables["Stat_Upgrades"].IsUnlocked)
                {
                    shopTabs[index].GetComponent<UiUtility>().Shake(0.5f, 1, shopTabs[index].transform.localPosition);
                    _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[1]);
                    return;
                }
                break;

            // Buff menu
            case 3:
                if (!Stats.Unlockables["Buff_Upgrades"].IsUnlocked)
                {
                    shopTabs[index].GetComponent<UiUtility>().Shake(0.5f, 1, shopTabs[index].transform.localPosition);
                    _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[1]);
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

        foreach (KeyValuePair<string, GenericUpgrade> pair in _game.upgrades)
        {
            if (_dataManager.defaultUpgradeData[pair.Key].upgradeType.Equals(UpgradeScriptableObject.UpgradeType.Active))
            {
                activeUpgrades += pair.Value.count;
            }

            else if(_dataManager.defaultUpgradeData[pair.Key].upgradeType.Equals(UpgradeScriptableObject.UpgradeType.Idle))
            {
                idleUpgrades += pair.Value.count;
            }
        }

        totalActiveUpgradesText.text = "Active Upgrades: " + activeUpgrades.ToString();
        totalIdleUpgradesText.text = "Idle Upgrades: " + idleUpgrades.ToString();

        critChanceText.text = "Crit Chance: " + Stats.CritChance.ToString() + "%";
        critMultiplierText.text = "Crit Multiplier: " + $"({Stats.CritMultiplier.x}x, {Stats.CritMultiplier.y}x)";

        totalPrestigesText.text = "Total Prestiges: " + _prestige.totalPrestiges.ToString("##,##0");
        prestigeBonusText.text = "Prestige Bonus: " + $"{_prestige.BlackBananas.GetValue() * 0.25f}%";
    }
}
