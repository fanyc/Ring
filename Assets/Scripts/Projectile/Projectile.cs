using UnityEngine;
using System.Collections;

public class Projectile : ObjectBase
{
	public float Speed = 1.0f;
	
	public virtual void Init(Vector3 startPos, Vector3 targetPos, System.Action callback)
	{
		cachedTransform.position = startPos;
		StartCoroutine(_update(targetPos, callback));
	}

	

	protected virtual IEnumerator _update(Vector3 targetPos, System.Action callback)
	{
		
		IEnumerator coroutine = _beforeMove();
		while(coroutine.MoveNext()) yield return coroutine.Current;

		coroutine = _move(targetPos);
		while(coroutine.MoveNext()) yield return coroutine.Current;

		callback();

		coroutine = _afterMove();
		while(coroutine.MoveNext()) yield return coroutine.Current;

		Recycle();
	}

	
	protected virtual IEnumerator _beforeMove()
	{
		yield break;
	}

	protected virtual IEnumerator _move(Vector3 targetPos)
	{
		yield return null;
		Vector3 dist = targetPos - cachedTransform.position;

		while(dist.sqrMagnitude > (Speed * Time.smoothDeltaTime) * (Speed * Time.smoothDeltaTime))
		{
			cachedTransform.position += dist.normalized * (Speed * Time.smoothDeltaTime);
			dist = targetPos - cachedTransform.position;
			yield return null;
		}

		cachedTransform.position = targetPos;
	}

	protected virtual IEnumerator _afterMove()
	{
		yield break;
	}
}
