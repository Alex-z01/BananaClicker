using UnityEngine.UI;

public class InventorySlot : GenericSlot
{
    private InventoryUI _inventory;

    private void Start()
    {
        _inventory = Manager.Instance.inventory;

        gameObject.GetComponent<Button>().onClick.AddListener(delegate { _inventory.SelectItem(Attached_Item); });
    }
}
