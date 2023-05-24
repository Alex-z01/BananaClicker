using System;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    private Game game;
    private DataManager dataManager;
    private UIController uIController;

    List<UpgradePrefab> upgradePrefabs = new List<UpgradePrefab>();

    public event EventHandler OnBought;
    
    private void Start()
    {
        dataManager = Manager.Instance.dataManager;
        game = Manager.Instance.game;
        uIController = Manager.Instance.uIController;

        SpawnUpgradePrefabs();
        SubscribeToPurchases();
    }

    public void RaiseOnBoughtEvent()
    {
        OnBought?.Invoke(this, EventArgs.Empty);
    }

    void SubscribeToPurchases()
    {
        foreach (UpgradePrefab upgrade in upgradePrefabs)
        {
            upgrade.UpgradePurchased += OnUpgradePurchased;
        }
    }

    private void OnUpgradePurchased(object sender, EventArgs e)
    {
        Debug.Log($"Purchased event: {sender}, {e}");

        OnBought?.Invoke(this, e);
        UnlockUpgrades();
    }

    private void SpawnUpgradePrefabs()
    {
        foreach (KeyValuePair<string, UpgradeScriptableObject> pair in dataManager.defaultUpgradeData)
        {
            GameObject entry = null;

            switch (pair.Value.upgradeType)
            {
                case UpgradeScriptableObject.UpgradeType.Active:
                    entry = Instantiate(pair.Value.entryPrefab, uIController.upgradeContainers[0]);
                    break;

                case UpgradeScriptableObject.UpgradeType.Idle:
                    entry = Instantiate(pair.Value.entryPrefab, uIController.upgradeContainers[1]);
                    break;

                case UpgradeScriptableObject.UpgradeType.Stat:
                    entry = Instantiate(pair.Value.entryPrefab, uIController.upgradeContainers[2]);
                    break;

                case UpgradeScriptableObject.UpgradeType.Buff:
                    entry = Instantiate(pair.Value.entryPrefab, uIController.upgradeContainers[3]);
                    break;

            }
            entry.GetComponent<UpgradePrefab>().game = Manager.Instance.game;
            entry.GetComponent<UpgradePrefab>().banana = Manager.Instance.banana;
            entry.GetComponent<UpgradePrefab>().upgradeData = pair.Value;
            entry.GetComponent<UpgradePrefab>().Unlocked = game.CheckUpgradeUnlockRequirements(pair.Value);

            upgradePrefabs.Add(entry.GetComponent<UpgradePrefab>());
        }
    }

    public void UnlockUpgrades()
    {
        foreach(UpgradePrefab upgrade in upgradePrefabs)
        {
            upgrade.Unlocked = upgrade.Unlocked ? true : game.CheckUpgradeUnlockRequirements(upgrade.upgradeData);
        }
    }
}