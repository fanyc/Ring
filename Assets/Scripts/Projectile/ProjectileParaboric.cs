using UnityEngine;
using System.Collections;

public class ProjectileParaboric : Projectile
{
	protected override IEnumerator _move(Vector3 targetPos)
	{
		Vector3 dist = targetPos - cachedTransform.position;
		float time = dist.x / Speed;
		float rotateTime = Random.Range(0.75f, 1.0f) * time;
		float delay = Random.Range(0.0f, 1.0f) * (time - rotateTime);

		float angle = cachedTransform.eulerAngles.z;
		float destAngle = 0.0f;

		while(delay > 0.0f)
		{
			delay -= Time.smoothDeltaTime;
			cachedTransform.position += new Vector3(Speed, Speed * Mathf.Tan(angle * Mathf.Deg2Rad)) * Time.smoothDeltaTime;
			yield return null;
		}
		Vector3 vecDestAngle = (targetPos - (cachedTransform.position + new Vector3(rotateTime * Speed, 0.0f))).normalized;
		destAngle = Mathf.Atan2(vecDestAngle.y, vecDestAngle.x) * Mathf.Rad2Deg;

		Debug.Log(rotateTime);
		float per = 0.0f;
		while(per < 1.0f)
		{ 
			cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.LerpAngle(angle, destAngle, per));//angle + (destAngle - angle) * per

			per = Mathf.Clamp01(per + Time.smoothDeltaTime / rotateTime);
			cachedTransform.position += new Vector3(Speed, Speed * Mathf.Tan(cachedTransform.eulerAngles.z * Mathf.Deg2Rad)) * Time.smoothDeltaTime;
			
			yield return null;
		}

		Debug.Log(cachedTransform.position + "	" + cachedTransform.eulerAngles.z);
		

		dist = targetPos - cachedTransform.position;
		while(dist.sqrMagnitude > (Speed * Time.smoothDeltaTime) * (Speed * Time.smoothDeltaTime))
		{
			cachedTransform.position += dist.normalized * (Speed * Time.smoothDeltaTime);
			dist = targetPos - cachedTransform.position;
			yield return null;
		}

		cachedTransform.position = targetPos;
	}
	
}
