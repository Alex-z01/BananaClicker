using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gear : MonoBehaviour
{
    public Dictionary<string, Equippable> All_Equippables = new Dictionary<string, Equippable>();

    public enum EquipSocket { Head, LeftSide, RightSide, Feet, Skin };
    public enum GearRating { Regular, Special, Legendary };

    public List<GameObject> SocketContainers = new List<GameObject>();

    private Dictionary<EquipSocket, Equippable> _equipSockets = new Dictionary<EquipSocket, Equippable>();

    public event Action EquipSocketChanged;

    private void Start()
    {
        EquipSocketChanged += RefreshEquippedDisplay;

        RefreshEquippedDisplay();
    }

    private void Update()
    {
        // FOR TESTING
        if (Input.GetKeyDown(KeyCode.P))
        {
            EquipItem(All_Equippables["Shades"]);
        }
        if (Input.GetKeyDown(KeyCode.U)) 
        {
            UnequipItem(All_Equippables["Shades"]);
        }
    }

    private void EquipItem(Equippable equippable)
    {
        if (equippable == null || equippable.EquipedState) { Debug.Log("Already Equipped"); return; }

        Debug.Log($"Equipped {equippable.Item_Name} into {equippable.Socket} socket");

        equippable.EquipedState = true;

        _equipSockets[equippable.Socket] = equippable;

        OnEquipSocketChanged(equippable.Socket, equippable);
    }

    private void UnequipItem(Equippable equippable)
    {
        if (equippable == null || !equippable.EquipedState) return;

        Debug.Log($"Unequipped {equippable.Item_Name} from {equippable.Socket} socket");

        equippable.EquipedState = false;

        _equipSockets[equippable.Socket] = null;

        OnEquipSocketChanged(equippable.Socket, equippable);
    }

    protected virtual void OnEquipSocketChanged(EquipSocket socket, Equippable equippable)
    {
        Debug.Log("Event Invoked");
        EquipSocketChanged?.Invoke();
    }

    private void RefreshEquippedDisplay()
    {
        int counter = 0;

        Debug.Log("Refreshing gear");

        foreach(KeyValuePair<EquipSocket, Equippable> pair in _equipSockets)
        {
            if(pair.Value == null)
            {
                SocketContainers[counter].GetComponent<Image>().enabled = false;
                counter++;
                continue;
            }

            SocketContainers[counter].GetComponent<RectTransform>().localPosition = pair.Value.Position;
            SocketContainers[counter].GetComponent<RectTransform>().localRotation = Quaternion.Euler(pair.Value.Rotation);
            SocketContainers[counter].GetComponent<RectTransform>().sizeDelta = pair.Value.Dimensions;

            SocketContainers[counter].GetComponent<Image>().enabled = true;
            SocketContainers[counter].GetComponent<Image>().sprite = pair.Value.Sprite;
            counter++;
        }
    }
}
