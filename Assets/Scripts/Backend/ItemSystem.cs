using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSystem : MonoBehaviour
{
    private DataManager _dataManager;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TierEnum { Regular, Special, Legendary, Mythic };

    private void Start()
    {
        _dataManager = Manager.Instance.dataManager;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            TierRNGTest(50000);
        }
    }

    private void TierRNGTest(float iterations)
    {
        int regular = 0;
        int special = 0;
        int legendary = 0;
        int mythic = 0;

        TierEnum tier = 0;

        for(int i = 0; i < iterations; i++)
        {
            tier = RandomTierSelection();
            if(tier.Equals(TierEnum.Regular)) { regular++; }
            if(tier.Equals(TierEnum.Special)) { special++; }
            if(tier.Equals(TierEnum.Legendary)) { legendary++; }
            if(tier.Equals(TierEnum.Mythic)) { mythic++; }
        }

        Debug.Log(
            $"{iterations} Iterations:\n" +
            $"Regular: {regular}, {regular / iterations * 100}%\n" +
            $"Special: {special}, {special / iterations * 100}%\n" +
            $"Legendary: {legendary}, {legendary / iterations * 100}%\n" +
            $"Mythic: {mythic}, {mythic / iterations * 100}%\n"
            );
    }

    // TODO: chance to support generic item class
    public Equippable RandomEquippable()
    {
        TierEnum tier = RandomTierSelection();

        EquippableScriptableObject randomEquippable = RandomItemSelection(tier);

        Equippable equippable = randomEquippable.ToEquippable();

        equippable.GenerateItemInfo(tier);

        print($"Generated {equippable.Item_Name}: {equippable.Item_Info}");

        return equippable;
    }

    private TierEnum RandomTierSelection()
    {
        TierEnum randomTier = TierEnum.Regular;

        float totalWeight = 0;

        foreach(KeyValuePair<TierEnum, TierScriptableObject> tier in _dataManager.defaultTierData)
        {
            totalWeight += tier.Value.GetWeight();
        }

        float randWeight = UnityEngine.Random.Range(0, totalWeight);
        float runningTotal = 0;

        foreach (KeyValuePair<TierEnum, TierScriptableObject> tier in _dataManager.defaultTierData)
        {
            runningTotal += tier.Value.GetWeight();

            if(randWeight <= runningTotal)
            {
                randomTier = tier.Key;
                break;
            }
        }

        return randomTier;
    }

    private EquippableScriptableObject RandomItemSelection(TierEnum tier)
    {
        List<EquippableScriptableObject> ValidItems = TierMatch(tier);

        EquippableScriptableObject randomItem = ValidItems[UnityEngine.Random.Range(0, ValidItems.Count)];

        return randomItem;
    }

    private List<EquippableScriptableObject> TierMatch(TierEnum tier)
    {
        List<EquippableScriptableObject> allEquippables = _dataManager.defaultEquippablesData.Values.ToList();

        allEquippables = allEquippables.FindAll(item => item.ValidTiers.Contains(tier));

        return allEquippables;
    }
}
