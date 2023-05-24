using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public TMP_Text Name_Text, Desc_Text, Stats_Text, Price_Text;
    public Image Currency_Icon;

    TierScriptableObject tierScriptableObject;

    public void UpdateValues(Equippable item)
    {
        Name_Text.text = item.Item_Name;
        Desc_Text.text = item.Item_Description;

        tierScriptableObject = Manager.Instance.dataManager.defaultTierData[item.Item_Info.tier];
        Name_Text.color = tierScriptableObject.color;

        if(item.Item_Info.Currency == ItemInfo.CurrencyType.Bananas)
        {
            Currency_Icon.sprite = Manager.Instance.uIController.CurrencySprites[0];
        }
        else if(item.Item_Info.Currency == ItemInfo.CurrencyType.BlackBananas)
        {
            Currency_Icon.sprite = Manager.Instance.uIController.CurrencySprites[1];
        }
        else
        {
            Currency_Icon.sprite = Manager.Instance.uIController.CurrencySprites[2];
        }

        string stats = "";
        
        foreach(KeyValuePair<Stats.StatType, object> pair in item.Item_Info.stats)
        {
            Type type = pair.Value.GetType();

            Debug.Log(type);

            if(type == typeof(Single[]))
            {
                Single[] arr = (Single[])pair.Value;

                stats += $"{pair.Key} +({arr[0]}, {arr[1]})\n";
                break;
            }

            if (Stats.TypeStatDictionary[pair.Key] == typeof(int))
            {
                if((int)pair.Value > 0)
                {
                    stats += $"{pair.Key} +{pair.Value}\n";
                    break;
                }
                else
                {
                    stats += $"{pair.Key} {pair.Value}\n";
                    break;
                }
            }

            if (Stats.TypeStatDictionary[pair.Key] == typeof(float))
            {
                if ((float)pair.Value > 0)
                {
                    stats += $"{pair.Key} +{pair.Value}\n";
                    break;
                }
                else
                {
                    stats += $"{pair.Key} {pair.Value}\n";
                    break;
                }
            }

            if (Stats.TypeStatDictionary[pair.Key] == typeof(BucketNumber))
            {
                if ((BucketNumber)pair.Value > 0)
                {
                    stats += $"{pair.Key} +{pair.Value}\n";
                    break;
                }
                else
                {
                    stats += $"{pair.Key} {pair.Value}\n";
                    break;
                }
            }
        }

        Stats_Text.text = stats;

        Price_Text.text = item.Item_Info.Buy_Price.ToString();
    }
}
