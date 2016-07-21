using UnityEngine;
using System.Collections;

public class Effect : ObjectBase {
	
	public Vector2 Offset;

	public virtual void Init(Vector3 position)
	{
		cachedTransform.localPosition = position + (Vector3)Offset;
	}

}
