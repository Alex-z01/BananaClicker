using Steamworks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePrefab : MonoBehaviour
{
    private AudioControl _audioControl;

    public UpgradeScriptableObject upgradeData;

    public BucketNumber upgradeCost;
    public BucketNumber perClickValue, perSecondValue;

    private bool _unlocked;
    public bool Unlocked
    {
        get { return _unlocked; }
        set
        {
            _unlocked = value;
            gameObject.SetActive(value);
            RefreshUI();
        }
    }

    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private TMP_Text upgradeDescText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private TMP_Text upgradeCountText;
    [SerializeField] private Image upgradeImage;

    private Dictionary<Stats.StatType, Action> statFunctions = new Dictionary<Stats.StatType, Action>();

    [HideInInspector] public Game game;
    [HideInInspector] public Banana banana;

    public event EventHandler UpgradePurchased;

    private void Start()
    {
        _audioControl = Manager.Instance.audioControl;

        statFunctions = new Dictionary<Stats.StatType, Action>()
        {
            { Stats.StatType.CrateUnlock, () => UnlockCrates() },
            { Stats.StatType.BalloonUnlock, () => UnlockBalloons() }
        };

        Unlocked = Manager.Instance.game.CheckUpgradeUnlockRequirements(upgradeData);
    }

    private void OnEnable()
    {
        Unlocked = Manager.Instance.game.CheckUpgradeUnlockRequirements(upgradeData);
    }

    private void CalculateScalingValues()
    {
        upgradeCost = upgradeData.basePrice + upgradeData.basePrice * Mathf.Pow(game.upgrades[upgradeData.upgradeName].count.GetValue(), upgradeData.priceIncrease.GetValue());


        //upgradeCost = game?.upgrades[upgradeData.upgradeName].price + game.upgrades[upgradeData.upgradeName].price * game.upgrades[upgradeData.upgradeName].count * upgradeData.priceIncrease;
    }

    public void RefreshUI()
    {
        CalculateScalingValues();

        if (upgradeData.sprite != null) { upgradeImage.sprite = upgradeData.sprite; }

        NameRefresh();
        CountRefresh();
        ValueRefresh();
        CostRefresh();
    }

    public void NameRefresh()
    {
        upgradeNameText.text = upgradeData.upgradeName;
    }

    public void CountRefresh()
    {
        // sets how many are owned, if any
        if (game.upgrades.ContainsKey(upgradeData.upgradeName))
        {
            upgradeCountText.text = game.upgrades[upgradeData.upgradeName].count.ToString();
        }
    }

    public void ValueRefresh()
    {
        switch(upgradeData.upgradeType)
        {
            case UpgradeScriptableObject.UpgradeType.Active:
                upgradeDescText.text = $"{upgradeData.upgradeDescription}\n +{Calculations.ApplyBuffs(upgradeData.basePerClickValue)*Calculations.BlackBananaBonus} bananas/click";
                break;

            case UpgradeScriptableObject.UpgradeType.Idle:
                upgradeDescText.text = $"{upgradeData.upgradeDescription}\n +{Calculations.ApplyBuffs(upgradeData.basePerSecondValue) * Calculations.BlackBananaBonus} bananas/sec";
                break;

            case UpgradeScriptableObject.UpgradeType.Stat:
                if(upgradeData.statType == Stats.StatType.CrateUnlock || upgradeData.statType == Stats.StatType.BalloonUnlock)
                {
                    upgradeDescText.text = $"{upgradeData.upgradeDescription}\n";
                    break;
                }
                if(upgradeData.statType == Stats.StatType.CritMultiplier)
                {
                    upgradeDescText.text = $"{upgradeData.upgradeDescription}\n +{upgradeData.statChange.GetValue()}x {upgradeData.statType.ToString()}";
                    break;
                }

                upgradeDescText.text = $"{upgradeData.upgradeDescription}\n +{upgradeData.statChange}% {upgradeData.statType.ToString()}";
                break;

            case UpgradeScriptableObject.UpgradeType.Buff:
                upgradeDescText.text = $"{upgradeData.upgradeDescription}\n {upgradeData.buffValue}%";
                break;

            default:
                upgradeDescText.text = $"{upgradeData.upgradeDescription}";
                Debug.LogWarning($"Unknown upgrade type {upgradeData.upgradeType}!");
                break;
        }
    }

    public void CostRefresh()
    {
        upgradeCostText.text = upgradeCost.ToString();

        if(game.DisplayBananaCount < upgradeCost) { upgradeCostText.color = Color.red; return; }
        if(upgradeData.maxAmount == game.upgrades[upgradeData.upgradeName].count) { upgradeCostText.color = Color.blue; return; }
        upgradeCostText.color = Color.white;
    }

    public void OnClick()
    {
        Buy();
    }

    private void Buy()
    {
        if(!CheckPurchaseValidity()) { return; }

        // If this upgrade is a stat upgrade
        if (upgradeData.upgradeType.Equals(UpgradeScriptableObject.UpgradeType.Stat) && statFunctions.ContainsKey(upgradeData.statType))
        {
            statFunctions[upgradeData.statType]();
        }
        
        game.BananaCount -= upgradeCost;
        game.upgrades[upgradeData.upgradeName].count += 1;

        _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[0]);

        UpgradePurchased?.Invoke(this, EventArgs.Empty);

        RefreshUI();
    }

    public void UnlockBalloons()
    {
        Manager.Instance.balloonSystem.SpawnBalloon();
    }

    public void UnlockCrates()
    {
        print("Unlocking crates");
        Manager.Instance.crateSystem.AddCrateSpawner();
        Manager.Instance.crateSystem.StartCrateSpawners();
    }

    private bool CheckPurchaseValidity()
    {
        if (game.BananaCount < upgradeCost)
        {
            GetComponent<UiUtility>().Shake(0.5f, 1f, transform.localPosition);
            _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[1]);
            print($"Cannot afford {upgradeData.upgradeName}!");
            return false;
        }

        if (upgradeData.maxAmount == game.upgrades[upgradeData.upgradeName].count)
        {
            GetComponent<UiUtility>().Shake(0.5f, 1f, transform.localPosition);
            _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[0]);
            print($"Max purchase amount of upgrade {upgradeData.upgradeName} reached!");
            return false;
        }

        return true;
    }
}
