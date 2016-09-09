using UnityEngine;
using System.Collections;

public class BackgroundObject : ObjectBase
{
	public Vector3 InitPosition
	{
		protected set;
		get;
	}

	protected virtual void Awake()
	{
		InitPosition = cachedTransform.localPosition;
	}

	public virtual void SetColor(Color color)
	{
	}
}

