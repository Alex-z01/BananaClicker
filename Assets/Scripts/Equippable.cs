using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Equippable
{
    public string Item_Name { get; set; }
    public string Item_Description { get; set; }
    public ItemInfo Item_Info { get; set; }
    public bool EquipedState { get; set; } = false;

    [JsonConverter(typeof(StringEnumConverter))]
    public Gear.EquipSocket Socket { get; set; }

    public Equippable() { }

    public Equippable(string item_Name, string item_Description, Gear.EquipSocket socket, List<Stats.StatType> validStats)
    {
        Item_Name = item_Name;
        Item_Description = item_Description;
        Socket = socket;

        Item_Info = new();
        Item_Info.ValidStats = validStats;
    }

    public void GenerateItemInfo(ItemSystem.TierEnum tier)
    {
        // Set the tier
        Item_Info.tier = tier;

        // Generate the stats
        List<Stats.StatType> stats = RandomStatsSelection(RandomStatCount());

        // Generate the values for the stats
        Dictionary<Stats.StatType, object> stat_info = RandomStatValues(stats);

        // Assign the stats
        Item_Info.stats = stat_info;

        // Generate Currency and Price 
        ItemInfo.CurrencyType currency = RandomCurrencyType();
        BucketNumber price = GeneratePrice(currency);

        Item_Info.Currency = currency;
        Item_Info.Buy_Price = price;
    }

    private int RandomStatCount()
    {
        // The tier object contains all tier information
        TierScriptableObject tierScriptableObject = Manager.Instance.dataManager.defaultTierData[Item_Info.tier];

        return UnityEngine.Random.Range(tierScriptableObject.MinStatCount, tierScriptableObject.MaxStatCount);
    }

    private List<Stats.StatType> RandomStatsSelection(int count)
    {
        // The tier object contains all tier information
        TierScriptableObject tierScriptableObject = Manager.Instance.dataManager.defaultTierData[Item_Info.tier];

        // UNCOMMENT if you want items to have specific valid stats as well
        // Create a copy of the list of valid stat types
        //List<Stats.StatType> copy = tierScriptableObject.ValidStats.Intersect(Item_Info.ValidStats).ToList();

        List<Stats.StatType> copy = new List<Stats.StatType>(tierScriptableObject.ValidStats);
        Debug.Log(copy.Count);

        // Create the result list
        List<Stats.StatType> result = new();

        // Pick a random from the copy
        for (int i = 0; i < count; i++)
        {
            int index = UnityEngine.Random.Range(0, copy.Count);
           
            result.Add(copy[index]);
            
            copy.RemoveAt(index);
        }

        return result;
    }

    private Dictionary<Stats.StatType, object> RandomStatValues(List<Stats.StatType> stats)
    {
        Dictionary<Stats.StatType, object> stat_info = new(stats.Count);

        for(int i = 0; i < stats.Count; i++)
        {
            Stats.StatType type = stats[i];

            var value = Stats.GenerateRandomStatValues[type]();

            stat_info.Add(type, value);
        }

        return stat_info;
    }

    private ItemInfo.CurrencyType RandomCurrencyType()
    {
        int index = 0;

        switch(Item_Info.tier)
        {
            case ItemSystem.TierEnum.Regular:
                index = 0;
                break;

            case ItemSystem.TierEnum.Special:
                index = UnityEngine.Random.Range(0, 2);
                break;

            case ItemSystem.TierEnum.Legendary:
                index = 1;
                break;

            case ItemSystem.TierEnum.Mythic:
                index = UnityEngine.Random.Range(1, 3);
                break;
        }

        return (ItemInfo.CurrencyType)index;
    }

    private BucketNumber GeneratePrice(ItemInfo.CurrencyType currencyType)
    {
        BucketNumber price = BucketNumber.Zero;

        // The tier object contains all tier information
        TierScriptableObject tierScriptableObject = Manager.Instance.dataManager.defaultTierData[Item_Info.tier];

        if (currencyType == ItemInfo.CurrencyType.Bananas)
        {
            price = Manager.Instance.game.DisplayBananaCount * tierScriptableObject.BasePriceRatio + new BucketNumber(1, 3);
        }
        else if(currencyType == ItemInfo.CurrencyType.BlackBananas)
        {
            price = Manager.Instance.prestige.BlackBananas * tierScriptableObject.BasePriceRatio + 5;
        }
        else if(currencyType == ItemInfo.CurrencyType.CosmicBananas)
        {
            price = new BucketNumber(1, 0);
        }

        BucketNumber original_price = price;
        foreach(KeyValuePair<Stats.StatType, object> pair in Item_Info.stats)
        {
            // The tier object contains all tier information
            StatScriptableObject statScriptableObject = Manager.Instance.dataManager.defaultStatData[pair.Key];

            price += original_price * statScriptableObject.PriceInfluence;
        }
        
        price = BucketNumber.Random(price, price * 3f);

        if(price.GetMagnitude() < 1) { return price.Round(0); }

        return price.Round(2);
    }

    public Sprite GetItemIcon()
    {
        return Manager.Instance.dataManager.defaultEquippablesData[Item_Name].sprite;
    }

    public Equippable Clone()
    {
        Equippable clone = new();
        clone.Item_Name = this.Item_Name;
        clone.Item_Description = this.Item_Description;
        clone.Item_Info = this.Item_Info.Clone();
        clone.EquipedState = this.EquipedState;
        clone.Socket = this.Socket;
        return clone;
    }
}

[Serializable]
public class ItemInfo
{
    public enum CurrencyType { Bananas, BlackBananas, CosmicBananas };

    [JsonConverter(typeof(StringEnumConverter))]
    public CurrencyType Currency;

    public BucketNumber Buy_Price;

    public int _ID;

    public ItemSystem.TierEnum tier;
    public Dictionary<Stats.StatType, object> stats;

    [JsonIgnore] public List<Stats.StatType> ValidStats;
    [JsonIgnore] public TierScriptableObject tierScriptableObject;

    public ItemInfo() { }

    public ItemInfo Clone()
    {
        ItemInfo clone = new();
        clone.Currency = this.Currency;
        clone.Buy_Price = this.Buy_Price.Clone();
        clone.tier = this.tier;
        clone.ValidStats = this.ValidStats;
        clone.stats = new Dictionary<Stats.StatType, object>(this.stats);
        return clone;
    }
}
