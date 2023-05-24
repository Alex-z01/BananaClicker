using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equippable", menuName = "ScriptableObjects/Items/Equippables")]
public class EquippableScriptableObject : ScriptableObject
{
    public int id;
    public string Item_Name, Item_Description;

    public Sprite sprite;
    public Vector3 position, rotation;
    public Vector2 dimensions;

    public Gear.EquipSocket Socket;

    public List<ItemSystem.TierEnum> ValidTiers;
    public List<Stats.StatType> ValidStats;

    public Equippable ToEquippable()
    {
        Equippable equippable = new Equippable(
                                Item_Name,
                                Item_Description,
                                Socket,
                                ValidStats
            );

        return equippable;
    }
}

