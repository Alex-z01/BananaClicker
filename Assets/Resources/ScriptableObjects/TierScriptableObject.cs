using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tier", menuName = "ScriptableObjects/System/Tier")]
public class TierScriptableObject : ScriptableObject
{
    public ItemSystem.TierEnum tier;
    public float BasePriceRatio, PriceInfluence;
    [JsonIgnore] public Color color;
    [JsonIgnore] public float Chance;
    [JsonIgnore] public int MinStatCount, MaxStatCount;
    [JsonIgnore] public List<Stats.StatType> ValidStats;

    private float _weight;

    public float GetWeight() 
    {
        _weight = (Chance / 100f) * System.Enum.GetValues(typeof(ItemSystem.TierEnum)).Length;

        return _weight;
    }
}
