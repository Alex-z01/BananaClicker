using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Stats
{
    /// <summary>
    /// This enum refers to all the stat types available in the game. (i.e CritChance, BalloonSpawnCount, etc)
    /// </summary>
    public enum StatType { None, CritMultiplier, CritChance, CrateUnlock, CrateSpawnChance, CrateSpawnCount, CrateSpawnRate, BalloonUnlock, BalloonSpawnCount, BalloonRewardChance }

    private static Vector2 CritMultiplierDefault = new Vector2(2, 3);
    private static int CrateSpawnerCountDefault = 1;
    private static float CrateSpawnChanceDefault = 20;
    private static float CrateSpawnRateDefault = 120;
    private static int BalloonCountDefault = 5;
    private static float BalloonRewardChanceDefault = 10;

    public static bool ActiveUnlocked = true;
    public static bool IdleUnlocked = true;
    public static bool StatUnlocked = false;
    public static bool BuffUnlocked = false;

    // Crit Stuff
    public static Vector2 CritMultiplier { get { return GetCritMultiplier(); } }
    public static BucketNumber CritChance { get { return GetCritChance(); } }

    // Crate stuff
    public static bool CratesUnlocked { get { return GetCratesUnlocked(); } }
    public static int CrateSpawnerCount { get { return GetCrateSpawnerCount(); } }
    public static float CrateSpawnChance { get { return GetCrateSpawnChance(); } }
    public static float CrateSpawnRate { get { return GetCrateSpawnRate(); } }

    // Balloon stuff
    public static bool BalloonsUnlocked { get { return GetBalloonsUnlocked(); } }
    public static int BalloonSpawnCount { get { return GetBalloonSpawnCount(); } }
    public static float BalloonRewardChance { get { return GetBalloonRewardChance(); } }

    /// <summary>
    /// Calculates the current crit multiplier based on any and all upgrades owned, that enhance the stat
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

        return new Vector2(CritMultiplierDefault.x + min.GetValue(), CritMultiplierDefault.y + max.GetValue());
    }

    /// <summary>
    /// Calculates the current crit chance based on any and all upgrades owned, that enhance the stat
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

    /// <summary>
    /// Checks whether or not crates have been unlocked
    /// </summary>
    /// <returns>Boolean CratesUnlocked</returns>
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

    /// <summary>
    /// Calculates the crate spawner count based on any and all upgrades owned, that enhance the stat
    /// </summary>
    /// <returns>int CrateSpawnerCount</returns>
    private static int GetCrateSpawnerCount()
    {
        int total = CrateSpawnerCountDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CrateSpawnCount)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber valuePer = statUpgrade.baseValueModifier;

                total += (int)(count * valuePer).GetValue();
            }
        }

        return total;
    }

    /// <summary>
    /// Calculates the crate spawn chance (shared by all spawners) based on any and all upgrades owned, that enhance the stat
    /// </summary>
    /// <returns>float CrateSpawnChance</returns>
    private static float GetCrateSpawnChance()
    {
        float total = CrateSpawnChanceDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CrateSpawnChance)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber valuePer = statUpgrade.baseValueModifier;

                total += (count * valuePer).GetValue();
            }
        }

        return total;
    }

    /// <summary>
    /// Calculates the crate spawn rate (shared by all spawners) based on any and all upgrades owned, that enhance the stat
    /// </summary>
    /// <returns>float CrateSpawnRate</returns>
    private static float GetCrateSpawnRate()
    {
        float total = CrateSpawnRateDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CrateSpawnRate)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber valuePer = statUpgrade.baseValueModifier;

                total -= (count * valuePer).GetValue();
            }
        }

        return total;
    }

    /// <summary>
    /// Checks whether or not balloons have been unlocked
    /// </summary>
    /// <returns>Boolean BalloonsUnlocked</returns>
    private static bool GetBalloonsUnlocked()
    {
        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.BalloonUnlock)
            {
                if (statUpgrade.count == 1) { return true; }
            }
        }

        return false;
    }

    private static int GetBalloonSpawnCount()
    {
        int total = BalloonCountDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.BalloonSpawnCount)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber valuePer = statUpgrade.baseValueModifier;

                total += (int)(count * valuePer).GetValue();
            }
        }

        return total;
    }

    private static float GetBalloonRewardChance()
    {
        float total = BalloonRewardChanceDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.BalloonSpawnCount)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber valuePer = statUpgrade.baseValueModifier;

                total += (count * valuePer).GetValue();
            }
        }

        return total;
    }

    // Event triggers
    
    public static void OnStatUpgradesUnlocked()
    {
        Debug.Log("UNLOCKED STAT UPGRADES");
        StatUnlocked = true;
    }

    public static void OnBuffUpgradesUnlocked()
    {
        Debug.Log("UNLOCKED BUFF UPGRADES");
        BuffUnlocked = true;
    }
}
