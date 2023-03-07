using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public void Collect()
    {
        BucketNumber blackBananaBonus = Calculations.BlackBananaBonus;
        BucketNumber critMultiplier = Calculations.CritMultiplier;

        BucketNumber withPerSecond = Manager.Instance.game.BananasPerSecond * 3f * blackBananaBonus * critMultiplier;
        BucketNumber fromTotal = Manager.Instance.game.BananaCount / 10f * blackBananaBonus * critMultiplier;

        print($"{Manager.Instance.game.BananasPerSecond} {blackBananaBonus} {critMultiplier}");
        print($"{Manager.Instance.game.BananaCount / 10f} {blackBananaBonus} {critMultiplier}");
        BucketNumber OnClickValue = withPerSecond > fromTotal ? withPerSecond : fromTotal;

        Manager.Instance.game.BananaCount += OnClickValue;
        Manager.Instance.game.DisplayBananaCount += OnClickValue;

        GameObject GO = new GameObject();
        GO.transform.parent = Manager.Instance.uIController.mainCanvas.transform;
        GO.transform.position = transform.position;
        TMP_Text text =  GO.AddComponent<TextMeshProUGUI>();
        TextUtility util = GO.AddComponent<TextUtility>();

        text.raycastTarget = false;
        text.text = $"+{OnClickValue.ToString()}";
        util.text = text;
        util.Pop(2f, 1f, new Vector2(-20f, 20f));
        util.FadeOut(3f, true);

        Destroy(gameObject);
    }
}
