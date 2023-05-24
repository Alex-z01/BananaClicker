using System;
using UnityEngine;
using UnityEngine.UI;

public class Balloon : MonoBehaviour
{
    public float time;
    private bool collected;

    private Upgrades _upgrades;

    private void Start()
    {
        float lifetime = UnityEngine.Random.Range(4f, 9f);

        _upgrades = Manager.Instance.upgrades;

        GetComponent<UiUtility>().SlideTo(gameObject, new Vector3(transform.localPosition.x, 700f), lifetime);
        Destroy(gameObject, lifetime);
    }

    public void Collect()
    {
        if (collected) { return; }

        string randomUpgrade = Calculations.BalloonReward;

        if(randomUpgrade != null)
        {
            collected = true;
            Manager.Instance.game.upgrades[randomUpgrade].count += 1;

            _upgrades.RaiseOnBoughtEvent();

            print($"Collected balloon, got {randomUpgrade}");

            GameObject GO = new GameObject();
            GO.transform.parent = Manager.Instance.uIController.mainCanvas.transform;
            GO.transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y + 150f);
            GO.AddComponent<Image>();
            GO.AddComponent<UiUtility>();

            GO.GetComponent<Image>().raycastTarget = false;
            GO.GetComponent<Image>().sprite = Manager.Instance.dataManager.defaultUpgradeData[randomUpgrade].sprite;
            GO.GetComponent<UiUtility>().SlideTo(GO, GO.transform.localPosition + new Vector3(0, 150, 0), 2f);
            GO.GetComponent<UiUtility>().FadeOut(2f, true);

        }

        GetComponent<Animator>().SetTrigger("pop");
        Destroy(gameObject, 0.267f);
    }


}
