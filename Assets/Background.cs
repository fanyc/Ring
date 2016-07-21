using UnityEngine;
using System.Collections;

public class Background : ObjectBase {

	public Vector2 Offset;
	public Vector2 Pivot = new Vector2(0.5f, 0.5f);
	
	public Vector2 Scale;
	public float Factor;
	protected Material m_material;
	protected CameraController m_camera;
	void Start()
	{
		m_material = GetComponent<MeshRenderer>().material;
		m_camera = cachedTransform.parent.parent.GetComponent<CameraController>();
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
		cachedTransform.localPosition = (Vector3)Offset + new Vector3(0.0f, cachedTransform.localScale.y * (0.5f - Pivot.y) + m_camera.Offset.y - m_camera.cachedTransform.position.y, 10.0f);

		m_material.mainTextureOffset = new Vector2(m_camera.cachedTransform.position.x / Scale.x * Factor, 0.0f);
	}

	void Reset()
	{
		Offset = (Vector2)transform.localPosition;
		Scale = (Vector2)transform.localScale;
	}
}

