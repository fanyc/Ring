using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollListCell : MonoBehaviour
{
    protected int m_ID = 0;

    protected RectTransform m_cachedRectTransform;

    public RectTransform cachedRectTransform
    {
        get
        {
            return m_cachedRectTransform;
        }
    }

    public Vector2 Margin = new Vector2(0.0f, 0.0f);

    protected virtual void Awake()
    {
        m_cachedRectTransform = GetComponent<RectTransform>();
    }

    public virtual void Draw(int index)
    {
        m_ID = index;
    }
}
