using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReverseComparer<T> : IComparer<T> where T : IComparable<T>
{
    public int Compare(T x, T y)
    {
        return y.CompareTo(x);
    }
}

public class TEST : MonoBehaviour
{
    private static readonly float PowerTerm = Mathf.Pow(10, 6 - 30 / (10e4f + 7));
    private static readonly float ExponentTerm = Mathf.Pow(10, 1.0f / 9999f);

    InventoryData<Equippable, Equippable> TESTDATA = new();
    BucketNumber bucketNumber = new BucketNumber();
    Dictionary<Stats.StatType, object> pairs = new Dictionary<Stats.StatType, object>();

    private BucketNumber currentBananas = new BucketNumber(12, 5);

    private void Start()
    {
        GenerateBlackBananas(new BucketNumber(100000, 0));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)) 
        {
            Debug.Log(GetMaxAffordableBlackBananas());
            //Debug.Log(PowerTerm * Mathf.Pow(ExponentTerm, 500));
        }
    }

    private void EquippableJsonTest()
    {  
        TESTDATA.PlayerInventory.Add(Manager.Instance.itemSystem.RandomEquippable());

        string JSON = JsonConvert.SerializeObject(TESTDATA, Formatting.Indented);

        Debug.Log(JSON);
        Debug.Log("ITEMS: " + TESTDATA.PlayerInventory.Count);

        TESTDATA.PlayerInventory.Clear();

        //TESTDATA = JsonConvert.DeserializeObject<InventoryData<Equippable, Equippable>>(JSON);
        JsonConvert.PopulateObject(JSON, TESTDATA);

        Debug.Log("ITEMS: " + TESTDATA.PlayerInventory.Count);
    }

    private void BucketNumberJsonTest()
    {
        var jsonSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> {
                                    new DictionaryConverter()
                        }
        };

        bucketNumber = new BucketNumber(99, 1);
        //Debug.Log(bucketNumber);

        string JSON = JsonConvert.SerializeObject(bucketNumber, Formatting.Indented);
        //Debug.Log($"BucketNumber: {JSON}");

        bucketNumber = null;
        bucketNumber = JsonConvert.DeserializeObject<BucketNumber>(JSON);
        Debug.Log(bucketNumber);

        foreach(var value in pairs)
        {
            Debug.Log(value);
        }

        pairs.Add(Stats.StatType.CrateSpawnChance, 10);
        pairs.Add(Stats.StatType.CritChance, bucketNumber);

        foreach (var value in pairs)
        {
            Debug.Log(value);
        }

        JSON = JsonConvert.SerializeObject(pairs, Formatting.Indented, jsonSettings);
        pairs.Clear();

        Debug.Log($"The dictionary: {JSON}");
        pairs = JsonConvert.DeserializeObject<Dictionary<Stats.StatType, object>>(JSON, jsonSettings);

        Debug.Log(pairs[Stats.StatType.CritChance].GetType());
        foreach (var value in pairs)
        {
            Debug.Log(value);
        }
    }

    public BucketNumber basePrice = new BucketNumber(75, 1);
    public float priceA = 0.1f;
    public float priceK = 1;

    private Dictionary<BucketNumber, BucketNumber> PrestigePrices = new Dictionary<BucketNumber, BucketNumber>();

    public BucketNumber GetBlackBananaPrice(BucketNumber n)
    {
        if(PrestigePrices.ContainsKey(n)) return PrestigePrices[n];

        double value = PowerTerm * Math.Pow(ExponentTerm, n.GetValue());

        return new BucketNumber((float)value, 0);
    }
    
    public void GenerateBlackBananas(BucketNumber count)
    {
        PrestigePrices.Clear();
        BucketNumber total = BucketNumber.Zero;
        for (int i = PrestigePrices.Values.Count + 1; i <= count.GetValue(); i++)
        {
            BucketNumber price = GetBlackBananaPrice(new BucketNumber(i, 0));
            total += price;

            if (!PrestigePrices.ContainsKey(total))
            {
                Debug.Log($"Total Price: {total}, BB: {i}");
                PrestigePrices.Add(total, new BucketNumber(i, 0));
            }
        }
    }
    
    private void ExpandPriceDict(int growth)
    {
        BucketNumber currentCount = new(PrestigePrices.Values.Count, 0);
        GenerateBlackBananas(currentCount + growth);
    }

    public BucketNumber GetMaxAffordableBlackBananas()
    {
        if (PrestigePrices.Count == 0)
        {
            return BucketNumber.Zero;
        }

        while (currentBananas >= PrestigePrices.Keys.Last())
        {
            // If current bananas exceed the most expensive key in the dictionary,
            // generate another 100 black bananas and add them to the dictionary.
            ExpandPriceDict(10000);
        }

        BucketNumber[] keys = new BucketNumber[PrestigePrices.Count];
        PrestigePrices.Keys.CopyTo(keys, 0);
        int min = 0;
        int max = keys.Length - 1;

        while (min <= max)
        {
            int mid = (min + max) / 2;
            BucketNumber key = keys[mid];

            if (key <= currentBananas)
            {
                min = mid + 1;
            }
            else
            {
                max = mid - 1;
            }
        }

        if (max < 0) return BucketNumber.Zero; // Not enough bananas to buy any black bananas

        return PrestigePrices[keys[max]];
    }
}



