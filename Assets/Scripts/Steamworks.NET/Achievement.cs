using System.Collections.Generic;

public class Achievement : IUnlockRequirement
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    private bool _isUnlocked;
    public bool IsUnlocked
    {
        get { return _isUnlocked; }
        set
        {
            _isUnlocked = value;
            if (_isUnlocked) {
                UnityEngine.Debug.Log($"Unlocked: {Name}");
                Steamworks.SteamUserStats.SetAchievement(Name);
                Steamworks.SteamUserStats.StoreStats();
            }
        }
    }
    public List<IUnlockRequirement> Requirements { get; private set; }

    public bool IsFulfilled()
    {
        throw new System.NotImplementedException();
    }

    public Achievement(string name, List<IUnlockRequirement> requirements)
    {
        Name = name;
        Requirements = requirements;
    }

    public bool CheckUnlockRequirements()
    {
        if (IsUnlocked) { return true; }

        foreach (IUnlockRequirement requirement in Requirements)
        {
            if (!requirement.IsFulfilled())
            {
                return false;
            }
        }

        return true;
    }
}

