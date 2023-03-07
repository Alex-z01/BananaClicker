using System.Collections;
using UnityEngine;

public class CrateSpawner : MonoBehaviour
{
    [HideInInspector] public GameObject cratePrefab;

    [HideInInspector] public float crateChance;
    [HideInInspector] public float crateLifeSpan;
    [HideInInspector] public float crateSpawnRate;
    public int id;

    public void SpawnCrate()
    {
        print("Spawning");
        StartCoroutine(CCrateSpawn());
    }

    public bool RollCrate()
    {
        float randChance = Random.Range(1, 101);

        return randChance <= crateChance;
    }

    IEnumerator CCrateSpawn()
    {
        float timer = 0;

        //print($"Spawner {id} initialized");

        while(true)
        {
            timer += Time.deltaTime;

            //print($"Spawner {id} is active");

            if(timer >= crateSpawnRate && RollCrate())
            {
                float x = Random.Range(-700f, 700f);
                float y = Random.Range(-430, 390f);
                    
                GameObject crate = Instantiate(cratePrefab, Manager.Instance.uIController.HUD);

                print($"Spawner {id} spawned a crate");

                crate.transform.localPosition = new Vector3(x, y, 0);
                crate.GetComponent<UiUtility>().Pop(0.1f, 2f, Vector2.zero, crate.transform.localScale);
                crate.GetComponent<UiUtility>().Shrink(crateLifeSpan, new Vector2(0.3f, 0.3f));
                
                Destroy(crate, crateLifeSpan);
                
                timer = 0; 
            }

            yield return null;
        }
    }
}
