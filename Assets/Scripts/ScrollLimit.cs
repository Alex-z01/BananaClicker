using UnityEngine;
using UnityEngine.UI;

public class ScrollLimit : MonoBehaviour
{
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private ScrollRect scrollRect;
    private RectTransform contentRect;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentRect = scrollRect.content;
    }
}
