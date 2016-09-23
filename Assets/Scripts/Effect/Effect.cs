using UnityEngine;
using System.Collections;

public class Effect : ObjectBase {
	
	public Vector2 Offset;
	
	protected bool m_isPlaying = false;
	public bool IsPlaying
	{
		get
		{
			return m_isPlaying;
		}
	}

	void OnDisable()
	{
		m_isPlaying = false;
	}

	public virtual void Init(Vector3 position)
	{
		cachedTransform.localPosition = position + (Vector3)Offset;
		m_isPlaying = true;
	}

	public virtual void Pause()
	{

	}

	public virtual void Resume()
	{
		
	}
}
