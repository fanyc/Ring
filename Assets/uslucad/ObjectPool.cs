using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool
{
    private static Transform m_ObjectPoolRoot = new GameObject("ObjectPool").transform;

    public static Transform ObjectPoolRoot
    {
        get
        {
            return m_ObjectPoolRoot;
        }
    }
}

public class ObjectPool<ObjectType>
	where ObjectType : ObjectBase
{
    public static ObjectPool<ObjectType> Instance = new ObjectPool<ObjectType>();

    protected Dictionary<string, GameObject> m_Sources = new Dictionary<string, GameObject>();
    protected Dictionary<string, Queue<ObjectType>> m_ObjectPool = new Dictionary<string, Queue<ObjectType>>();

    public static void CreatePool(string poolName, GameObject source, int size = 100)
    {
        Instance.createPool(poolName, source, size);
    }

    protected void createPool(string poolName, GameObject source, int size)
    {
        if(m_Sources.ContainsKey(poolName))
        {
            Debug.LogError("ObjectPool : \"" + poolName + "\" has already been created.");
            return;
        }

        m_Sources.Add(poolName, source);

        Queue<ObjectType> queue = new Queue<ObjectType>(size);
        ObjectType obj;

        for(int i = 0; i < size; ++i)
        {
            obj = ((GameObject)Object.Instantiate(source)).GetComponent<ObjectType>();
            obj.cachedTransform.SetParent(ObjectPool.ObjectPoolRoot);
            obj.cachedGameObject.name = obj.cachedGameObject.name.Replace("(Clone)", "");
            obj.cachedTransform.position = new Vector3(10000.0f, 10000.0f, 0.0f);
            obj.cachedGameObject.SetActive(false);
            queue.Enqueue(obj);
        }

        m_ObjectPool.Add(poolName, queue);
    }

    public static ObjectType Spawn(string poolName, Vector3 localPosition = default(Vector3), Transform parent = null)
    {
        return Instance.spawn(poolName, localPosition, parent);
    }

    protected ObjectType spawn(string poolName, Vector3 localPosition, Transform parent)
    {
        if(m_ObjectPool.ContainsKey(poolName) == false)
        {
            Debug.LogError("ObjectPool : " + poolName + " is not found.");
            return null;
        }

        ObjectType obj = null;

        if(m_ObjectPool[poolName].Count > 0)
        {
            obj = m_ObjectPool[poolName].Dequeue();
        }
        else
        {
            obj = ((GameObject)Object.Instantiate(m_Sources[poolName])).GetComponent<ObjectType>();
            obj.cachedTransform.SetParent(ObjectPool.ObjectPoolRoot);
            obj.cachedGameObject.name = obj.cachedGameObject.name.Replace("(Clone)", "");
            obj.cachedTransform.position = new Vector3(10000.0f, 10000.0f, 0.0f);
        }

        obj.cachedGameObject.SetActive(true);
        if(parent)
        {
            obj.cachedTransform.SetParent(parent);
        }
        obj.cachedTransform.localPosition = localPosition;

        obj.CallbackRecycle = delegate
        {
            recycle(poolName, obj);
        };


        return obj;
    }

    protected void recycle(string poolName, ObjectType obj)
    {
        if(m_ObjectPool.ContainsKey(poolName) == false)
        {
            Debug.LogError("ObjectPool : " + poolName + " is not found.");
            return;
        }

        obj.cachedTransform.SetParent(ObjectPool.ObjectPoolRoot);
        obj.cachedTransform.position = new Vector3(10000.0f, 10000.0f);
        obj.cachedGameObject.SetActive(false);

        m_ObjectPool[poolName].Enqueue(obj);
    }
}