using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UIElements;

[Serializable]
public class GameData
{
    public BucketNumber bananaCount;
    public BucketNumber critChance;
    public float minCrit, maxCrit;
    public float timePlayed;
    public BucketNumber blackBananas;
    public double totalPrestiges;
    public Dictionary<string, GenericUpgrade> upgrades;

    public GameData(BucketNumber bananaCount, Dictionary<string, GenericUpgrade> upgrades, BucketNumber critChance, float minCrit, float maxCrit, float timePlayed, BucketNumber blackBananas, double totalPrestiges)
    {
        this.bananaCount = bananaCount;
        this.upgrades = upgrades;
        this.critChance = critChance;
        this.minCrit = minCrit;
        this.maxCrit = maxCrit;
        this.timePlayed = timePlayed;
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
public class InventoryData<TPlayer, TDealer>
{
    [JsonProperty] public Inventory<TPlayer> PlayerInventory { get; set; }
    [JsonProperty] public Inventory<TDealer> DealerInventory { get; set; }

    public InventoryData()
    {
        PlayerInventory = new Inventory<TPlayer>(0);
        DealerInventory = new Inventory<TDealer>(0);
    }

    public InventoryData(Inventory<TPlayer> playerInventory, Inventory<TDealer> dealerInventory)
    {
        PlayerInventory = playerInventory;
        DealerInventory = dealerInventory;
    }
}

[Serializable]
public class TimerData
{
    public DateTime LogoutTime;
    public TimeSpan DealerTimer; 

    public TimerData(DateTime logoutTime, TimeSpan dealerTimer)
    {
        LogoutTime = logoutTime;
        DealerTimer = dealerTimer;
    }
}

public class DataManager : MonoBehaviour
{
    private Game game;
    private Gear gear;
    private Banana banana;
    private Dealer dealer;
    private UIController uIController;
    private Prestige prestige;
    private CrateSystem crateSystem;
    private BalloonSystem balloonSystem;

    // Data
    public Dictionary<string, UpgradeScriptableObject> defaultUpgradeData = new();
    public Dictionary<string, EquippableScriptableObject> defaultEquippablesData = new();
    public Dictionary<ItemSystem.TierEnum, TierScriptableObject> defaultTierData = new();
    public Dictionary<Stats.StatType, StatScriptableObject> defaultStatData = new();
    public List<string> magnitudeNames = new List<string>();

    // Files
    private UpgradeScriptableObject[] upgradeScriptableObjects;
    private EquippableScriptableObject[] equippableScriptableObjects;
    private TierScriptableObject[] tierScriptableObjects;
    private StatScriptableObject[] statScriptableObjects;

    private string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private string appName = "BananaClicker";
    private string saveFolderPath;

    private JsonSerializerSettings jsonSettings;

    private GameData _gameData;
    private UnlockablesData _unlockablesData;
    private InventoryData<Equippable, Equippable> _inventoryData;
    private TimerData _timersData;

    private void Awake()
    {
        jsonSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter> { 
                                    new PolymorphicConverter(),
                                    new DictionaryConverter()
                        }
        };

        saveFolderPath = Path.Combine(appDataFolderPath, appName);

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        magnitudeNames = new List<string>() { 
            "", "", "m", "b", "t", "qd", "qt", "sx", "spt", "oct", 
            "non", "dec", "un", "duo", "tre", "quat", "quin", "sex", 
            "sept", "octo", "nov", "vig", "unvig", "duovig", "tresvig",
            "qtvig", "qinvig", "sesvig", "sptvig", "octovig", "novvig",
            "tigin", "untrig", "duotrig", "trestrig", "qttrig", "qintrig"
        };

        // These are for loading in scriptables objects so that default data can be created 
        // must be run before any save data relatded functions happen
        LoadUpgradeScriptableObjects();
        LoadItemScriptableObjects();
        LoadTierScriptablesObjects();
        LoadStatScriptablesObjects();

        PopulateDefaultUpgrades();
        PopulateDefaultItems();
        InitializeTierObjects();
        InitializeStatObjects();

        GenerateDefaultObjects();

        game = Manager.Instance.game;
        gear = Manager.Instance.gear;
        banana = Manager.Instance.banana;
        dealer = Manager.Instance.dealer;
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

    public void QuitGame()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    /// <summary>
    /// Loads all the upgrade scriptable object files into an UpgradeScriptableObject array
    /// </summary>
    private void LoadUpgradeScriptableObjects()
    {
        upgradeScriptableObjects = Resources.LoadAll<UpgradeScriptableObject>("ScriptableObjects/Upgrades");
    }

    /// <summary>
    /// Loads all the equippable item scriptable object files into an EquippableScriptableObject array
    /// </summary>
    private void LoadItemScriptableObjects()
    {
        equippableScriptableObjects = Resources.LoadAll<EquippableScriptableObject>("ScriptableObjects/Items");
    }

    /// <summary>
    /// Loads all the tier scriptable object files into an TierScriptableObject array
    /// </summary>
    private void LoadTierScriptablesObjects()
    {
        tierScriptableObjects = Resources.LoadAll<TierScriptableObject>("ScriptableObjects/System/Tiers");
    }

    /// <summary>
    /// Loads all the stat scriptable object files into an StatScriptableObject array
    /// </summary>
    private void LoadStatScriptablesObjects()
    {
        statScriptableObjects = Resources.LoadAll<StatScriptableObject>("ScriptableObjects/System/Stats");
    }

    /// <summary>
    /// Populates the default upgrade data dictionary with the upgrade scriptable objects
    /// </summary>
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

    /// <summary> 
    /// Populates the default item data dictionary with the item scriptable objects
    /// </summary>
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

    private void InitializeTierObjects()
    {
        foreach (TierScriptableObject tier in tierScriptableObjects)
        {
            defaultTierData.Add(tier.tier, tier);
        }
    }

    private void InitializeStatObjects()
    {
        foreach (StatScriptableObject stat in statScriptableObjects)
        {
            defaultStatData.Add(stat.stat, stat);
        }
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

        Unlockable statUpgrades = new Unlockable("Stat_Upgrades",
                                    new List<IUnlockRequirement>() { 
                                        new UpgradeRequirement("Farmer", new BucketNumber(5, 0)),
                                        new UpgradeRequirement("Banana Bundle", new BucketNumber(5, 0))
                                    },
                                    new List<Unlockable.UnlockableUnlockedHandler>() { Stats.OnStatUpgradesUnlocked }
                                    );

        Unlockable buffUpgrades = new Unlockable("Buff_Upgrades",
                                    new List<IUnlockRequirement>() { 
                                        new BananaRequirement(new BucketNumber(300, 1)),
                                        new UnlockableRequirement(statUpgrades)
                                    },
                                    new List<Unlockable.UnlockableUnlockedHandler>() { Stats.OnBuffUpgradesUnlocked }
                                    );

        Unlockable prestige = new Unlockable("Prestige",
                                    new List<IUnlockRequirement>() { new BananaRequirement(new BucketNumber(1, 2)) },
                                    new List<Unlockable.UnlockableUnlockedHandler>() { Stats.OnPrestigeUnlocked }
                                    );

        Unlockable inventory = new Unlockable("Inventory",
                                    new List<IUnlockRequirement>() 
                                    {
                                        new UpgradeRequirement("Shady Dealer", new BucketNumber(1, 0))
                                    },
                                    new List<Unlockable.UnlockableUnlockedHandler>() { Stats.OnInventoryUnlocked }
                                    );

        Unlockable dealer = new Unlockable("Dealer",
                            new List<IUnlockRequirement>()
                            {
                                new UpgradeRequirement("Shady Dealer", new BucketNumber(1, 0))
                            },
                            new List<Unlockable.UnlockableUnlockedHandler>() { Stats.OnDealerUnlocked }
                            );

        temp.Add(statUpgrades.Name, statUpgrades);
        temp.Add(buffUpgrades.Name, buffUpgrades);
        temp.Add(prestige.Name, prestige);
        temp.Add(inventory.Name, inventory);
        temp.Add(dealer.Name, dealer);

        return temp;
    }

    public Dictionary<Gear.EquipSocket, Equippable> InitializeDefaultEquipSockets()
    {
        return new Dictionary<Gear.EquipSocket, Equippable>()
        {
            { Gear.EquipSocket.Head, null },
            { Gear.EquipSocket.LeftSide, null },
            { Gear.EquipSocket.RightSide, null },
            { Gear.EquipSocket.Feet, null },
            { Gear.EquipSocket.Skin, null },
        };
    }

    private void GenerateDefaultObjects()
    {
        _gameData = new GameData(BucketNumber.Zero, InitializeUpgrades(), BucketNumber.Zero, 0f, 0f, 0f, BucketNumber.Zero, 0f);

        Dictionary<string, bool> tempUnlockables = new Dictionary<string, bool>();

        foreach (KeyValuePair<string, Unlockable> pair in InitializeUnlockables())
        {
            tempUnlockables.Add(pair.Key, pair.Value.IsUnlocked);
        }

        _unlockablesData = new UnlockablesData(tempUnlockables);

        _inventoryData = new InventoryData<Equippable, Equippable>(new Inventory<Equippable>(10), new Inventory<Equippable>(4));

        _timersData = new TimerData(DateTime.Now, new TimeSpan(12, 0, 0));
    }

    public void Load()
    {
        CheckFiles();

        string dataJSON = File.ReadAllText(saveFolderPath + "/data.json");
        string unlockablesJSON = File.ReadAllText(saveFolderPath + "/unlockables.json");
        string inventoryJSON = File.ReadAllText(saveFolderPath + "/inventory.json");
        string timersJSON = File.ReadAllText(saveFolderPath + "/timers.json");

        JsonConvert.PopulateObject(dataJSON, _gameData, jsonSettings);
        JsonConvert.PopulateObject(unlockablesJSON, _unlockablesData);
        JsonConvert.PopulateObject(inventoryJSON, _inventoryData, jsonSettings);
        JsonConvert.PopulateObject(timersJSON, _timersData);

        ApplyLoadedData();

        uIController.statusText.text = "Loading...";
        uIController.statusText.GetComponent<TextUtility>().FadeOut(3f, false);
    }

    private void CheckFiles()
    {
        if (!File.Exists(saveFolderPath + "/data.json"))
        {
            string dataJSON = JsonConvert.SerializeObject(_gameData, Formatting.Indented, jsonSettings);
            File.WriteAllText(saveFolderPath + "/data.json", dataJSON);
        }
        
        if (!File.Exists(saveFolderPath + "/unlockables.json"))
        {
            string unlockablesJSON = JsonConvert.SerializeObject(_unlockablesData, Formatting.Indented);
            File.WriteAllText(saveFolderPath + "/unlockables.json", unlockablesJSON);
        }
        
        if (!File.Exists(saveFolderPath + "/inventory.json"))
        {
            string inventoryJSON = JsonConvert.SerializeObject(_inventoryData, Formatting.Indented, jsonSettings);
            File.WriteAllText(saveFolderPath + "/inventory.json", inventoryJSON);
        }
        
        if (!File.Exists(saveFolderPath + "/timers.json"))
        {
            string timersJSON = JsonConvert.SerializeObject(_timersData, Formatting.Indented);
            File.WriteAllText(saveFolderPath + "/timers.json", timersJSON);
        }

    }

    public void ApplyLoadedData()
    {
        ApplyLoadedUnlockables();
        ApplyLoadedTimers();
        ApplyLoadedInventory();
        ApplyLoadedEquippedSockets();

        // Simple data 
        game.upgrades = _gameData.upgrades;
        game.BananaCount = _gameData.bananaCount;
        game.timePlayed = _gameData.timePlayed;

        prestige.BlackBananas = _gameData.blackBananas;
        prestige.totalPrestiges = _gameData.totalPrestiges;

        // Data that needs to be calculated from the upgrade information
        game.BananasPerSecond = Calculations.PerSecondValue;
        banana.BananaClickValue = Calculations.BaseOnClickValue;

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
        Stats.Unlockables = InitializeUnlockables();

        foreach(KeyValuePair<string, bool> pair in _unlockablesData.unlockables)
        {
            Stats.Unlockables[pair.Key].IsUnlocked = pair.Value;
            Debug.Log(Stats.Unlockables[pair.Key].IsUnlocked);
        }
    }

    public void ApplyLoadedInventory()
    {
        gear.PlayerInventory = _inventoryData.PlayerInventory;
        dealer.DealerInventory = _inventoryData.DealerInventory;
    }

    public void ApplyLoadedEquippedSockets()
    {
        gear.EquipSockets = InitializeDefaultEquipSockets();

        foreach(Equippable equippable in gear.PlayerInventory)
        {
            if (!equippable.EquipedState) continue;

            // The equipsockets are references to their corresponding Equipables in inventory
            gear.EquipSockets[equippable.Socket] = equippable;
        }
    }

    public void ApplyLoadedTimers()
    {
        dealer.LogoutTime = _timersData.LogoutTime;
        dealer.DealerTimer = _timersData.DealerTimer;
    }

    public void Save()
    {
        GameData saveData = GetData();
        UnlockablesData unlockablesData = GetUnlockablesData();
        InventoryData<Equippable, Equippable> inventoryData = GetInventoryData();
        TimerData timersData = GetTimerData();

        string dataJSON = JsonConvert.SerializeObject(saveData, Formatting.Indented, jsonSettings);
        string unlockablesJSON = JsonConvert.SerializeObject(unlockablesData, Formatting.Indented);
        string inventoryJSON = JsonConvert.SerializeObject(inventoryData, Formatting.Indented, jsonSettings);
        string timersJSON = JsonConvert.SerializeObject(timersData, Formatting.Indented);

        File.WriteAllText(saveFolderPath + "/data.json", dataJSON);
        File.WriteAllText(saveFolderPath + "/unlockables.json", unlockablesJSON);
        File.WriteAllText(saveFolderPath + "/inventory.json", inventoryJSON);
        File.WriteAllText(saveFolderPath + "/timers.json", timersJSON);

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
                game.BananaCount, game.upgrades, Stats.CritChance,
                Stats.CritMultiplier.x, Stats.CritMultiplier.y,
                game.timePlayed, prestige.BlackBananas, prestige.totalPrestiges
            );
    }

    /// <summary>
    /// Get the latest unlockables data
    /// 
    /// Maps the default unlockables dictionary with the latest
    /// boolean values
    /// </summary>
    /// <returns>UnlockablesData</returns>
    private UnlockablesData GetUnlockablesData()
    {
        Dictionary<string, bool> temp = new Dictionary<string, bool>();

        foreach (KeyValuePair<string, Unlockable> pair in Stats.Unlockables)
        {
            temp[pair.Key] = pair.Value.IsUnlocked;
        }

        return new UnlockablesData(temp);
    }

    /// <summary>
    /// Get the latest inventory data
    /// </summary>
    /// <returns>InventoryData</returns>
    private InventoryData<Equippable, Equippable> GetInventoryData()
    {
        return new InventoryData<Equippable, Equippable>(gear.PlayerInventory, dealer.DealerInventory);
    }

    private TimerData GetTimerData()
    {
        return new TimerData(DateTime.Now, dealer.DealerTimer);
    }
}
