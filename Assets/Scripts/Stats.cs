using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine.UI;

public static class Stats
{
    /// <summary>
    /// This enum refers to all the stat types available in the game. (i.e CritChance, BalloonSpawnCount, etc)
    /// 
    /// When adding a stat to the game you must also do the following: <br></br>
    /// - Add it to the TypeStatDictionary. <br></br>
    /// - Add a Default value variable for it <br></br>
    /// - Create a static getter variable for it that returns the result of it's getter method <br></br>
    /// - Create the getter method for it that accounts for any related upgrades
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum StatType 
    { 
        None, 
        CritMultiplier, 
        CritChance, 
        CrateUnlock, 
        CrateSpawnChance, 
        CrateSpawnCount, 
        CrateSpawnRate,
        CrazyCritDuration,
        CrazyCritRate,
        BalloonUnlock, 
        BalloonSpawnCount,
        BalloonRewardChance 
    }

    /// <summary>
    /// This dictionary points to the variable type corresponding to each StatType (Ex. CritChance => BucketNumber)
    /// </summary>
    public static Dictionary<StatType, Type> TypeStatDictionary = new Dictionary<StatType, Type>()
    {
        { StatType.None, typeof(int) },
        { StatType.CritMultiplier, typeof(float[]) },
        { StatType.CritChance, typeof(BucketNumber) },
        { StatType.CrateSpawnChance, typeof(float) },
        { StatType.CrateSpawnCount, typeof(int) },
        { StatType.CrazyCritDuration, typeof(float) },
        { StatType.CrazyCritRate, typeof(float) },
        { StatType.CrateSpawnRate, typeof(float) },
        { StatType.BalloonSpawnCount, typeof(int) },
        { StatType.BalloonRewardChance, typeof(float) }
    };

    /// <summary>
    /// Used for item stat generation, each StatType has its own random value generator method
    /// </summary>
    public static Dictionary<StatType, Func<object>> GenerateRandomStatValues = new Dictionary<StatType, Func<object>>()
    {
        { StatType.None, () => NoneReturn() },
        { StatType.CritMultiplier, () => RandomCritMultiplier() },
        { StatType.CritChance, () => RandomCritChance() },
        { StatType.CrateSpawnChance, () => RandomCrateSpawnChance() },
        { StatType.CrateSpawnCount, () => RandomCrateSpawnCount() },
        { StatType.CrateSpawnRate, () => RandomCrateSpawnRate() },
        { StatType.CrazyCritDuration, () => RandomCrazyCritDuration() },
        { StatType.CrazyCritRate, () => RandomCrazyCritRate() },
        { StatType.BalloonSpawnCount, () => RandomBalloonSpawnCount() },
        { StatType.BalloonRewardChance, () => RandomBalloonRewardChance() }
    };

    private static float RandomCrazyCritRate()
    {
        float rand = UnityEngine.Random.Range(1f, 5f);

        return (float) Math.Round(rand, 2);
    }

    private static float RandomCrazyCritDuration()
    {
        float rand = UnityEngine.Random.Range(1f, 10f);

        return (float)Math.Round(rand, 2);
    }

    public static Dictionary<string, Unlockable> Unlockables = new Dictionary<string, Unlockable>();

    private static object NoneReturn()
    {
        return null;
    }

    private static float[] RandomCritMultiplier()
    {
        float randX = UnityEngine.Random.Range(1, 4);
        float randY = UnityEngine.Random.Range(5, 10);

        return new float[2] { randX, randY };
    }

    private static BucketNumber RandomCritChance()
    {
        float randomCoefficient = UnityEngine.Random.Range(1, 10);

        return new BucketNumber(randomCoefficient, 0);
    }

    private static float RandomCrateSpawnChance()
    {
        double rand = UnityEngine.Random.Range(1, 10);

        return (float)Math.Round(rand, 2);
    }

    private static int RandomCrateSpawnCount()
    {
        return UnityEngine.Random.Range(1, 5);
    }

    private static float RandomCrateSpawnRate()
    {
        double rand = UnityEngine.Random.Range(-2f, -0.1f);

        return (float)Math.Round(rand, 2);
    }

    private static int RandomBalloonSpawnCount()
    {
        return UnityEngine.Random.Range(1, 5);
    }

    private static float RandomBalloonRewardChance()
    {
        double rand = UnityEngine.Random.Range(1, 5);

        return (float)Math.Round(rand, 2);
    }

