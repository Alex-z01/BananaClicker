using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
    /// <summary>
    /// Total time played in seconds
    /// </summary>
    public float timePlayed;

    [SerializeField]
    private BucketNumber _bananaCount;
    public BucketNumber BananaCount
    {
        get { return _bananaCount; }
        set 
        {
            _bananaCount = value;
            if (_bananaCount < DisplayBananaCount) { DisplayBananaCount = _bananaCount; }
            if (BananasPerSecond == new BucketNumber(0, 0) && DisplayBananaCount != BananaCount) { DisplayBananaCount = BananaCount; }
        }
    }

    [SerializeField]
    private BucketNumber _displayBananaCount;
    public BucketNumber DisplayBananaCount
    {
        get { return _displayBananaCount; }
        set
        {
            if(value > BananaCount) { _displayBananaCount = BananaCount; }
            else { _displayBananaCount = value; }
            Manager.Instance.uIController.UpdateBananaCount(DisplayBananaCount);
            Manager.Instance.uIController.SoftRefreshAllUpgrades();
        }
    }

    [SerializeField]
    private BucketNumber _bananasPerSecond;
    public BucketNumber BananasPerSecond
    {
        get { return _bananasPerSecond; }
        set
        {
            _bananasPerSecond = value;
            Manager.Instance.uIController.UpdateBananaProduce(_bananasPerSecond);
        }
    }

    public Dictionary<string, GenericUpgrade> upgrades = new Dictionary<string, GenericUpgrade>();

    private Gear _gear;
    private Upgrades _upgrades;
    private Prestige _prestige;

    public bool crazyCrit;

    private void Start()
    {
        InvokeRepeating("IdleIncome", 1, 1);
        InvokeRepeating("TimePlayed", 1, 1);
        InvokeRepeating("CheckUnlockables", 1, 3);
        InvokeRepeating("CrazyCrit", Stats.CrazyCritRate, Stats.CrazyCritRate);
        StartCoroutine(CClimb());
        DisplayBananaCount = BananaCount;

        _gear = Manager.Instance.gear;
        _upgrades = Manager.Instance.upgrades;
        _prestige = Manager.Instance.prestige;

        Subscriptions();
    }

    private void CrazyCrit()
    {
        crazyCrit = true;

        print(Stats.CrazyCritDuration);

        StartCoroutine(BackgroundGradient());
    }

    private IEnumerator BackgroundGradient()
    {
        float elapsedTime = 0f;
        float colorTime = 0;
        Color endColor = Color.red;

        while (elapsedTime < Stats.CrazyCritDuration)
        {
            elapsedTime += Time.deltaTime;
            colorTime += Time.deltaTime * 1.15f;

            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, endColor, Time.deltaTime * 1.15f);

            if (colorTime > 0.95f)
            {
                colorTime = 0;
                endColor = UnityEngine.Random.ColorHSV();
            }

            yield return null;
        }

        crazyCrit = false;

        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;

            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, new Color(0.65f, 0.93f, 0.4f), Time.deltaTime);

            yield return null;
        }
    }

    private void Subscriptions()
    {
        _gear.EquipSocketChanged += OnItemChange;
        _upgrades.OnBought += OnUpgradeBought;
        _prestige.OnPrestige += OnPrestige;
    }

    private void OnItemChange()
    {
        BananasPerSecond = Calculations.PerSecondValue;
    }

    private void TimePlayed()
    {
        timePlayed++;
    }

    private void IdleIncome()
    {
        if(BananasPerSecond > BucketNumber.Zero) {
            BananaCount += BananasPerSecond;
        }   
    }

    IEnumerator CClimb()
    {
        while(true)
        {
            yield return new WaitUntil(() => BananasPerSecond > BucketNumber.Zero);

            yield return new WaitForSeconds(1f / 20f);
            if (DisplayBananaCount < BananaCount)
            {
                DisplayBananaCount += BananasPerSecond * 1f / 20f;
            }
        }
    }

    public bool CheckUpgradeUnlockRequirements(UpgradeScriptableObject upgradeData)
    {
        if (upgradeData.unlockRequirements.Count == 0) { upgrades[upgradeData.upgradeName].unlocked = true; return true; }

        // Check each requirement to unlock this upgrade
        foreach (UpgradeUnlockRequirement req in upgradeData.unlockRequirements)
        {
            // If any requirement is not met, the upgrade has not been unlocked
            if (upgrades[req.upgradeName].count < req.count)
            {
                return false;
            }
        }
        // If all requirements are met, the upgrade is unlocked
        upgrades[upgradeData.upgradeName].unlocked = true;
        return true;
    }

    public void OnUpgradeBought(object sender, EventArgs args)
    {
        BananasPerSecond = Calculations.PerSecondValue;
    }  

    public void OnPrestige(object sender, EventArgs args)
    {
        upgrades = Manager.Instance.dataManager.InitializeUpgrades();

        BananaCount = BucketNumber.Zero;
        DisplayBananaCount = BucketNumber.Zero;
        BananasPerSecond = BucketNumber.Zero;
    }

    /// <summary>
    /// Go through all locked unlocks and check their requirements to see if theyve been met.
    /// Unlockable class handles the actual changing of lock state
    /// </summary>
    public void CheckUnlockables()
    {
        foreach(KeyValuePair<string, Unlockable> pair in Stats.Unlockables)
        {
            if(!pair.Value.IsUnlocked) { pair.Value.IsUnlocked = pair.Value.CheckUnlockRequirements(); }
        }
    }

}
