using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public List<InventorySlot> slots = new List<InventorySlot>();

    public int Slot_Count = 10;

    public GameObject Slot_Prefab;
    public GameObject Slot_Container;

    private bool _initialized;

    private Gear _gear;
    private UIController uIController;
    private AudioControl _audioControl;

    private void Start()
    {
        _gear = Manager.Instance.gear;
        uIController = Manager.Instance.uIController;
        _audioControl = Manager.Instance.audioControl;
    }

    public void OpenInventory()
    {
        if (!Stats.Unlockables["Inventory"].IsUnlocked)
        {
            uIController.menuButtons[1].GetComponentInChildren<UiUtility>().Shake(0.5f, 1, uIController.menuButtons[1].Find("lock").transform.localPosition);
            _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[1]);
            return;
        }

        // Enable the UI Layout
        uIController.ShowLayout("Inventory");

        // Slide it in
        RectTransform rectTransform = uIController.layoutList[3].layout.GetComponent<RectTransform>();
        StartCoroutine(AnimUtility.Slide(rectTransform, rectTransform.localPosition, uIController.layoutList[3].intendedLocation, 0.35f));

        InitializeInventory();
        PopulateInventory();
        RefreshInventory();
    }

    /// <summary>
    /// Builds an empty slot inventory
    /// </summary>
    private void InitializeInventory()
    {
        if (_initialized) return;

        Slot_Count = _gear.PlayerInventory.Count == 0 ? 10 : _gear.PlayerInventory.Count;

        for(int i = 0; i < Slot_Count; i++)
        {
            GameObject GO = Instantiate(Slot_Prefab, Slot_Container.transform);

            GO.GetComponent<InventorySlot>().Attached_Item = null;

            slots.Add(GO.GetComponent<InventorySlot>());
        }

        _initialized = true;
    }

    private void PopulateInventory()
    {
        if (_gear.PlayerInventory.Count == 0) return;

        for(int i = 0; i < _gear.PlayerInventory.Count; i++)
        {
            slots[i].Attached_Item = _gear.PlayerInventory[i];
        }
    }

    private void RefreshInventory()
    {
        foreach(InventorySlot slot in slots)
        {
            slot.Setup();
        }
    }

    public void SelectItem(Equippable item)
    {
        if(item == null) return;

        if(!item.EquipedState) { _gear.EquipItem(item); }
        else { _gear.UnequipItem(item); }

        foreach (InventorySlot slot in slots)
        {
            slot.Setup();
        }

        Debug.Log($"Selected {item.Item_Name}:{item.Item_Info._ID}");
    }

}


