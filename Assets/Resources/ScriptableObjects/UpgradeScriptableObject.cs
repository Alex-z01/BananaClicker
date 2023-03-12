using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "ScriptableObjects/Purchases/GenericUpgrade")]
public class UpgradeScriptableObject : ScriptableObject
{
    public enum UpgradeType { Active, Idle, Stat, Buff };

    [Header("Generic")]
    public UpgradeType upgradeType;
    public GameObject entryPrefab;

    public int id;
    public string upgradeName;
    public string upgradeDescription;
    public Sprite sprite;
    public BucketNumber basePrice, priceIncrease;
    public BucketNumber maxAmount;
    public bool defaultUnlocked;
    public List<UpgradeUnlockRequirement> unlockRequirements;

    [Space(5)]
    [Header("Active")]
    public BucketNumber basePerClickValue;

    [Space(5)]
    [Header("Idle")]
    public BucketNumber basePerSecondValue;

    [Space(5)]
    [Header("Stat")]
    public Stats.StatType statType;
    public BucketNumber statChange;

    [Space(5)]
    [Header("Buff")]
    public List<string> buffTargets;
    public BucketNumber buffValue;
}

[Serializable]
public class UpgradeUnlockRequirement
{
    public string upgradeName;

    public int count;
}
