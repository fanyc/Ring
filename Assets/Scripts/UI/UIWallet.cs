using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Numerics;

public class UIWallet : MonoSingleton<UIWallet>
{
    public Image imgGold;
    public TMPro.TextMeshProUGUI textGold;
    public Image imgCube;
    public TMPro.TextMeshProUGUI textCube;
    public Image imgRuby;
    public TMPro.TextMeshProUGUI textRuby;

    protected BigDecimal m_fGold = new BigDecimal(0);
    public BigDecimal Gold
    {
        get
        {
            return m_fGold;
        }
        set
        {
            m_fGold = value;
            UIWallet.Instance.textGold.text = m_fGold.ToUnit();
        }
    }

    protected BigDecimal m_fCube= new BigDecimal(0);
    public BigDecimal Cube
    {
        get
        {
            return m_fCube;
        }
        set
        {
            m_fCube = value;
            UIWallet.Instance.textCube.text = m_fCube.ToUnit();
        }
    }

    protected BigDecimal m_fRuby = new BigDecimal(0);
    public BigDecimal Ruby
    {
        get
        {
            return m_fRuby;
        }
        set
        {
            m_fRuby = value;
            UIWallet.Instance.textRuby.text = m_fRuby.ToUnit();
        }
    }

    public void Init()
    {
        Gold = new BigDecimal(0);
        Cube = new BigDecimal(0);
        Ruby = new BigDecimal(0);
    }
}
