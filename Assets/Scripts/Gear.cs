using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Gear : MonoBehaviour
{
    public Inventory<Equippable> PlayerInventory = new Inventory<Equippable>(10);

    public enum EquipSocket { Head, LeftSide, RightSide, Feet, Skin };

    public List<GameObject> SocketContainers = new();

    public Dictionary<EquipSocket, Equippable> EquipSockets = new();

    public event Action EquipSocketChanged;

    private Prestige _prestige;

    private void Start()
    {
        _prestige = Manager.Instance.prestige;

        Subscriptions();

        RefreshEquippedDisplay();
    }

    private void Subscriptions()
    {
        EquipSocketChanged += RefreshEquippedDisplay;
        _prestige.OnPrestige += OnPrestige;
    }

    private void OnPrestige(object sender, EventArgs e)
    {
        PlayerInventory.Clear();
        EquipSockets = Manager.Instance.dataManager.InitializeDefaultEquipSockets();

        RefreshEquippedDisplay();
    }

    public void EquipItem(Equippable equippable)
    {
        if (equippable == null || equippable.EquipedState) { Debug.Log("Already Equipped"); return; }

        UnequipItem(equippable.Socket);

        Debug.Log($"Equipped {equippable.Item_Name}:{equippable.Item_Info._ID} into {equippable.Socket} socket");

        equippable.EquipedState = true;

        EquipSockets[equippable.Socket] = equippable;

        OnEquipSocketChanged(equippable.Socket, equippable);
    }

    public void UnequipItem(Equippable equippable)
    {
        if (equippable == null || !equippable.EquipedState) { Debug.Log("Nothing to unequip!"); return; }

        Debug.Log($"Unequipped {equippable.Item_Name}:{equippable.Item_Info._ID} from {equippable.Socket} socket");

        equippable.EquipedState = false;

        EquipSockets[equippable.Socket] = null;

        OnEquipSocketChanged(equippable.Socket, equippable);
    }

    private void UnequipItem(EquipSocket socket)
    {
        Equippable equippable = PlayerInventory.Find(item => item == EquipSockets[socket]);

        if (equippable == null || !equippable.EquipedState) { Debug.Log("Nothing to unequip!"); return; }

        Debug.Log($"Unequipped {equippable.Item_Name}:{equippable.Item_Info._ID} from {equippable.Socket} socket");

        equippable.EquipedState = false;

        EquipSockets[equippable.Socket] = null;

        OnEquipSocketChanged(equippable.Socket, equippable);
    }

    private void UnequipItem(int ID)
    {
        Equippable equippable = PlayerInventory.Find(item => item.Item_Info._ID == ID);

        if (equippable == null || !equippable.EquipedState) { Debug.Log("Nothing to unequip!"); return; }

        Debug.Log($"Unequipped {equippable.Item_Name}:{equippable.Item_Info._ID} from {equippable.Socket} socket");

        equippable.EquipedState = false;

        EquipSockets[equippable.Socket] = null;

        OnEquipSocketChanged(equippable.Socket, equippable);
    }

    protected virtual void OnEquipSocketChanged(EquipSocket socket, Equippable equippable)
    {
        Debug.Log("Event Invoked");
        EquipSocketChanged?.Invoke();
    }

    private void RefreshEquippedDisplay()
    {
        Debug.Log("Refreshing gear");
        foreach(KeyValuePair<EquipSocket, Equippable> pair in EquipSockets)
        {
            int socket = pair.Key.GetHashCode();

            if (pair.Value == null || !pair.Value.EquipedState) { SocketContainers[socket].GetComponent<Image>().enabled = false; continue; }

            EquippableScriptableObject obj = Manager.Instance.dataManager.defaultEquippablesData[pair.Value.Item_Name];

            SocketContainers[socket].GetComponent<RectTransform>().localPosition = obj.position;
            SocketContainers[socket].GetComponent<RectTransform>().localRotation = Quaternion.Euler(obj.rotation);
            SocketContainers[socket].GetComponent<RectTransform>().sizeDelta = obj.dimensions;

            SocketContainers[socket].GetComponent<Image>().enabled = true;
            SocketContainers[socket].GetComponent<Image>().sprite = obj.sprite;
        }
    }
}
