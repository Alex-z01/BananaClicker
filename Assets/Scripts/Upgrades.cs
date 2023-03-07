using System;
using System.Collections.Generic;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    private Game game;
    private DataManager dataManager;
    private UIController uIController;

    List<UpgradePrefab> upgradePrefabs = new List<UpgradePrefab>();
    
    private void Start()
    {
        dataManager = Manager.Instance.dataManager;
        game = Manager.Instance.game;
        uIController = Manager.Instance.uIController;

        SpawnUpgradePrefabs();
        SubscribeToPurchases();
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
                    entry = Instantiate(pair.Value.entryPrefab, uIController.activeUpgradeContainer);
                    break;

                case UpgradeScriptableObject.UpgradeType.Idle:
                    entry = Instantiate(pair.Value.entryPrefab, uIController.idleUpgradeContainer);
                    break;

                case UpgradeScriptableObject.UpgradeType.Stat:
                    entry = Instantiate(pair.Value.entryPrefab, uIController.statUpgradeContianer);
                    break;

                case UpgradeScriptableObject.UpgradeType.Buff:
                    entry = Instantiate(pair.Value.entryPrefab, uIController.buffUpgradeContainer);
                    break;

            }
            entry.GetComponent<UpgradePrefab>().game = Manager.Instance.game;
            entry.GetComponent<UpgradePrefab>().banana = Manager.Instance.banana;
            entry.GetComponent<UpgradePrefab>().upgradeData = pair.Value;
            entry.GetComponent<UpgradePrefab>().Unlocked = game.CheckUnlockRequirements(pair.Value);

            upgradePrefabs.Add(entry.GetComponent<UpgradePrefab>());
        }
    }

    public void UnlockUpgrades()
    {
        foreach(UpgradePrefab upgrade in upgradePrefabs)
        {
            upgrade.Unlocked = upgrade.Unlocked ? true : game.CheckUnlockRequirements(upgrade.upgradeData);
        }
    }
}