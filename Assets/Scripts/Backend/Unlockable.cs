using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Unlockable
{
    public string Name { get; private set; }
    public bool IsUnlocked { get; set; }
    public List<IUnlockRequirement> Requirements { get; private set; }

    public delegate void UnlockableUnlockedHandler();
    public event UnlockableUnlockedHandler OnUnlocked;

    public Unlockable(string name, List<IUnlockRequirement> requirements, List<UnlockableUnlockedHandler> methods)
    {
        Name = name;
        Requirements = requirements;
        
        foreach(UnlockableUnlockedHandler handler in methods)
        {
            OnUnlocked += handler;
        }

        IsUnlocked = CheckUnlockRequirements();
    }

    public bool CheckUnlockRequirements()
    {
        if (IsUnlocked)
        {
            InvokeOnUnlock();
            return true;
        }

        foreach (IUnlockRequirement requirement in Requirements)
        {
            if(!requirement.IsFulfilled())
            {
                return false;
            }
        }

        InvokeOnUnlock();
        IsUnlocked = true;
        return true;
    }
   
    private void InvokeOnUnlock()
    {
        if (OnUnlocked != null)
        {
            OnUnlocked.Invoke();
        }
    }
}

public interface IUnlockRequirement
{
    bool IsFulfilled();
}

[Serializable]
public class BananaRequirement : IUnlockRequirement
{
    [SerializeField]
    private BucketNumber _requiredBananas;

    public BananaRequirement(BucketNumber requiredBananas)
    {
        _requiredBananas = requiredBananas;
    }

    public bool IsFulfilled()
    {
        return GetBananaCount() >= _requiredBananas;
    }

    private BucketNumber GetBananaCount()
    {
        return Manager.Instance.game.BananaCount;
    }
}

[Serializable]
public class UpgradeRequirement : IUnlockRequirement
{
    [SerializeField]
    private string _requiredUpgrade;
    [SerializeField]
    private BucketNumber _requiredCount;

    public UpgradeRequirement(string requiredUpgrade, BucketNumber requiredCount) 
    {
        _requiredUpgrade = requiredUpgrade;
        _requiredCount = requiredCount;
    }

    public bool IsFulfilled()
    {
        return GetUpgradeCount(_requiredUpgrade) >= _requiredCount;
    }

    private BucketNumber GetUpgradeCount(string name)
    {
        return Manager.Instance.game.upgrades[name].count;
    }
}

[Serializable]
public class TimeRequirement : IUnlockRequirement
{
    private float _time;

    public TimeRequirement(float time)
    {
        _time = time;
    }

    public bool IsFulfilled()
    {
        return GetTimePlayed() >= _time;
    }

    private float GetTimePlayed()
    {
        return Manager.Instance.game.timePlayed;
    }
}