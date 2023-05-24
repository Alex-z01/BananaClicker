using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateSystem : MonoBehaviour
{
    public int numCrateSpawners;
    public bool cratesUnlocked;

    public List<GameObject> CrateSpawners;

    public GameObject cratePrefab;

    [Header("Crate Stats")]
    public float crateChance;
    public float crateLifeSpan;
    public float crateSpawnRate;


    private Gear _gear;
    private Prestige _prestige;

    private void Start()
    {
        _gear = Manager.Instance.gear;
        _prestige = Manager.Instance.prestige;

        if(cratesUnlocked)
        {
            for (int i = 0; i < numCrateSpawners; i++)
            {
                AddCrateSpawner();
            }
            StartCrateSpawners();
        }

        Subscriptions();
    }


    private void Subscriptions()
    {
        _gear.EquipSocketChanged += OnItemChange;
        _prestige.OnPrestige += OnPrestige;
    }

    private void OnItemChange()
    {
        numCrateSpawners = Stats.CrateSpawnerCount;
        crateChance = Stats.CrateSpawnChance;
        crateSpawnRate = Stats.CrateSpawnRate;
    }

    private void OnPrestige(object sender, EventArgs args)
    {
        cratesUnlocked = false;
    }

    public void AddCrateSpawner()
    {
        GameObject GO = new GameObject("CrateSpawner" + CrateSpawners.Count.ToString());
        GO.AddComponent<CrateSpawner>();

        GO.GetComponent<CrateSpawner>().cratePrefab = cratePrefab;
        GO.GetComponent<CrateSpawner>().id = CrateSpawners.Count;
        GO.GetComponent<CrateSpawner>().crateChance = crateChance;
        GO.GetComponent<CrateSpawner>().crateLifeSpan = crateLifeSpan;
        GO.GetComponent<CrateSpawner>().crateSpawnRate = crateSpawnRate;

        CrateSpawners.Add(GO);
    }

    public void StartCrateSpawners()
    {
        StartCoroutine(CStartCrateSpawners());
    }

    public IEnumerator CStartCrateSpawners()
    {
        print("Trying to spawn crate spawners");
        yield return new WaitUntil(() => cratesUnlocked);
        print("Ready to spawn");
        for(int i = 0; i < CrateSpawners.Count; i++)
        {
            yield return new WaitForSeconds(2f);
            CrateSpawners[i].GetComponent<CrateSpawner>().SpawnCrate();
        }
    }
}
