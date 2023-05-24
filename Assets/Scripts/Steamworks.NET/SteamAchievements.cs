using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class SteamAchievements : MonoBehaviour
{
    private List<Achievement> achievements = new List<Achievement>()
    {
        new Achievement("ACH_TIME_ONE", new List<IUnlockRequirement>()
        {
            new TimeRequirement(10)
        }),

        new Achievement("ACH_TIME_TWO",  new List<IUnlockRequirement>()
        {
            new TimeRequirement(300)
        }),

        new Achievement("ACH_TIME_THREE",  new List<IUnlockRequirement>()
        {
            new TimeRequirement(1800)
        }),

        new Achievement("ACH_RAKES_ONE", new List<IUnlockRequirement>()
        {
            new UpgradeRequirement("Banana Rake", new BucketNumber(10, 0))
        }),

        new Achievement("ACH_RAKES_TWO", new List<IUnlockRequirement>()
        {
            new UpgradeRequirement("Banana Rake", new BucketNumber(50, 0))
        }),
    };

    private void Start()
    {
        achievements.Add(new Achievement("ACH_STATS_UNLOCKED", new List<IUnlockRequirement>()
        {
            new UnlockableRequirement(Stats.Unlockables["Stat_Upgrades"])
        }));

        achievements.Add(new Achievement("ACH_PRESTIGE_UNLOCKED", new List<IUnlockRequirement>()
        {
            new UnlockableRequirement(Stats.Unlockables["Prestige"])
        }));

        achievements.Add(new Achievement("ACH_DEALER_UNLOCKED", new List<IUnlockRequirement>()
        {
            new UnlockableRequirement(Stats.Unlockables["Dealer"])
        }));

        InvokeRepeating("CheckAchievements", 1, 10);
    }


    // Call this function to unlock an achievement by name
    public void UnlockAchievement(string achievementName)
    {
        if(!SteamManager.Initialized) { return; }

        // Check if the player has already unlocked the achievement
        bool isAchievementUnlocked = false;
        SteamUserStats.GetAchievement(achievementName, out isAchievementUnlocked);
        if (isAchievementUnlocked)
        {
            // The achievement is already unlocked, do nothing
            return;
        }

        // Unlock the achievement
        SteamUserStats.SetAchievement(achievementName);
        SteamUserStats.StoreStats();
    }

    // Call this function to display the list of achievements and their descriptions
    public void DisplayAchievementsList()
    {
        foreach (Achievement achievement in achievements)
        {
            bool isAchievementUnlocked = false;
            SteamUserStats.GetAchievement(achievement.Name, out isAchievementUnlocked);
            string status = isAchievementUnlocked ? "Unlocked" : "Locked";
            Debug.Log($"{achievement.Name}: {achievement} ({status})");
        }
    }

    void CheckAchievements()
    {
        foreach(Achievement achievement in achievements)
        {
            if(achievement.IsUnlocked) { continue; }

            achievement.IsUnlocked = achievement.CheckUnlockRequirements();
        }
    }
}