using System;
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
    private AudioControl _audioControl;

    public event EventHandler OnPrestige;

    private void Start()
    {
        game = Manager.Instance.game;
        banana = Manager.Instance.banana;
        uIController = Manager.Instance.uIController;
        _audioControl = Manager.Instance.audioControl;
    }

    public void OpenPrestigeMenu()
    {
        if (!Stats.Unlockables["Prestige"].IsUnlocked)
        {
            uIController.menuButtons[2].GetComponentInChildren<UiUtility>().Shake(0.5f, 1, uIController.menuButtons[2].Find("lock").transform.localPosition);
            _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[1]);
            return;
        }

        uIController.ShowLayout("Prestige");

        RectTransform rectTransform = uIController.layoutList[2].layout.GetComponent<RectTransform>();

        StartCoroutine(AnimUtility.Slide(rectTransform, rectTransform.localPosition, uIController.layoutList[2].intendedLocation, 0.35f));

        Refresh();
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
            _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[3]);

            return;
        }

        // Play purchased sound effect
        _audioControl.sfx.PlayOneShot(_audioControl.UI_Clips[0]);

        // Prestige
        SoftPrestige();

        // Return to default UI layout
        uIController.ShowLayout("Default");
    }

    protected virtual void OnPrestigeEvent(EventArgs e)
    {
        OnPrestige?.Invoke(this, EventArgs.Empty);
    }

    public void SoftPrestige()
    {
        totalPrestiges++;

        BlackBananas += Calculations.MaxBlackBananas;

        Stats.Unlockables = Manager.Instance.dataManager.InitializeUnlockables();

        OnPrestigeEvent(EventArgs.Empty);
    }
}
