using UnityEngine;

public class Singleton<T>
    where T : new()
{
    private static T _instance = default(T);

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }
    }
}


public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            if(applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                "' already destroyed on application quit." +
                " Won't create again - returning null.");
                return null;
            }

            lock(_lock)
            {
                if(_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));

                    if(FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                        " - there should never be more than 1 singleton!" +
                        " Reopening the scene might fix it.");
                        return _instance;
                    }

                    if(_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                        " is needed in the scene, so '" + singleton +
                        "' was created with DontDestroyOnLoad.");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created: " +
                        _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }

    private Transform m_cachedTransform = null;
    private GameObject m_cachedGamaObject = null;


    public Transform cachedTransform
    {
        get
        {
            if(m_cachedTransform == null)
            {
                m_cachedTransform = transform;
            }

            return m_cachedTransform;
        }
    }

    public GameObject cachedGameObject
    {
        get
        {
            if(m_cachedGamaObject == null)
            {
                m_cachedGamaObject = gameObject;
            }

            return m_cachedGamaObject;
        }
    }
}