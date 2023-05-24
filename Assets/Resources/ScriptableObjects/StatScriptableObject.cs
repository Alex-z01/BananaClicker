using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat", menuName = "ScriptableObjects/System/Stat")]
public class StatScriptableObject : ScriptableObject
{
    public Stats.StatType stat;
    public float PriceInfluence;
}