using UnityEngine;
using System.Collections;

public class BackgroundObjectSprite : BackgroundObject
{
	protected SpriteRenderer m_cachedSpriteRenderer;

	protected override void Awake()
	{
		base.Awake();
		m_cachedSpriteRenderer = GetComponent<SpriteRenderer>();
	}
	public override void SetColor(Color color)
	{
		m_cachedSpriteRenderer.color = color;
	}		
}

