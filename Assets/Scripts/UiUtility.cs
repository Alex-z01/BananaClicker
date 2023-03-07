using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UiUtility : MonoBehaviour
{
    private Coroutine pop;

    private Color originalColor;
    public Color toggleColor;

    [SerializeField]
    private Vector3 maxPopScaleDifference;

    private void Start()
    {
        if(GetComponent<Image>() != null) { originalColor = GetComponent<Image>().color; }
    }

    public void SlideTo(GameObject subject, Vector3 target, float duration)
    {
        StartCoroutine(CSlideTo(subject, target, duration));
    }

    public void Shake(float dur, float shakeStrength, Vector3 originalPos)
    {
        StartCoroutine(CShake(dur, shakeStrength, originalPos));
    }

    public void Pop(float dur, float strength, Vector2 angleRange, Vector3 originalScale)
    {
        if(pop != null) { StopCoroutine(pop); }
        pop = StartCoroutine(CPop(dur, strength, angleRange, originalScale));
    }

    public void Shrink(float dur, Vector2 targetScale)
    {
        StartCoroutine(CShrink(dur, targetScale));
    }

    public void FadeOut(float duration, bool destroy)
    {
        StartCoroutine(CFadeOut(duration, destroy));
    }

    public void ToggleColor()
    {
        GetComponent<Image>().color = GetComponent<Image>().color == originalColor ? toggleColor : originalColor;
    }

    IEnumerator CFadeOut(float dur, bool destroy)
    {
        float elapsedTime = 0;
        while (elapsedTime < dur)
        {
            GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, Mathf.Lerp(1, 0, elapsedTime / dur));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (destroy) { Destroy(gameObject); }
    }
    IEnumerator CSlideTo(GameObject subject, Vector3 target, float duration)
    {
        float elapsedTime = 0;

        Vector3 startPos = subject.transform.localPosition;
        
        while (elapsedTime < duration)
        {
            subject.transform.localPosition = Vector3.Lerp(startPos, target, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator CShake(float dur, float shakeStrength, Vector3 originalPos)
    {
        Vector3 startPos = transform.localPosition;

        float elapsedTime = 0;
        while(elapsedTime < dur)
        {
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = new Vector3(startPos.x + x, startPos.y + y, startPos.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }

    IEnumerator CPop(float dur, float popStrength, Vector2 angleRange, Vector3 originalScale)
    {
        float elapsedTime = 0;

        Vector3 startScale = transform.localScale;
        Quaternion startRotation = transform.localRotation;

        // Custom random gen with cosine distribution (inclination towards ends of the range)
        float randomAngle = angleRange.x + Random.Range(0, 1f) * (angleRange.y - angleRange.x);

        // Regular random
        // float randomAngle = Random.Range(angleRange.x, angleRange.y);
        transform.localRotation = startRotation * Quaternion.Euler(0, 0, randomAngle);

        while (elapsedTime < dur)
        {
            float t = elapsedTime / dur;
            float pop = Mathf.Sin(Mathf.PI * t) * popStrength;

            Vector3 newScale = startScale + new Vector3(pop, pop, 0);

            // Limit the scaling size
            if (newScale.x < originalScale.x + maxPopScaleDifference.x && newScale.x < originalScale.x + maxPopScaleDifference.y)
            {
                transform.localScale = newScale;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        float lerpDuration = 0.5f;
        float lerpElapsedTime = 0;
        while(lerpElapsedTime < lerpDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, lerpElapsedTime / lerpDuration);
            lerpElapsedTime += Time.deltaTime;
            yield return null;
        }
        
    }

    IEnumerator CShrink(float dur, Vector2 targetScale)
    {
        float elapsedTime = 0f;

        Vector2 startScale = transform.localScale;

        while(elapsedTime < dur)
        {
            elapsedTime += Time.deltaTime;

            transform.localScale = Vector2.Lerp(startScale, targetScale, elapsedTime / dur);
            yield return null;
        }
    }
}
