using UnityEngine;
using System.Collections;

public class ProjectileArrow : Projectile
{
	public SpriteRenderer Sprite;
	public ParticleSystem TrailEffect;
	
	public override void Init(Vector3 startPos, Vector3 targetPos, System.Action callback)
	{
		Sprite.enabled = true;
		TrailEffect.Clear();
		TrailEffect.Stop();
		TrailEffect.Play();

		base.Init(startPos, targetPos, callback);
	}

	protected override IEnumerator _afterMove()
	{
		Sprite.enabled = false;
		yield return null;
		if(TrailEffect != null)
		{
			TrailEffect.Stop();
			yield return new WaitForSeconds(TrailEffect.startLifetime);
		}
	}
}
