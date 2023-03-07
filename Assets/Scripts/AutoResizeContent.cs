using UnityEngine;
using UnityEngine.UI;

public class AutoResizeContent : MonoBehaviour
{
    public int maxRows;
    public float rowHeight;
    public float padding;

    private RectTransform rectTransform;
    private int rowCount;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        rowCount = Mathf.CeilToInt((float)transform.childCount / maxRows);
        float height = (rowCount * rowHeight) + ((rowCount + 1) * padding);

        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}
