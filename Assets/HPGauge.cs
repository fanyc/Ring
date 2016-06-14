using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Numerics;
using System.Collections.Generic;

public class HPGauge : ObjectBase {

	protected static HashSet<HPGauge> m_listInstance = new HashSet<HPGauge>();

	protected Image m_imgGauge;

	void Awake()
	{
		m_imgGauge = GetComponent<Image>();
		m_listInstance.Add(this);
	}
	protected void _updateRatio()
	{
		if(GameManager.Instance.CurrentEnemy != null)
		{
			m_imgGauge.fillAmount = BigDecimal.Ratio(GameManager.Instance.CurrentEnemy.HP, GameManager.Instance.CurrentEnemy.MaxHP);
		}
	}

	public static void UpdateRatio()
	{
		var e = m_listInstance.GetEnumerator();
		while(e.MoveNext())
		{
			e.Current._updateRatio();
		}
	}
}
