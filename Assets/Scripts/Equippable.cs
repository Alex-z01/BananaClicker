using System;
using System.Collections.Generic;
using UnityEngine;

public class Equippable
{
    public string Item_Name, Item_Description;
    public ItemInfo Item_Info;
    public bool EquipedState = false;

    public Gear.EquipSocket Socket;

    public Sprite Sprite;
    public Vector3 Position, Rotation;
    public Vector2 Dimensions;

    public Equippable(string item_Name, string item_Description, Sprite sprite, Gear.EquipSocket socket, Vector3 position, Vector3 rotation, Vector2 dimensions)
    {
        Item_Name = item_Name;
        Item_Description = item_Description;
        Sprite = sprite;
        Socket = socket;
        Position = position;
        Rotation = rotation;
        Dimensions = dimensions;

        Item_Info = GenerateItemInfo();
    }

    private ItemInfo GenerateItemInfo()
    {
        int randomIndex = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(ItemInfo.CurrencyType)).Length);
        ItemInfo.CurrencyType randomCurrency = (ItemInfo.CurrencyType)randomIndex;

        float randomCoefficient = UnityEngine.Random.Range(1, 999);
        int randomMagnitude = UnityEngine.Random.Range(0, 4);
        BucketNumber randomBuyPrice = new BucketNumber(randomCoefficient, randomMagnitude);

        randomIndex = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(Gear.GearRating)).Length);
        Gear.GearRating randomGearRating = (Gear.GearRating)randomIndex;

        int randomStatsCount = UnityEngine.Random.Range(1, 5);
        HashSet<Stats.StatType> stats = new HashSet<Stats.StatType>(randomStatsCount);

        for(int i = 0; i < randomStatsCount; i++)
        {
            randomIndex = UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(Stats.StatType)).Length);

            stats.Add((Stats.StatType)randomIndex);
        }

        ItemInfo info = new ItemInfo();

        return info;
    }
}

[Serializable]
public class ItemInfo
{
    public enum CurrencyType { Bananas, BlackBananas, CosmicBananas };
    public CurrencyType Currency;

    public BucketNumber Buy_Price;

    public Gear.GearRating Gear_Rating;

    public HashSet<Stats.StatType> Stats;

    public ItemInfo() { }

    public ItemInfo(CurrencyType currency, BucketNumber buy_Price, Gear.GearRating gear_Rating, HashSet<Stats.StatType> stats)
    {
        Currency = currency;
        Buy_Price = buy_Price;
        Gear_Rating = gear_Rating;
        Stats = stats;
    }

    public override string ToString()
    {
        return $"Currency: {Currency} \nPrice: {Buy_Price} \nRating: {Gear_Rating} \nStats: {Stats.Count}";
    }
}

[CreateAssetMenu(fileName = "Equippable", menuName = "ScriptableObjects/Items/Equippables")]
public class EquippableScriptableObject : ScriptableObject
{
    public int id;
    public string Item_Name, Item_Description;

    public Sprite sprite;
    public Vector3 position, rotation;
    public Vector2 dimensions;

    public Gear.EquipSocket Socket;
}
