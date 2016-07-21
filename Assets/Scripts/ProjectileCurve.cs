using UnityEngine;
using System.Collections;

public class ProjectileCurve : Projectile
{
	public AnimationCurve[] Curve;

	public ParticleSystem Particle;

	protected override IEnumerator _move(Vector3 targetPos)
	{
		AnimationCurve curve = Curve[Random.Range(0, Curve.Length)];
		Vector3 pos = cachedTransform.position;
		Vector3 Dist = targetPos - cachedTransform.position;
		float per = 0.0f;
		while(per < 1.0f)
		{
			cachedTransform.position = pos + Dist * per + new Vector3(0.0f, curve.Evaluate(per));
			per += Time.smoothDeltaTime * (Dist.magnitude * Speed);
			yield return null;
		}

		cachedTransform.position = targetPos;
	}

	protected override IEnumerator _afterMove()
	{
		Particle.Stop();
		if(Particle != null)
		{
			yield return new WaitForSeconds(Particle.startLifetime);
		}
	}
	
}
