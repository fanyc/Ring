using UnityEngine;
using System.Collections;

public abstract class Background : ObjectBase {

	public Vector2 Offset;
	public Vector2 Pivot = new Vector2(0.5f, 0.5f);
	
	public Vector2 Scale;
	public float Factor;
	protected CameraController m_camera;
	
	protected virtual void Awake()
	{
		CameraController.ListBackground.Add(this);
		m_camera = cachedTransform.parent.parent.GetComponent<CameraController>();
		Offset.x -= (0.5f - Pivot.x) * Scale.x;
		Offset.y -= (0.5f - Pivot.y) * Scale.y;
		
	}

	void LateUpdate () {

		if(Factor <= 1.0f)
		{
			cachedTransform.localScale = (Vector3)(Scale * (1.0f + (m_camera.Scale - 1.0f) * (1.0f - Factor)) );
		}
		else
		{
			cachedTransform.localScale = (Vector3)(Scale / (1.0f + (m_camera.Scale - 1.0f) * (Factor - 1.0f)) );
		}
		Vector3 newPos = (Vector3)Offset + new Vector3(0.0f, cachedTransform.localScale.y * (0.5f - Pivot.y) + m_camera.Offset.y - m_camera.cachedTransform.position.y, 10.0f);
		cachedTransform.localPosition = newPos;
		SetPosition(new Vector2(m_camera.cachedTransform.position.x, 0.0f));
	}

	protected abstract void SetPosition(Vector2 position);
	protected virtual void Reset()
	{
		Offset = (Vector2)transform.localPosition;
		Scale = (Vector2)transform.localScale;
	}

	public abstract void SetColor(Color color);
}

