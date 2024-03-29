using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Calculations
{
    private const float BasePrestigeCost = 1000000;

    public static BucketNumber BlackBananaBonus
    {
        get
        {
            return GetBlackBananaBonus();
        }
    }
    public static BucketNumber CritMultiplier
    {
        get
        {
            return RollCrit();
        }
    }
    public static BucketNumber BaseOnClickValue
    {
        get
        {
            return GetBaseClickValue();
        }
    }
    public static BucketNumber PerSecondValue
    {
        get
        {
            return GetAllStructureIncome();
        }
    }
    public static BucketNumber PeelReward
    {
        get { return GetPeelReward(); }
    }
    public static string BalloonReward
    {
        get { return GetBalloonReward(); }
    }

    public static double MaxBlackBananas
    {
        get { return GetMaxBlackBananas(Manager.Instance.prestige.BlackBananas); }
    }
    public static BucketNumber TotalPrestigeCost
    {
        get { return GetTotalPrestigeCost(Manager.Instance.prestige.BlackBananas.GetValue()); }
    }

    private static BucketNumber GetBlackBananaBonus()
    {
        BucketNumber bonus = Manager.Instance.prestige.BlackBananas * 0.25f / 100f + 1f;

        return bonus;
    }

    private static BucketNumber GetBaseClickValue()
    {
        BucketNumber baseValue = BlackBananaBonus * 1;
        BucketNumber structureValues = GetAllStructureClickValue();

        return baseValue + structureValues;
    }

    /// <summary>
    /// Determines wether or not a click is critical based on the CritChance and CritMultiplier stats
    /// accounting for any other factors such as CrazyCrit
    /// </summary>
    /// <returns></returns>
    private static BucketNumber RollCrit()
    {
        if (Manager.Instance.game.crazyCrit) 
        {
            float randomMultiplier = Random.Range(Stats.CritMultiplier.x, Stats.CritMultiplier.y);

            return new BucketNumber(randomMultiplier, 0);
        }

        int randomChance = Random.Range(1, 101);

        if (new BucketNumber(randomChance, 0) <= Stats.CritChance)
        {
            float randomMultiplier = Random.Range(Stats.CritMultiplier.x, Stats.CritMultiplier.y);

            return new BucketNumber(randomMultiplier, 0);
        }
        return BucketNumber.Zero + 1;
    }

    private static double GetMaxBlackBananas(BucketNumber value)
    {
        double start = value.GetValue();

        BucketNumber prestigeCost = GetPrestigeCost(start);

        double BBcounter = start;
        // While 
        while (prestigeCost < Manager.Instance.game.BananaCount)
        {
            prestigeCost += GetPrestigeCost(BBcounter);
            BBcounter++;
        }

        BBcounter -= start;

        return BBcounter;
    }

    private static BucketNumber GetTotalPrestigeCost(double start)
    {
        BucketNumber prestigeCost = GetPrestigeCost(start);

        double BBcounter = start;
        // While 
        while(prestigeCost + GetPrestigeCost(BBcounter) < Manager.Instance.game.BananaCount)
        {
            prestigeCost += GetPrestigeCost(BBcounter);
            BBcounter++;
        }

        return prestigeCost;
    }

    private static BucketNumber GetPrestigeCost(double value)
    {
        float basePrice = 1000000;
        float powValue = Mathf.Pow((float)value, 0.4f);
        float logValue = Mathf.Log((float)value + 1);

        float price = basePrice * (1f + Mathf.Pow(powValue, 1.25f * logValue));

        return new BucketNumber(price, 0);
    }

    private static BucketNumber GetAllStructureIncome()
    {
        BucketNumber total = BucketNumber.Zero;

        foreach(KeyValuePair<string, GenericUpgrade> pair in Manager.Instance.game.upgrades)
        {
            IdleUpgrade idleUpgrade = pair.Value as IdleUpgrade;

            if(idleUpgrade != null)
            {
                total += GetStructureIncome(pair.Key);
            }
        }

        return total;
    }

    public static BucketNumber GetStructureIncome(string key)
    {
        IdleUpgrade idleUpgrade = (IdleUpgrade)Manager.Instance.game.upgrades[key];

        BucketNumber count = idleUpgrade.count;
        BucketNumber value = idleUpgrade.basePerSecondValue * BlackBananaBonus;

        // Calculate all upgrade targetted buffs
        value = ApplyBuffs(value);

        BucketNumber income = count * value;

        return income;
    }

    private static BucketNumber GetAllStructureClickValue()
    {
        BucketNumber total = BucketNumber.Zero;

        foreach (KeyValuePair<string, GenericUpgrade> pair in Manager.Instance.game.upgrades)
        {
            ActiveUpgrade activeUpgrade = pair.Value as ActiveUpgrade;

            if(activeUpgrade != null)
            {
                total += GetStructureClickValue(pair.Key);
            }
        }

        return total;
    }

    public static BucketNumber GetStructureClickValue(string key)
    {
        ActiveUpgrade activeUpgrade = (ActiveUpgrade)Manager.Instance.game.upgrades[key];

        BucketNumber count = activeUpgrade.count;
        BucketNumber value = activeUpgrade.basePerClickValue * BlackBananaBonus;

        // Calculate all upgrade targetted buffs
        value = ApplyBuffs(value);

        BucketNumber gain = count * value;

        return gain;
    }

    /// <summary>
    /// Returns the total percentage buff gained from upgrade, if any
    /// </summary>
    /// <param name="key">The name of the upgrade</param>
    /// <returns></returns>
    private static BucketNumber GetTotalBuff(string key)
    {
        BuffUpgrade buffUpgrade = (BuffUpgrade)Manager.Instance.game.upgrades[key];

        BucketNumber count = buffUpgrade.count;
        BucketNumber value = buffUpgrade.buffValue; // Represented as a percent

        BucketNumber totalBuff = count * value;

        return totalBuff;
    }

    public static BucketNumber ApplyBuffs(BucketNumber value)
    {
        foreach (KeyValuePair<string, GenericUpgrade> pair in Manager.Instance.game.upgrades)
        {
            if(pair.Value is BuffUpgrade)
            {
                BuffUpgrade buffUpgrade = pair.Value as BuffUpgrade;
                if (buffUpgrade.buffTargets.Count > 0)
                {
                    foreach (string target in buffUpgrade.buffTargets)
                    {
                        value *= GetTotalBuff(pair.Key) / 100f + 1f;
                    }
                }
            }
        }

        return value;
    }

    private static BucketNumber GetPeelReward()
    {
        BucketNumber reward = Manager.Instance.banana.PeelThreshold * BaseOnClickValue * CritMultiplier;

        return reward;
    }

    private static string GetBalloonReward()
    {
        // RNG to give reward or not
        int random = Random.Range(0, 101);

        if(random <= Manager.Instance.balloonSystem.balloonRewardChance)
        {
            List<string> upgradesAsList = Manager.Instance.game.upgrades
                .Where(pair => pair.Value.unlocked == true 
                                && pair.Value.stat != Stats.StatType.CrateUnlock
                                && pair.Value.stat != Stats.StatType.BalloonUnlock)
                .Select(pair => pair.Key)
                .ToList();

            int randomIndex = Random.Range(0, upgradesAsList.Count);
            string randomUpgrade = upgradesAsList[randomIndex];

            Debug.Log(randomUpgrade);
            return randomUpgrade;
        }
        return null;
    }
}
