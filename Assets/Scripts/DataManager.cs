using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

public class DataManager : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public BucketNumber bananaCount;
        public BucketNumber critChance;
        public float minCrit, maxCrit;
        public BucketNumber blackBananas;
        public double totalPrestiges;
        public Dictionary<string, GenericUpgrade> upgrades;

        public Data(BucketNumber bananaCount, Dictionary<string, GenericUpgrade> upgrades, BucketNumber critChance, float minCrit, float maxCrit, BucketNumber blackBananas, double totalPrestiges)
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

    private Game game;
    private Banana banana;
    private UIController uIController;
    private Prestige prestige;
    private CrateSystem crates;

    // Data
    public Dictionary<string, UpgradeScriptableObject> defaultUpgradeData = new Dictionary<string, UpgradeScriptableObject>();
    public List<string> magnitudeNames = new List<string>();

    // Files
    public UpgradeScriptableObject[] upgradeScriptableObjects;


    private JsonSerializerSettings jsonSettings;

    Data gameData;

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
        PopulateDefaultUpgrades();

        game = Manager.Instance.game;
        banana = Manager.Instance.banana;
        crates = Manager.Instance.crateSystem;
        prestige = Manager.Instance.prestige;
        uIController = Manager.Instance.uIController;

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

    public void FreshGame()
    {
        game.BananaCount = BucketNumber.Zero;
        game.BananasPerSecond = BucketNumber.Zero;
        game.DisplayBananaCount = BucketNumber.Zero;
        game.upgrades = InitializeUpgrades();

        banana.BananaClickValue = new BucketNumber(1, 0);
        banana.critChance = BucketNumber.Zero;
        banana.critMultiplierRange = new Vector2(2, 3);

        prestige.BlackBananas = BucketNumber.Zero;
        prestige.totalPrestiges = 0;

        Save();
    }

    public void Save()
    {
        Data saveData = new Data(
                game.BananaCount, game.upgrades, banana.critChance, 
                banana.critMultiplierRange.x, banana.critMultiplierRange.y, 
                prestige.BlackBananas, prestige.totalPrestiges
            );

        string superJson = JsonConvert.SerializeObject(saveData, Formatting.Indented, jsonSettings);
        File.WriteAllText(Application.dataPath + "/data.json", superJson);
        Debug.Log($"Saved {superJson}");

        uIController.statusText.text = "Saving...";
        uIController.statusText.GetComponent<TextUtility>().FadeOut(3f, false);
    }

    public void Load()
    {
        if(!File.Exists(Application.dataPath + "/data.json"))
        {
            uIController.statusText.text = "No load file found.";
            uIController.statusText.GetComponent<TextUtility>().FadeOut(3f, false);

            FreshGame();
            return;
        }

        string json = File.ReadAllText(Application.dataPath + "/data.json");
        gameData = JsonConvert.DeserializeObject<Data>(json, jsonSettings);

        print(json);
        InitializeLoadedData();

        uIController.statusText.text = "Loading...";
        uIController.statusText.GetComponent<TextUtility>().FadeOut(3f, false);
    }

    public void InitializeLoadedData()
    {
        // Simple data 
        game.upgrades = gameData.upgrades;
        game.BananaCount = gameData.bananaCount;

        prestige.BlackBananas = gameData.blackBananas;
        prestige.totalPrestiges = gameData.totalPrestiges;

        // Data that needs to be calculated from the base information
        game.BananasPerSecond = Calculations.PerSecondValue;
        banana.BananaClickValue = Calculations.BaseOnClickValue;
        banana.critChance = Stats.CritChance;
        banana.critMultiplierRange = Stats.CritMultiplier;
        crates.cratesUnlocked = Stats.CratesUnlocked;
    }



}
