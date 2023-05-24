using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class GenericSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Equippable Attached_Item;

    public Image Slot_Image;
    public Image Equipped_Image;

    public GameObject TooltipPrefab;
    private GameObject _currentTooltip;

    public void SetIcon(Sprite image)
    {
        Slot_Image.sprite = image;
    }

    public void Setup()
    {
        if(_currentTooltip != null)
        {
            _currentTooltip.SetActive(false);
            _currentTooltip = null;
        }

        if (Attached_Item == null)
        {
            Debug.Log("Empty item slot");
            Slot_Image.enabled = false;
            Equipped_Image.enabled = false;
            return;
        }

        Equipped_Image.enabled = Attached_Item.EquipedState;

        Slot_Image.enabled = true;
        SetIcon(Attached_Item.GetItemIcon());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Attached_Item == null) return;

        GameObject tooltip = Instantiate(TooltipPrefab, transform);

        tooltip.GetComponent<ToolTip>().UpdateValues(Attached_Item);
        tooltip.transform.localPosition = new Vector3(-250, 0, 0); 
        _currentTooltip = tooltip;

        Debug.Log($"Hovering over {Attached_Item.Item_Name}:{Attached_Item.Item_Info._ID}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Attached_Item == null) return;

        if (_currentTooltip != null)
        {
            Destroy(_currentTooltip);
            _currentTooltip = null;
        }

        Debug.Log($"Stopped hovering over {Attached_Item.Item_Name}:{Attached_Item.Item_Info._ID}");
    }
}
