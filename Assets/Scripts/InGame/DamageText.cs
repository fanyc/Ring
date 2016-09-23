using UnityEngine;
using System.Collections;

public class DamageText : ObjectBase
{	

	protected TMPro.TextMeshPro m_Text;
	protected TMPro.TextMeshPro m_Outline;
	
	void Awake()
	{
		m_Text = cachedTransform.GetChild(0).GetChild(1).GetComponent<TMPro.TextMeshPro>();
		m_Outline = cachedTransform.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshPro>();
		
	}

	public void Init(string text, Vector3 offset, Color startColor, Color endColor, Color outlineColor)
	{
		m_Text.text = m_Outline.text = text;
		m_Text.colorGradient = new TMPro.VertexGradient(startColor, startColor, endColor, endColor);
		m_Outline.color = outlineColor;
	}
}
