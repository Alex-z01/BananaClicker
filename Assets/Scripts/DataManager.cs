using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[Serializable]
public class GameData
{
    public BucketNumber bananaCount;
    public BucketNumber critChance;
    public float minCrit, maxCrit;
    public BucketNumber blackBananas;
    public double totalPrestiges;
    public Dictionary<string, GenericUpgrade> upgrades;

    public GameData(BucketNumber bananaCount, Dictionary<string, GenericUpgrade> upgrades, BucketNumber critChance, float minCrit, float maxCrit, BucketNumber blackBananas, double totalPrestiges)
    {
        this.bananaCount = bananaCount;
        this.upgrades = upgrades;
        this.critChance = critChance;
        this.minCrit = minCrit;
        this.maxCrit = maxCrit;
        this.blackBananas = blackBananas;
        this.totalPrestiges = totalPrestiges;
    }

    public override string ToString()
    {
        return $"Player Data:\nBananas:{bananaCount},\ncritChance:{critChance}, minCrit:{minCrit}x, maxCrit:{maxCrit}";
    }
}

[Serializable]
public class UnlockablesData
{
    public Dictionary<string, bool> unlockables;

    public UnlockablesData(Dictionary<string, bool> unlockables)
    {
        this.unlockables = unlockables;
    }
}

[Serializable]
public class InventoryData
{

}

public class DataManager : MonoBehaviour
{
    private Game game;
    private Gear gear;
    private Banana banana;
    private UIController uIController;
    private Prestige prestige;
    private CrateSystem crateSystem;
    private BalloonSystem balloonSystem;

    // Data
    public Dictionary<string, UpgradeScriptableObject> defaultUpgradeData = new Dictionary<string, UpgradeScriptableObject>();
    public Dictionary<string, EquippableScriptableObject> defaultEquippablesData = new Dictionary<string, EquippableScriptableObject>();
    public List<string> magnitudeNames = new List<string>();

    // Files
    public UpgradeScriptableObject[] upgradeScriptableObjects;
    public EquippableScriptableObject[] equippableScriptableObjects;

    private JsonSerializerSettings jsonSettings;

    private GameData gameData;
    private UnlockablesData unlockablesData;

    private void Awake()
    {
        jsonSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { new PolymorphicConverter() }
        };


        magnitudeNames = new List<string>() { 
            "", "", "m", "b", "t", "qd", "qt", "sx", "spt", "oct", 
            "non", "dec", "un", "duo", "tre", "quat", "quin", "sex", 
            "sept", "octo", "nov", "vig", "unvig", "duovig", "tresvig",
            "qtvig", "qinvig", "sesvig", "sptvig", "octovig", "novvig",
            "tigin", "untrig", "duotrig", "trestrig", "qttrig", "qintrig"};

        LoadUpgradeScriptableObjects();
        LoadItemScriptableObjects();
        PopulateDefaultUpgrades();
        PopulateDefaultItems();

        game = Manager.Instance.game;
        gear = Manager.Instance.gear;
        banana = Manager.Instance.banana;
        crateSystem = Manager.Instance.crateSystem;
        prestige = Manager.Instance.prestige;
        uIController = Manager.Instance.uIController;
        balloonSystem = Manager.Instance.balloonSystem;

