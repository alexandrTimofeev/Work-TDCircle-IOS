using System;
using System.Collections;
using UnityEngine;
using System.Linq;

public static class AchieviementSystem
{
    private static Achiviement[] allAchiviements => AchievimentsData.AchivFromResource.AllAchiviements;

    public static bool IsUnlockAchiv (string id, bool testCondition = false, Achiviement[] allAchiviements = null)
    {
        if (allAchiviements == null)
            allAchiviements = AchieviementSystem.allAchiviements;

        return allAchiviements.Any((x) => x.Info.ID == id &&
        (x.IsUnlocked || (testCondition && x.CheckUnlock(true))));// || x.achievementBehaviour.IsConditionSuccess));
    }

    public static void ForceUnlock (string id)
    {
        foreach (var achiviement in allAchiviements)
        {
            if(achiviement.Info.ID == id)
            achiviement.ForceUnlock();
        }
    }

    public static AchivInfo[] GetAllAchivInfo()
    {
        AchivInfo[] achivInfos = new AchivInfo[allAchiviements.Length];
        for (int i = 0; i < achivInfos.Length; i++)
        {
            achivInfos[i] = allAchiviements[i].Info;
        }
        return achivInfos;
    }

    public static Achiviement GetAchiviementData (string ID)
    {
        return allAchiviements.FirstOrDefault(x => x.Info.ID == ID);
    }

    public static AchivInfo GetAchivInfo(string id)
    {
        foreach (var achiviement in allAchiviements)
        {
            if (achiviement.Info.ID == id)
                return achiviement.Info;
        }
        return null;
    }

    public static AchievimentsData LoadFromResource(string title) => Resources.Load<AchievimentsData>(title);
    public static Achiviement[] AllAchiviementsFromResource(string title) => LoadFromResource(title).AllAchiviements;

    public static AchivInfo[] GetAllAchivInfo(Achiviement[] achiviements)
    {
        AchivInfo[] achivInfos = new AchivInfo[achiviements.Length];
        for (int i = 0; i < achivInfos.Length; i++)
        {
            achivInfos[i] = achiviements[i].Info;
        }
        return achivInfos;
    }

    public static bool TestAllPlatina(params string[] iDBehaviourWithout)
    {
        Achiviement[] allAchiviements = AchieviementSystem.allAchiviements;
        foreach (var achiv in allAchiviements)
        {
            if (achiv.Behaviour is AchievementBehaviour beh)
            {
                if (iDBehaviourWithout != null && iDBehaviourWithout.Contains(beh.IDBehaviour))
                    continue;
                if (!beh.IsConditionSuccess)
                    return false;
            }
        }
        return true;
    }

    public static bool TestLocalPlatina(params string[] idsBehavioursTest)
    {
        Achiviement[] allAchiviements = AchieviementSystem.allAchiviements;
        foreach (var id in idsBehavioursTest)
        {
            var achiv = allAchiviements.FirstOrDefault(x => x.Behaviour.IDBehaviour == id);
            if (achiv == null || !(achiv.Behaviour is AchievementBehaviour platina) || !platina.IsConditionSuccess)
                return false;
        }
        return true;
    }
}