using UnityEngine;
using UnityEngine.UI;

public class ScrollLimit : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public float padding = 10f;

    private float contentHeight;
    private float viewportHeight;

    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

        contentHeight = content.rect.height;
        viewportHeight = scrollRect.viewport.rect.height;
    }

    void LateUpdate()
    {
        float lowerLimit = 0f;
        float upperLimit = contentHeight - viewportHeight;

        if (scrollRect.verticalNormalizedPosition < 0f)
        {
            scrollRect.verticalNormalizedPosition = 0f;
        }
        else if (scrollRect.verticalNormalizedPosition > 1f)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        float contentPosY = content.anchoredPosition.y;

        if (contentPosY > (upperLimit + padding))
        {
            contentPosY = upperLimit + padding;
        }
        else if (contentPosY < (-padding))
        {
            contentPosY = -padding;
        }

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, contentPosY);
    }
}
