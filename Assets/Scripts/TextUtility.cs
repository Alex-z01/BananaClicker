using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextUtility : MonoBehaviour
{
    public TMP_Text text;
    public TMP_FontAsset defaultFont, outlinedFont;

    private Game game;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        game = Manager.Instance.game;
    }

    public void FadeOut(float duration, bool destroy)
    {
        StartCoroutine(CFadeOut(duration, destroy));
    }

    public void SlideUp(float duration, float dist)
    {
        StartCoroutine(CSlideUp(duration, dist));
    }

    public void Pop(float dur, float strength, Vector2 angleRange)
    {
        StartCoroutine(CPop(dur, strength, angleRange));
    }

    public void DefaultFont()
    {
        text.font = defaultFont;
    }

    public void EnableOutline()
    {
        text.font = outlinedFont;
    }

    IEnumerator CFadeOut(float dur, bool destroy)
    {
        float elapsedTime = 0;
        while(elapsedTime < dur)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1, 0, elapsedTime / dur));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if(destroy) { Destroy(gameObject); }
    }

    IEnumerator CSlideUp(float dur, float dist)
    {
        float elapsedTime = 0;
        while(elapsedTime < dur)
        {
            float speed = dist / dur;

            text.transform.localPosition += Vector3.up * speed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator CPop(float dur, float popStrength, Vector2 angleRange) 
    {
        float elapsedTime = 0;

        Vector3 startScale = text.transform.localScale;
        Quaternion startRotation = text.transform.localRotation;

        // Custom random gen with cosine distribution (inclination towards ends of the range)
        float randomAngle = angleRange.x + Random.Range(0, 1f) * (angleRange.y - angleRange.x);

        // Regular random
        // float randomAngle = Random.Range(angleRange.x, angleRange.y);
        text.transform.localRotation = startRotation * Quaternion.Euler(0, 0, randomAngle);

        while (elapsedTime < dur)
        {
            float t = elapsedTime / dur;
            float pop = Mathf.Sin(Mathf.PI * t) * popStrength;

            text.transform.localScale = startScale + new Vector3(pop, pop, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.transform.localScale = startScale;
    }
}
