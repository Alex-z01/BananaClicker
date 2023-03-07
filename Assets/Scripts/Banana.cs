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

    public BucketNumber critChance;
    public Vector2 critMultiplierRange;
    public bool crazyCrit;
    public float crazyCritTimer;

    private Vector3 originalScale;

    private Game game;
    private AudioControl audioControl;
    
    private void Start()
    {
        game = Manager.Instance.game;
        audioControl = Manager.Instance.audioControl;

        originalScale = transform.localScale;

        InvokeRepeating("CrazyCrit", 5, 300);
    }

    public void Collect()
    {
        BucketNumber blackBananaBonus = Calculations.BlackBananaBonus;
        BucketNumber critMultiplier = Calculations.CritMultiplier;
        BucketNumber finalClickValue = BananaClickValue * blackBananaBonus * critMultiplier;

        //print($"{BananaClickValue} {blackBananaBonus} {critMultiplier}");

        Peel();
        game.BananaCount += finalClickValue;
        game.DisplayBananaCount += finalClickValue;

        // Banana Pop
        GetComponent<UiUtility>().Pop(0.2f, 0.1f, Vector2.zero, originalScale);

        GameObject bananaGainContainer = Instantiate(gainTextPrefab, transform);

        bananaGainContainer.GetComponent<TMP_Text>().text = "+" + finalClickValue.ToString();
        bananaGainContainer.GetComponent<TextUtility>().text = bananaGainContainer.GetComponent<TMP_Text>();

        if (critMultiplier > 1)
        {
            bananaGainContainer.GetComponent<TextUtility>().EnableOutline();
            bananaGainContainer.GetComponent<TMP_Text>().fontSize = 85f;
            bananaGainContainer.GetComponent<TMP_Text>().fontSharedMaterial.SetFloat("_OutlineWidth", 0.1f);
            bananaGainContainer.GetComponent<TMP_Text>().color = Random.ColorHSV();

            bananaGainContainer.GetComponent<TextUtility>().Pop(1f, 1f + critMultiplier.GetCoefficient() / 10f, new Vector2(-30f, 30f));
            bananaGainContainer.GetComponent<TextUtility>().FadeOut(4f, true);
            bananaGainContainer.GetComponent<TextUtility>().SlideUp(4f, 150f);

            GetComponent<AudioSource>().PlayOneShot(audioControl.Banana_Clip[1]);

            return;
        }

        GetComponent<AudioSource>().PlayOneShot(audioControl.Banana_Clip[0]);
        bananaGainContainer.GetComponent<TextUtility>().DefaultFont();
        bananaGainContainer.GetComponent<TMP_Text>().fontSize = 20f;
        bananaGainContainer.GetComponent<TextUtility>().Pop(0.5f, 0.5f, new Vector2(-20f, 20f));
        bananaGainContainer.GetComponent<TextUtility>().FadeOut(1.5f, true);
        bananaGainContainer.GetComponent<TextUtility>().SlideUp(1.5f, 65f);
    }

    private void Peel()
    {
        PeelProgress += 1;

        if(PeelProgress >= PeelThreshold)
        {
            PeelProgress = BucketNumber.Zero;
            game.BananaCount += Calculations.PeelReward;
        }
    }

    public void CrazyCrit()
    {
        crazyCrit = true;

        StartCoroutine(BackgroundGradient());
    }

    public void HandleUpgradeBought()
    {
        BananaClickValue = Calculations.BaseOnClickValue;
    }

    IEnumerator BackgroundGradient()
    {
        float elapsedTime = 0f;
        float colorTime = 0;
        Color endColor = Color.red;

        while (elapsedTime < crazyCritTimer)
        {
            elapsedTime += Time.deltaTime;
            colorTime += Time.deltaTime * 1.15f;

            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, endColor, Time.deltaTime * 1.15f);

            if(colorTime > 0.95f)
            {
                colorTime = 0;
                endColor = Random.ColorHSV();
            }

            yield return null;
        }

        crazyCrit = false;

        elapsedTime = 0f;
        while(elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;

            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, new Color(0.65f, 0.93f, 0.4f), Time.deltaTime);

            yield return null;
        }
    }
}
