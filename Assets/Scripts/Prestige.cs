using System.Collections.Generic;
using UnityEngine;

public class Prestige : MonoBehaviour
{
    private BucketNumber _blackBananas;
    public BucketNumber BlackBananas
    {
        get { return _blackBananas; }
        set
        {
            _blackBananas = value;
            Manager.Instance.uIController.UpdateBlackBananaCount(value);
        }
    }

    public double totalPrestiges;

    private Game game;
    private Banana banana;
    private UIController uIController;
    private AudioControl audioControl;

    private void Start()
    {
        game = Manager.Instance.game;
        banana = Manager.Instance.banana;
        uIController = Manager.Instance.uIController;
        audioControl = Manager.Instance.audioControl;
    }

    public void Refresh()
    {
        // Update the UI values when activated
        uIController.UpdatePrestigeCount(Calculations.MaxBlackBananas);
        uIController.UpdatePrestigeCost(Calculations.TotalPrestigeCost);
    }

    public void PurchasePrestige()
    {
        // Check if can afford
        if (game.BananaCount < Calculations.TotalPrestigeCost)
        {
            // Play unavailable animation
            uIController.prestigeButton.GetComponent<UiUtility>().Shake(1f, 2f, uIController.prestigeButton.transform.localPosition);

            // Play unavailable sound effect
            GetComponent<AudioSource>().PlayOneShot(audioControl.UI_Clips[3]);

            return;
        }

        // Play purchased sound effect
        GetComponent<AudioSource>().PlayOneShot(audioControl.UI_Clips[0]);

        // Prestige
        SoftPrestige();

        // Return to default UI layout
        uIController.ShowLayout("Default");
    }

    public void SoftPrestige()
    {
        totalPrestiges++;
        BlackBananas += Calculations.MaxBlackBananas;

        game.upgrades = Manager.Instance.dataManager.InitializeUpgrades();
        game.unlockables = Manager.Instance.dataManager.InitializeUnlockables();

        game.BananaCount = BucketNumber.Zero;
        game.BananasPerSecond = BucketNumber.Zero;
        game.DisplayBananaCount = BucketNumber.Zero;

        banana.BananaClickValue = new BucketNumber(1, 0);
        banana.critChance = BucketNumber.Zero;
        banana.critMultiplierRange = new Vector2(2,3);
    }
}
