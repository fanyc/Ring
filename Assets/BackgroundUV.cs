using UnityEngine;
using System.Collections;

public class BackgroundUV : Background
{
	protected Material m_material;

	protected override void Awake()
	{
		base.Awake();
		m_material = GetComponent<MeshRenderer>().material;
	}
	
	protected override void SetPosition(Vector2 position)
	{
		m_material.mainTextureOffset = new Vector2(position.x / Scale.x * Factor * m_material.mainTextureScale.x, 0.0f);
	}

	public override void SetColor(Color color)
	{
		m_material.color = color;
	}
}

