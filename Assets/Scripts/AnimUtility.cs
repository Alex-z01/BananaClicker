using System.Collections;
using UnityEngine;

public class AnimUtility : MonoBehaviour
{
    public static IEnumerator Slide(RectTransform rectTransform, Vector3 startPos, Vector3 endPos, float dur)
    {
        float elapsedTime = 0f;
        rectTransform.localPosition = startPos;

        while (elapsedTime < dur)
        {
            rectTransform.localPosition = Vector3.Lerp(startPos, endPos, elapsedTime / dur);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = endPos;
    }
}
