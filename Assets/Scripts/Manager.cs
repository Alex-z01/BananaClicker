using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance;

    public static Manager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Manager>();
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "Manager";
                    instance = go.AddComponent<Manager>();
                }
            }
            return instance;
        }
    }

    public AudioControl audioControl;
    public BalloonSystem balloonSystem;
    public Banana banana;
    public CrateSystem crateSystem;
    public DataManager dataManager;
    public Game game;
    public Gear gear;
    public Prestige prestige;
    public Upgrades upgrades;
    public UIController uIController;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
