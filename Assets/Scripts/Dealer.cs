using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    private AudioControl _audioControl;
    private Game _game;
    private Gear _gear;
    private Prestige _prestige;
    private UIController _uIController;

    private bool _initialized = false;

    public GameObject SlotContainer, SlotPrefab;

    public TMP_Text TimerText;

    public DateTime LogoutTime;
    public TimeSpan DealerTimer;

    public Inventory<Equippable> DealerInventory = new(4);
    public List<DealerSlot> DealerSlots = new(4);

    private void Start()
    {
        _audioControl = Manager.Instance.audioControl;
        _game = Manager.Instance.game;
        _gear = Manager.Instance.gear;
        _prestige = Manager.Instance.prestige;
        _uIController = Manager.Instance.uIController;

        OfflineTimer();
        InvokeRepeating("Timer", 1, 1);
    }

    public void OpenDealer()
    {
        if (!Stats.Unlockables["Dealer"].IsUnlocked)
        {
            _uIController.menuButtons[3].GetComponentInChildren<UiUtility>().Shake(0.5f, 1, _uIController.menuButtons[3].Find("lock").transform.localPosition);
            _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[1]);
            return;
        }

        _uIController.ShowLayout("Dealer");

        InitializeDealer();
        RefreshView();
    }

    private void OfflineTimer()
    {
        TimeSpan offlineDuration = DateTime.Now - LogoutTime;

        DealerTimer -= offlineDuration;
    }

    private void Timer()
    {
        DealerTimer -= TimeSpan.FromSeconds(1);

        TimerText.text = DealerTimer.ToString("hh\\:mm\\:ss");

        if(DealerTimer <= TimeSpan.Zero)
        {
            DealerTimer = new TimeSpan(12, 0, 0);
            RegenarateInventory();
        }
    }

    private void InitializeDealer()
    {
        if (_initialized) return;

        for(int i = 0; i < 4; i++)
        {
            if (DealerInventory.Count < 4)
            {
                DealerInventory.Add(Manager.Instance.itemSystem.RandomEquippable());
            }

            GameObject GO = Instantiate(SlotPrefab, SlotContainer.transform);

            GO.GetComponent<DealerSlot>().Attached_Item = DealerInventory[i];
            DealerSlots.Add(GO.GetComponent<DealerSlot>());
        }

        _initialized = true;
    }

    private void RegenarateInventory()
    {
        for(int i = 0; i < DealerInventory.Count; i++)
        {
            Equippable item = Manager.Instance.itemSystem.RandomEquippable();

            DealerInventory.UpdateItem(DealerInventory[i], item);
        }

        RefreshView();
    }

    private void RefreshView()
    {
        for(int i = 0; i < DealerSlots.Count; i++)
        {
            try
            {
                DealerSlots[i].Attached_Item = DealerInventory[i];
                DealerSlots[i].Setup();
            }
            catch(ArgumentOutOfRangeException e)
            {
                DealerSlots[i].Attached_Item = null;
                DealerSlots[i].Setup();
            }
        }
    }

    private void RemoveItem(Equippable item)
    {
        if (item == null) return;

        // Unattach the item from its slot
        for (int i = 0; i < DealerSlots.Count; i++)
        {
            if (DealerSlots[i].Attached_Item != item) { continue; }

            DealerSlots[i].Attached_Item = null;
            break;
        }

        // Remove the item from the inventory
        DealerInventory.Remove(item);
    }

    public void SelectItem(Equippable item)
    {
        if (item == null) return;

        if (item.Item_Info.Currency == ItemInfo.CurrencyType.Bananas)
        {
            if (_game.DisplayBananaCount < item.Item_Info.Buy_Price) return;

            _game.DisplayBananaCount -= item.Item_Info.Buy_Price;
        }

        if (item.Item_Info.Currency == ItemInfo.CurrencyType.BlackBananas)
        {
            if (_prestige.BlackBananas < item.Item_Info.Buy_Price) return;

            _prestige.BlackBananas -= item.Item_Info.Buy_Price;
        }

        if (item.Item_Info.Currency == ItemInfo.CurrencyType.CosmicBananas)
        {
            // TODO cosmic banana implemenation 
            return;
        }

        _gear.PlayerInventory.Add(item);

        RemoveItem(item);

        RefreshView();
        Debug.Log($"Selected {item.Item_Name}:{item.Item_Info._ID}");
    }
}
