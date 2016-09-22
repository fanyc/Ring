using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIWallet : MonoSingleton<UIWallet>
{
    public Image imgGold;
    public TMPro.TextMeshProUGUI textGold;
    public Image imgCube;
    public TMPro.TextMeshProUGUI textCube;
    public Image imgRuby;
    public TMPro.TextMeshProUGUI textRuby;

    protected float m_fGold = 0.0f;
    public float Gold
    {
        get
        {
            return m_fGold;
        }
        set
        {
            m_fGold = value;
            UIWallet.Instance.textGold.text = m_fGold.ToString();
        }
    }

    protected float m_fCube= 0.0f;
    public float Cube
    {
        get
        {
            return m_fCube;
        }
        set
        {
            m_fCube = value;
            UIWallet.Instance.textCube.text = m_fCube.ToString();
        }
    }

    protected float m_fRuby = 0.0f;
    public float Ruby
    {
        get
        {
            return m_fRuby;
        }
        set
        {
            m_fRuby = value;
            UIWallet.Instance.textRuby.text = m_fRuby.ToString();
        }
    }

    public void Init()
    {
        Gold = 0.0f;
        Cube = 0.0f;
        Ruby = 0.0f;
    }
}
