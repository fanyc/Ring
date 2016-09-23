using UnityEngine;
using System;

public class ObjectBase : MonoBehaviour
{

    private Transform m_cachedTransform = null;
    private GameObject m_cachedGamaObject = null;

    private Action m_callbackRecycle = null;

    public Action CallbackRecycle
    {
        get
        {
            return m_callbackRecycle;
        }

        set
        {
            m_callbackRecycle = value;
        }
    }

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

    public virtual void Recycle()
    {
        if(m_callbackRecycle != null)
        {
            Action recycle = m_callbackRecycle;
            m_callbackRecycle = null;
            recycle();
        }
    }
}