    private static Vector2 CritMultiplierDefault = new Vector2(2, 3);
    private static int CrateSpawnerCountDefault = 1;
    private static float CrateSpawnChanceDefault = 20;
    private static float CrateSpawnRateDefault = 120;
    private static float CrazyCritDurationDefault = 10f;
    private static float CrazyCritRateDefault = 180f;
    private static int BalloonCountDefault = 5;
    private static float BalloonRewardChanceDefault = 10;

    // Crit Stuff
    public static Vector2 CritMultiplier { get { return GetCritMultiplier(); } }
    public static BucketNumber CritChance { get { return GetCritChance(); } }
    public static float CrazyCritDuration { get { return GetCrazyCritDuration(); } }
    public static float CrazyCritRate { get { return GetCrazyCritRate(); } }

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

        var itemBuff = (float[])GetAllSocketsBonus(StatType.CritMultiplier);

        return new Vector2(CritMultiplierDefault.x + min.GetValue() + itemBuff[0], CritMultiplierDefault.y + max.GetValue() + itemBuff[1]);
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

        var itemBuff = (BucketNumber)GetAllSocketsBonus(StatType.CritChance);

        return critChance + itemBuff;
    }

    /// <summary>
    /// Calculates the current crazy crit duration based on any and all upgrades owned, that enhance the stat
    /// </summary>
    /// <returns>float CrazyCritDuration</returns>
    private static float GetCrazyCritDuration()
    {
        float dur = CrazyCritDurationDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CrazyCritDuration)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber value = statUpgrade.baseValueModifier;

                BucketNumber total = count * value;

                dur += total.GetValue();
            }
        }

        var itemBuff = (float)GetAllSocketsBonus(StatType.CrazyCritDuration);

        return dur + itemBuff;
    }

    /// <summary>
    /// Calculates the current crazy crit rate based on any and all upgrades owned, that enhance the stat
    /// </summary>
    /// <returns>float CrazyCritRate</returns>
    private static float GetCrazyCritRate()
    {
        float rate = CrazyCritRateDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CrazyCritRate)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber value = statUpgrade.baseValueModifier;

                BucketNumber total = count * value;

                rate += total.GetValue();
            }
        }

        var itemBuff = (float)GetAllSocketsBonus(StatType.CrazyCritRate);

        return rate + itemBuff;
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
        int crateCount = CrateSpawnerCountDefault;

        foreach (KeyValuePair<string, GenericUpgrade> upgrade in Manager.Instance.game.upgrades)
        {
            StatUpgrade statUpgrade = upgrade.Value as StatUpgrade;

            if (statUpgrade != null && statUpgrade.stat == StatType.CrateSpawnCount)
            {
                BucketNumber count = statUpgrade.count;
                BucketNumber valuePer = statUpgrade.baseValueModifier;

                BucketNumber total = count * valuePer;
                crateCount += (int)total.GetValue();
            }
        }

        var itemBuff = (int)GetAllSocketsBonus(StatType.CrateSpawnCount);

        return crateCount + itemBuff;
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

        var itemBuff = (float)GetAllSocketsBonus(StatType.CrateSpawnChance);

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

        var itemBuff = (float)GetAllSocketsBonus(StatType.CrateSpawnRate);

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

        var itemBuff = (int)GetAllSocketsBonus(StatType.BalloonSpawnCount);

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

        var itemBuff = (float)GetAllSocketsBonus(StatType.BalloonRewardChance);

        return total;
    }

    // Event triggers
 
    public static void OnStatUpgradesUnlocked()
    {
        Debug.Log("UNLOCKED STAT UPGRADES");

        // Disable the lock icon
        Manager.Instance.uIController.shopTabs[2].Find("lock").GetComponent<Image>().enabled = false;
        Manager.Instance.uIController.shopTabs[2].Find("icon").GetComponent<Image>().enabled = true;
    }

    public static void OnBuffUpgradesUnlocked()
    {
        Debug.Log("UNLOCKED BUFF UPGRADES");

        // Disable the lock icon
        Manager.Instance.uIController.shopTabs[3].Find("lock").GetComponent<Image>().enabled = false;
        Manager.Instance.uIController.shopTabs[3].Find("icon").GetComponent<Image>().enabled = true;
    }

    public static void OnInventoryUnlocked()
    {
        Debug.Log("UNLOCKED INVENTORY");

        // Disable the lock icon
        Manager.Instance.uIController.menuButtons[1].Find("lock").GetComponent<Image>().enabled = false;
        Manager.Instance.uIController.menuButtons[1].Find("icon").GetComponent<Image>().enabled = true;
        Manager.Instance.uIController.menuButtons[1].GetComponent<Button>().image = Manager.Instance.uIController.menuButtons[1].Find("icon").GetComponent<Image>();
    }

    public static void OnPrestigeUnlocked()
    {
        Debug.Log("UNLOCKED PRESTIGE");

        // Disable the lock icon
        Manager.Instance.uIController.menuButtons[2].Find("lock").GetComponent<Image>().enabled = false;
        Manager.Instance.uIController.menuButtons[2].Find("icon").GetComponent<Image>().enabled = true;
        Manager.Instance.uIController.menuButtons[2].GetComponent<Button>().image = Manager.Instance.uIController.menuButtons[2].Find("icon").GetComponent<Image>();
    }

    public static void OnDealerUnlocked()
    {
        Debug.Log("UNLOCKED DEALER");

        // Disable the lock icon
        Manager.Instance.uIController.menuButtons[3].Find("lock").GetComponent<Image>().enabled = false;
        Manager.Instance.uIController.menuButtons[3].Find("icon").GetComponent<Image>().enabled = true;
        Manager.Instance.uIController.menuButtons[3].GetComponent<Button>().image = Manager.Instance.uIController.menuButtons[3].Find("icon").GetComponent<Image>();
    }

    private static object GetSocketBonus(Gear.EquipSocket socket, StatType stat)
    {
        Type type = TypeStatDictionary[stat];
        object value = null;

        if (type == typeof(BucketNumber))
        {
            value = Activator.CreateInstance(type);
        }
        else if (type == typeof(Single[]))
        {
            value = Activator.CreateInstance(type, 2);
        }
        else
        {
            if (type == typeof(int))
            {
                value = (int)default;
            }
            else if (type == typeof(float))
            {
                value = (float)default;
            }
            else if (type == typeof(double))
            {
                value = (double)default;
            }
        }

        Gear gear = Manager.Instance.gear;

        // If the slot is empty, no bonus
        if (gear.EquipSockets[socket] == null) { return value; }

        ItemInfo info = gear.EquipSockets[socket].Item_Info;

        foreach(KeyValuePair<Stats.StatType, object> pair in info.stats)
        {
            if (pair.Key.Equals(stat))
            {
                value = Add(value, pair.Value, type);
            }
        }

        return value;
    }

    private static object GetAllSocketsBonus(StatType stat)
    {
        Type type = TypeStatDictionary[stat];
        object total = null;

        if(type == typeof(BucketNumber))
        {
            total = Activator.CreateInstance(type);
        }
        else if(type == typeof(Single[]))
        {
            total = Activator.CreateInstance(type, 2);
        }
        else
        {
            if (type == typeof(int))
            {
                total = (int)default;
            }
            else if (type == typeof(float))
            {
                total = (float)default;
            }
            else if (type == typeof(double))
            {
                total = (double)default;
            }
        }

        Gear.EquipSocket[] sockets = (Gear.EquipSocket[])Enum.GetValues(typeof(Gear.EquipSocket));

        foreach (Gear.EquipSocket socket in sockets)
        {
            var bonus = GetSocketBonus(socket, stat);
            var convertedBonus = bonus.ConvertTo(type);

            if(convertedBonus != null) { total = Add(total, convertedBonus, type); }  
        }

        return total;
    }

    private static object Add(dynamic a, dynamic b, Type type)
    {
        MethodInfo addMethod = type.GetMethod("op_Addition", new[] { type, type });
        if (addMethod != null)
        {
            return addMethod.Invoke(null, new[] { a, b });
        }
        else
        {
            if(type == typeof(Single[]))
            {
                Single[] sum = new float[2];
                Single[] num1 = (float[])a;
                Single[] num2 = (float[])b;

                for(int i = 0; i < num1.Length; i++)
                {
                    sum[i] = num1[i] + num2[i];
                }

                return sum;
            }
            else
            {
                return a + b;
            }

            throw new InvalidOperationException("No addition operator found for type " + type.FullName);
        }
    }
}
