using UnityEngine;
using System.Collections;

public class BackgroundObjects : Background
{
	protected BackgroundObject[] m_objects;
	protected override void Awake()
	{
		base.Awake();

		m_objects = GetComponentsInChildren<BackgroundObject>();
	}
	
	protected override void SetPosition(Vector2 position)
	{
		for(int i = 0; i < m_objects.Length; ++i)
		{
			Vector3 newPos = (Vector3)(position * -Factor) + m_objects[i].InitPosition;
			newPos.x = Scale.x * 0.5f + (-Scale.x * 0.5f + newPos.x) % Scale.x;
			m_objects[i].cachedTransform.localPosition = newPos;
		}
	}

	public override void SetColor(Color color)
	{
		for(int i = 0; i < m_objects.Length; ++i)
		{
			m_objects[i].SetColor(color);
		}
	}
}

