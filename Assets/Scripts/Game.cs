using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Game : MonoBehaviour
{
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

    public UnityEvent HandleBoughtEvent;

    private void Start()
    {
        InvokeRepeating("IdleIncome", 1, 1);
        StartCoroutine(CClimb());
        DisplayBananaCount = BananaCount;
    }

    public void IdleIncome()
    {
        if(BananasPerSecond > BucketNumber.Zero) {
            BananaCount += BananasPerSecond * Calculations.BlackBananaBonus;
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

    public bool CheckUnlockRequirements(UpgradeScriptableObject upgradeData)
    {
        if (upgradeData.unlockRequirements.Count == 0) { upgrades[upgradeData.upgradeName].unlocked = true; return true; }

        // Check each requirement to unlock this upgrade
        foreach (UnlockRequirement req in upgradeData.unlockRequirements)
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

    public void HandleUpgradeBought()
    {
        BananasPerSecond = Calculations.PerSecondValue;
    }


}