        Load();
    }

    private void Start()
    {
        InvokeRepeating("Save", 60, 60);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            FreshGame();
        }
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void LoadUpgradeScriptableObjects()
    {
        upgradeScriptableObjects = Resources.LoadAll<UpgradeScriptableObject>("ScriptableObjects/Upgrades");
    }

    private void LoadItemScriptableObjects()
    {
        equippableScriptableObjects = Resources.LoadAll<EquippableScriptableObject>("ScriptableObjects/Items");
    }

    private void PopulateDefaultUpgrades()
    {
        foreach (UpgradeScriptableObject upgrade in upgradeScriptableObjects)
        {
            if (upgrade != null)
            {
                defaultUpgradeData.Add(upgrade.upgradeName, upgrade);
            }
        }

        List<KeyValuePair<string, UpgradeScriptableObject>> list = new List<KeyValuePair<string, UpgradeScriptableObject>>(defaultUpgradeData);

        list = list.OrderBy(pair => pair.Value.id)
            .ThenBy(pair => pair.Value.upgradeType)
            .ToList();

        defaultUpgradeData = list.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private void PopulateDefaultItems()
    {
        foreach (EquippableScriptableObject equippable in equippableScriptableObjects)
        {
            if (equippable != null)
            {
                defaultEquippablesData.Add(equippable.Item_Name, equippable);
            }
        }

        List<KeyValuePair<string, EquippableScriptableObject>> list = new List<KeyValuePair<string, EquippableScriptableObject>>(defaultEquippablesData);

        list = list.OrderBy(pair => pair.Value.id)
            .ThenBy(pair => pair.Value.Socket)
            .ToList();

        defaultEquippablesData = list.ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    /// <summary>
    /// Creates all the default upgrades and populates a dictionary with them
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, GenericUpgrade> InitializeUpgrades()
    {
        Dictionary<string, GenericUpgrade> upgrades = new Dictionary<string, GenericUpgrade>();

        // For each upgrade in the game, initialize to 0 owned and default unlock value
        foreach (KeyValuePair<string, UpgradeScriptableObject> pair in defaultUpgradeData)
        {
            // Check what type of upgrade it is 
            switch (pair.Value.upgradeType)
            {
                case UpgradeScriptableObject.UpgradeType.Active:
                    upgrades.Add(pair.Key, new ActiveUpgrade(
                                            BucketNumber.Zero,
                                            pair.Value.basePrice,
                                            pair.Value.defaultUnlocked,
                                            pair.Value.basePerClickValue
                        )
                    );
                    break;

                case UpgradeScriptableObject.UpgradeType.Idle:
                    upgrades.Add(pair.Key, new IdleUpgrade(
                                            BucketNumber.Zero,
                                            pair.Value.basePrice,
                                            pair.Value.defaultUnlocked,
                                            pair.Value.basePerSecondValue
                        )
                    );
                    break;

                case UpgradeScriptableObject.UpgradeType.Buff:
                    upgrades.Add(pair.Key, new BuffUpgrade(
                                            BucketNumber.Zero,
                                            pair.Value.basePrice,
                                            pair.Value.defaultUnlocked,
                                            pair.Value.buffValue,
                                            pair.Value.buffTargets
                        )
                     ); 
                    break;

                case UpgradeScriptableObject.UpgradeType.Stat:
                    upgrades.Add(pair.Key, new StatUpgrade(
                                            BucketNumber.Zero,
                                            pair.Value.basePrice,
                                            pair.Value.defaultUnlocked,
                                            pair.Value.statType,
                                            pair.Value.statChange
                        )
                     );
                    break;
            }
        }

        return upgrades;
    }

    /// <summary>
    /// Create all the Unlockables and populates a dictionary with them
    /// </summary>
    public Dictionary<string, Unlockable> InitializeUnlockables()
    {
        Dictionary<string, Unlockable> temp = new Dictionary<string, Unlockable>();

        Unlockable statUpgrades = new Unlockable("StatUpgrades",
                                    new List<IUnlockRequirement>() { new BananaRequirement(new BucketNumber(1, 0)) },
                                    new List<Unlockable.UnlockableUnlockedHandler>() { Stats.OnStatUpgradesUnlocked }
                                    );

        temp.Add("StatUpgrades", statUpgrades);

        Unlockable buffUpgrades = new Unlockable("BuffUpgrades",
                                    new List<IUnlockRequirement>() { new BananaRequirement(new BucketNumber(1, 1)) },
                                    new List<Unlockable.UnlockableUnlockedHandler>() { Stats.OnBuffUpgradesUnlocked }
                                    );
        temp.Add("BuffUpgrades", buffUpgrades);

        return temp;
    }

    public Dictionary<string, Equippable> InitializeItems()
    {
        Dictionary<string, Equippable> temp = new Dictionary<string, Equippable>();

        foreach(KeyValuePair<string, EquippableScriptableObject> pair in defaultEquippablesData)
        {
            EquippableScriptableObject scriptableObject = pair.Value;

            temp.Add(pair.Key, new Equippable(
                                scriptableObject.Item_Name,
                                scriptableObject.Item_Description,
                                scriptableObject.sprite,
                                scriptableObject.Socket,
                                scriptableObject.position,
                                scriptableObject.rotation,
                                scriptableObject.dimensions
                            ));
        }

        return temp;
    }

    public void FreshGame()
    {
        game.BananaCount = BucketNumber.Zero;
        game.BananasPerSecond = BucketNumber.Zero;
        game.DisplayBananaCount = BucketNumber.Zero;
        game.upgrades = InitializeUpgrades();
        game.unlockables = InitializeUnlockables();

        banana.BananaClickValue = new BucketNumber(1, 0);
        banana.critChance = BucketNumber.Zero;
        banana.critMultiplierRange = new Vector2(2, 3);

        prestige.BlackBananas = BucketNumber.Zero;
        prestige.totalPrestiges = 0;

        gear.All_Equippables = InitializeItems();

        Save();
    }

    public void Save()
    {
        GameData saveData = GetData();
        UnlockablesData unlockablesData = GetUnlockablesData();

        string dataJSON = JsonConvert.SerializeObject(saveData, Formatting.Indented, jsonSettings);
        string unlockablesJSON = JsonConvert.SerializeObject(unlockablesData, Formatting.Indented);

        File.WriteAllText(Application.dataPath + "/data.json", dataJSON);
        File.WriteAllText(Application.dataPath + "/unlockables.json", unlockablesJSON);

        uIController.statusText.text = "Saving...";
        uIController.statusText.GetComponent<TextUtility>().FadeOut(3f, false);
    }
    
    /// <summary>
    /// Get the latest game data
    /// </summary>
    /// <returns>Data</returns>
    private GameData GetData()
    {
        return new GameData(
                game.BananaCount, game.upgrades, banana.critChance,
                banana.critMultiplierRange.x, banana.critMultiplierRange.y,
                prestige.BlackBananas, prestige.totalPrestiges
            );
    }

    /// <summary>
    /// Get the latest unlockables data
    /// 
    /// Maps the default unlockables dictionary with the latest
    /// boolean values
    /// </summary>
    /// <returns></returns>
    private UnlockablesData GetUnlockablesData()
    {
        Dictionary<string, bool> temp = new Dictionary<string, bool>();

        foreach(KeyValuePair<string, Unlockable> pair in game.unlockables)
        {
            temp.Add(pair.Key, pair.Value.IsUnlocked);
        }

        return new UnlockablesData(temp);
    }

    public void Load()
    {
        if (!CheckFiles()) { return; }

        string dataJSON = File.ReadAllText(Application.dataPath + "/data.json");
        string unlockablesJSON = File.ReadAllText(Application.dataPath + "/unlockables.json");

        gameData = JsonConvert.DeserializeObject<GameData>(dataJSON, jsonSettings);
        unlockablesData = JsonConvert.DeserializeObject<UnlockablesData>(unlockablesJSON);

        ApplyLoadedData();
        ApplyLoadedUnlockables();

        uIController.statusText.text = "Loading...";
        uIController.statusText.GetComponent<TextUtility>().FadeOut(3f, false);
    }

    private bool CheckFiles()
    {
        if (!File.Exists(Application.dataPath + "/data.json") && 
            !File.Exists(Application.dataPath + "/unlockables.json"))
        {
            FreshGame();
            return false;
        }
        return true;
    }

    public void ApplyLoadedData()
    {
        // Simple data 
        game.upgrades = gameData.upgrades;
        game.BananaCount = gameData.bananaCount;

        prestige.BlackBananas = gameData.blackBananas;
        prestige.totalPrestiges = gameData.totalPrestiges;

        // Data that needs to be calculated from the upgrade information
        game.BananasPerSecond = Calculations.PerSecondValue;
        banana.BananaClickValue = Calculations.BaseOnClickValue;
        banana.critChance = Stats.CritChance;
        banana.critMultiplierRange = Stats.CritMultiplier;
        crateSystem.cratesUnlocked = Stats.CratesUnlocked;
        crateSystem.numCrateSpawners = Stats.CrateSpawnerCount;
        crateSystem.crateChance = Stats.CrateSpawnChance;
        crateSystem.crateSpawnRate = Stats.CrateSpawnRate;
        balloonSystem.balloonsUnlocked = Stats.BalloonsUnlocked;
        balloonSystem.balloonCount = Stats.BalloonSpawnCount;
        balloonSystem.balloonRewardChance = Stats.BalloonRewardChance;
    }

    public void ApplyLoadedUnlockables()
    {
        Dictionary<string, Unlockable> temp = InitializeUnlockables();

        foreach(KeyValuePair<string, bool> pair in unlockablesData.unlockables)
        {
            temp[pair.Key].IsUnlocked = pair.Value;
        }

        game.unlockables = temp;
    }

    

}
