using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public GameObject gainTextPrefab;

    private BucketNumber _bananaClickValue;
    public BucketNumber BananaClickValue 
    {
        get { return _bananaClickValue; }
        set
        {
            _bananaClickValue = value;
        }
    }

    public BucketNumber PeelThreshold;
    public BucketNumber PeelProgress;

    private Vector3 originalScale;

    private AudioControl _audioControl;
    private Game _game;
    private Gear _gear;
    private Prestige _prestige;
    private Upgrades _upgrades;
    private UIController _uIController;
    
    private void Start()
    {
        _audioControl = Manager.Instance.audioControl;
        _game = Manager.Instance.game;
        _gear = Manager.Instance.gear;
        _prestige = Manager.Instance.prestige;
        _upgrades = Manager.Instance.upgrades;
        _uIController = Manager.Instance.uIController;

        originalScale = transform.localScale;

        Subscriptions();
    }

    private void Subscriptions()
    {
        _gear.EquipSocketChanged += OnItemChange;
        _upgrades.OnBought += OnUpgradeBought;
        _prestige.OnPrestige += OnPrestige;
    }

    private void OnItemChange()
    {
        BananaClickValue = Calculations.BaseOnClickValue;
    }

    public void Collect()
    {
        BucketNumber blackBananaBonus = Calculations.BlackBananaBonus;
        BucketNumber critMultiplier = Calculations.CritMultiplier;
        BucketNumber finalClickValue = Calculations.BaseOnClickValue * critMultiplier;

        Peel();

        _game.BananaCount += finalClickValue;
        _game.DisplayBananaCount += finalClickValue;

        // Banana Pop
        GetComponent<UiUtility>().Pop(0.2f, 0.1f, Vector2.zero, originalScale);

        GameObject bananaGainContainer = Instantiate(gainTextPrefab, transform.parent);

        bananaGainContainer.transform.position = Input.mousePosition;
        bananaGainContainer.GetComponent<TMP_Text>().text = "+" + finalClickValue.ToString();
        bananaGainContainer.GetComponent<TextUtility>().text = bananaGainContainer.GetComponent<TMP_Text>();

        if (critMultiplier > 1)
        {
            bananaGainContainer.GetComponent<TextUtility>().EnableOutline();
            bananaGainContainer.GetComponent<TMP_Text>().fontSize = 85f;
            bananaGainContainer.GetComponent<TMP_Text>().fontSharedMaterial.SetFloat("_OutlineWidth", 0.1f);
            bananaGainContainer.GetComponent<TMP_Text>().color = UnityEngine.Random.ColorHSV();

            bananaGainContainer.GetComponent<TextUtility>().Pop(1f, 1f + critMultiplier.GetCoefficient() / 10f, new Vector2(-30f, 30f));
            bananaGainContainer.GetComponent<TextUtility>().FadeOut(2f, true);
            bananaGainContainer.GetComponent<TextUtility>().SlideUp(2f, 150f);

            _audioControl.sfx.PlayOneShot(_audioControl.Banana_Clip[1]);
            return;
        }

        _audioControl.sfx.PlayOneShot(_audioControl.Banana_Clip[0]);
        bananaGainContainer.GetComponent<TextUtility>().DefaultFont();
        bananaGainContainer.GetComponent<TMP_Text>().fontSize = 20f;
        bananaGainContainer.GetComponent<TextUtility>().Pop(0.5f, 0.5f, new Vector2(-20f, 20f));
        bananaGainContainer.GetComponent<TextUtility>().FadeOut(1.5f, true);
        bananaGainContainer.GetComponent<TextUtility>().SlideUp(1.5f, 65f);
    }

    private void Peel()
    {
        PeelProgress += 1;

        double ratio = PeelProgress.GetValue() / PeelThreshold.GetValue();

        if (ratio < 0.2f) { _uIController.PeelUI(0); }
        else if(ratio < 0.4f) { _uIController.PeelUI(1); }
        else if(ratio < 0.6f) { _uIController.PeelUI(2); }
        else if(ratio < 0.8f) { _uIController.PeelUI(3); }
        else if(ratio < 1f) { _uIController.PeelUI(4); }

        if(PeelProgress >= PeelThreshold)
        {
            _uIController.PeelUI(5);
            _uIController.PeelUI(0);

            PeelProgress = BucketNumber.Zero;
            _game.BananaCount += Calculations.PeelReward;
            _game.DisplayBananaCount += Calculations.PeelReward;
        }
    }

    public void OnUpgradeBought(object sender, EventArgs args)
    {
        BananaClickValue = Calculations.BaseOnClickValue;
    }

    private void OnPrestige(object sender, EventArgs args)
    {
        BananaClickValue = new BucketNumber(1, 0);
    }


}
