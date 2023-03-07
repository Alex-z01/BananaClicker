using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Stats
{
    public enum StatType { None, CritMultiplier, CritChance, CrateUnlock, CrateSpawnChance, CrateSpawnCount, CrateSpawnTime, BalloonUnlock, BalloonSpawnCount, BalloonRewardChance }

    public static Vector2 CritMultiplier { get { return GetCritMultiplier(); } }
    public static BucketNumber CritChance { get { return GetCritChance(); } }
    public static bool CratesUnlocked { get { return GetCratesUnlocked(); } }

    /// <summary>
    /// Returns the current crit multiplier based on any and all upgrades owned that enhance the stat
    /// </summary>
    /// <returns>Vector2 CritMultiplierRange</returns>
    private static Vector2 GetCritMultiplier()
    {
        BucketNumber min = BucketNumber.Zero;
        BucketNumber max = BucketNumber.Zero;

        foreach(KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if(statUpgrade != null && statUpgrade.stat == StatType.CritMultiplier)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber value = statUpgrade.baseValueModifier;

                BucketNumber total = count * value;

                min += total;
                max += total;
            }
        }

        return new Vector2(2 + min.GetValue(), 3 + max.GetValue());
    }

    /// <summary>
    /// Returns the current crit chance based on any and all upgrades owned that enhance the stat
    /// </summary>
    /// <returns>BucketNumber CritChance</returns>
    private static BucketNumber GetCritChance()
    {
        BucketNumber critChance = BucketNumber.Zero;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CritChance)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber value = statUpgrade.baseValueModifier;

                BucketNumber total = count * value;

                critChance += total;
            }
        }

        return critChance;
    }


    private static bool GetCratesUnlocked()
    {
        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CrateUnlock)
            {
                if(statUpgrade.count == 1) { return true; }
            }
        }

        return false;
    }
}
