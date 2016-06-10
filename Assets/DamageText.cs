using UnityEngine;
using System.Collections;

public class DamageText : ObjectBase
{	
	protected TMPro.TextMeshPro m_Text;

	void Awake()
	{
		m_Text = cachedTransform.GetChild(0).GetComponent<TMPro.TextMeshPro>();
	}

	public void Init(string text)
	{
		m_Text.text = text;
	}
}
