using System.Collections;
using UnityEngine;

public class BalloonSystem : MonoBehaviour
{
    public GameObject balloonPrefab;

    private const float delayTime = 0.4f;
    public float swarmSpawnTime;
    public int balloonCount;
    public float balloonRewardChance;

    public void SpawnBalloon()
    {
        StartCoroutine(CBalloonSpawn());
    }

    IEnumerator CBalloonSpawn()
    {
        float swarmTimer = 0;
        float delayTimer = 0;
        int count = 0;

        //print($"Spawner {id} initialized");

        while (true)
        {
            swarmTimer += Time.deltaTime;

            //print($"Spawner {id} is active");

            if (swarmTimer >= swarmSpawnTime)
            {
                print("Start swarm");
                delayTimer += Time.deltaTime;

                if(delayTimer >= delayTime)
                {
                    print("Spawn 1");
                    GameObject balloon = Instantiate(balloonPrefab, Manager.Instance.uIController.HUD);

                    //print($"Spawner {id} spawned a crate");
                    float x = Random.Range(-800f, 700f);
                    float y = Random.Range(-800f, -600f);

                    balloon.transform.localPosition = new Vector3(x, y, 0);

                    count++;

                    if(count >= balloonCount) { delayTimer = 0; swarmTimer = 0; count = 0; }
                }
            }

            yield return null;
        }
    }
}
