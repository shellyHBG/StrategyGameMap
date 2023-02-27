using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject($"(Singleton){typeof(T).Name}");
                setInst(go.AddComponent<T>());
            }
            return _instance;
        }
    }

    protected virtual bool dontDestroyOnLoad { get; }

    private static T _instance { get; set; }

    private static void setInst(T instance)
    {
        _instance = instance;
        if (_instance.dontDestroyOnLoad)
            DontDestroyOnLoad(_instance);
    }
}
