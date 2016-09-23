using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


[RequireComponent(typeof(ScrollRect))]
public class ScrollList : MonoBehaviour
{
    public ScrollListCell CellOriginal;

    public Vector2 Padding;

    public bool Loop = true;

    protected RectTransform m_cachedRectTransform;
    protected ScrollRect m_cachedScrollRect;

    protected int m_nCount = 100;
    protected int m_nCursor = 0;
    protected int m_nPrevCursor = 0;
    protected Vector2 m_vecCellPerPage;

    protected ScrollListCell[,] m_Cell = null;

    public int Cursor
    {
        get
        {
            return m_nCursor;
        }

        set
        {
            m_nPrevCursor = m_nCursor;
            m_nCursor = value;//Mathf.Clamp(value, 0, m_Cell.GetLength(0));

            Draw();
        }
    }

    public int Count
    {
        get
        {
            return m_nCount;
        }

        set
        {
            if(m_Cell == null)
            {
                Init();
            }
            m_nCount = value;

            int width;
            int height;

            if(m_cachedScrollRect.horizontal && m_cachedScrollRect.vertical)
            {
                width = 0;
                height = 0;
            }
            else if(m_cachedScrollRect.horizontal)
            {
                width = Mathf.CeilToInt(m_nCount / m_vecCellPerPage.y);
                height = (int)m_vecCellPerPage.y;
            }
            else
            {
                width = (int)m_vecCellPerPage.x;
                height = Mathf.CeilToInt(m_nCount / m_vecCellPerPage.x);
            }
            m_cachedScrollRect.content.sizeDelta = new Vector2(
                Padding.x * 2.0f + (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x) * width - CellOriginal.Margin.x,
                Padding.y * 2.0f + (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y) * height - CellOriginal.Margin.y);

            Draw();
        }
    }

    void Awake()
    {
        m_cachedRectTransform = GetComponent<RectTransform>();
        m_cachedScrollRect = GetComponent<ScrollRect>();

    }

    void Start()
    {
        if(m_Cell == null)
        {
            Init();
        }
    }

    public void Init()
    {
        m_cachedScrollRect.content.anchoredPosition = Vector3.zero;
        
        m_vecCellPerPage.x = (m_cachedRectTransform.rect.width + CellOriginal.Margin.x - Padding.x * 2.0f) / (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x);
        m_vecCellPerPage.y = (m_cachedRectTransform.rect.height + CellOriginal.Margin.y - Padding.y * 2.0f) / (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y);

        if(m_cachedScrollRect.horizontal)
        {
            m_Cell = new ScrollListCell[Mathf.CeilToInt(m_vecCellPerPage.x) + 2, Mathf.FloorToInt(m_vecCellPerPage.y)];
        }
        else if(m_cachedScrollRect.vertical)
        {
            m_Cell = new ScrollListCell[Mathf.FloorToInt(m_vecCellPerPage.x), Mathf.CeilToInt(m_vecCellPerPage.y) + 2];
        }


        for(int x = 0; x < m_Cell.GetLength(0); ++x)
        {
            for(int y = 0; y < m_Cell.GetLength(1); ++y)
            {
                m_Cell[x, y] = Instantiate(CellOriginal).GetComponent<ScrollListCell>();
                m_Cell[x, y].gameObject.SetActive(true);
                m_Cell[x, y].cachedRectTransform.SetParent(m_cachedScrollRect.content);
                m_Cell[x, y].cachedRectTransform.localScale = Vector3.one;
                m_Cell[x, y].cachedRectTransform.anchoredPosition = new Vector2(
                    Padding.x + x * (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x),
                    -Padding.y + -y * (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y));
            }
        }

        Vector2 resize = new Vector2();

        if(m_cachedScrollRect.horizontal)
        {
            resize.x = Padding.x * 2.0f + (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x) * Mathf.Ceil((float)m_nCount / Mathf.Floor(m_vecCellPerPage.y)) - CellOriginal.Margin.x;
            resize.y = Padding.y * 2.0f + (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y) * (m_vecCellPerPage.y) - CellOriginal.Margin.y;
        }
        else if(m_cachedScrollRect.vertical)
        {
            resize.x = Padding.x * 2.0f + (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x) * (m_vecCellPerPage.x) - CellOriginal.Margin.x;
            resize.y = Padding.y * 2.0f + (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y) * Mathf.Ceil((float)m_nCount / Mathf.Floor(m_vecCellPerPage.x)) - CellOriginal.Margin.y;
        }
        m_cachedScrollRect.content.sizeDelta = resize;

        Draw();
        
        m_cachedScrollRect.content.anchoredPosition = new Vector2(0.0f, -0.1f);
    }

    public void Draw()
    {
        int width = m_Cell.GetLength(0);
        int height = m_Cell.GetLength(1);

        Vector2 resize = m_cachedScrollRect.content.rect.size;
        for(int x = 0; x < m_Cell.GetLength(0); ++x)
        {
            for(int y = 0; y < m_Cell.GetLength(1); ++y)
            {
                int index = 0;
                Vector2 position = Vector2.zero;

                if(m_cachedScrollRect.horizontal && m_cachedScrollRect.vertical)
                {
                }
                else if(m_cachedScrollRect.horizontal)
                {
                    index = (x - Cursor) % width;
                    if(index < 0) index += width;
                    index += Cursor;
                    position = new Vector2(
                        Padding.x + index * (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x),
                        -Padding.y + -y * (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y));

                    index = index * height + y;

                    if(Loop && Count > 0)
                    {
                        index %= (int)(Mathf.Ceil(Count / (float)height) * height);
                        resize.x = Padding.x * 2.0f + (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x) * Mathf.Ceil((Cursor + m_vecCellPerPage.x + 2.0f)) - CellOriginal.Margin.x;
                    }

                }
                else if(m_cachedScrollRect.vertical)
                {
                    index = (y - Cursor) % height;
                    if(index < 0) index += height;
                    index += Cursor;
                    position = new Vector2(
                        Padding.x + x * (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x),
                        -Padding.y + -index * (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y));
                    
                    index = index * width + x;

                    if(Loop && Count > 0)
                    {
                        index %= (int)(Mathf.Ceil(Count / (float)width) * width);
                        resize.y = Padding.y * 2.0f + (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y) * Mathf.Ceil((Cursor + m_vecCellPerPage.y + 2.0f)) - CellOriginal.Margin.y;
                    }
                }

                if(0 <= index &&
                   index < Count)
                {
                    m_Cell[x, y].gameObject.SetActive(true);
                    m_Cell[x, y].Draw(index);
                    m_Cell[x, y].cachedRectTransform.anchoredPosition = position;
                }
                else
                {
                    m_Cell[x, y].gameObject.SetActive(false);
                }
            }
        }

        if(Loop)
            m_cachedScrollRect.content.sizeDelta = resize;

    }

    void LateUpdate()
    {
        int newCursor = Cursor;
        if(m_cachedScrollRect.horizontal)
        {
            newCursor = Mathf.FloorToInt(-m_cachedScrollRect.content.anchoredPosition.x / (CellOriginal.cachedRectTransform.rect.size.x + CellOriginal.Margin.x));
        }
        else
        {
            newCursor = Mathf.FloorToInt(m_cachedScrollRect.content.anchoredPosition.y / (CellOriginal.cachedRectTransform.rect.size.y + CellOriginal.Margin.y));
        }
        if(Cursor != newCursor)
        {
            Cursor = newCursor;
        }
    }
}
