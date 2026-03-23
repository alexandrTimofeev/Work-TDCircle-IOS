using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GameGlobalDataPreload", menuName = "SGames/GameGlobalDataEmpty")]
public class GameGlobalDataPreload<T> : ScriptableObject where T : GameGlobalDataPreload<T>
{
   private static T fromResources;
    public static T FromResources
    {
        get
        {
            if (fromResources == null)
            {
                fromResources = Resources.Load<T>("GameGlobalDataPreload");
                fromResources.Init();
            }
            return fromResources;
        }
    }

    public virtual void Init()
    { }
}