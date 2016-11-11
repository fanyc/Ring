using UnityEngine;
using System.Collections;

public class ProjectileArrow : Projectile
{
	public SpriteRenderer Sprite;
	protected ParticleSystem[] m_particle;

	void Awake()
	{
		m_particle = GetComponentsInChildren<ParticleSystem>();
	}
	public override void Init(Vector3 startPos, Vector3 targetPos, System.Action callback)
	{
		for(int i= 0; i < m_particle.Length; ++i)
		{
			m_particle[i].Clear();
			m_particle[i].Stop();
			m_particle[i].Play();
		}

		base.Init(startPos, targetPos, callback);
	}

	protected override IEnumerator _afterMove()
	{
		if(m_particle != null)
		{
			for(int i= 0; i < m_particle.Length; ++i)
				m_particle[i]?.Stop(true);
			
			float biggest = 0.0f;
			for(int i= 0; i < m_particle.Length; ++i)
			{
				if(m_particle[i] != null)
				{
					if(biggest < m_particle[i].startLifetime)
						biggest = m_particle[i].startLifetime;
				}
			}

			yield return new WaitForSeconds(biggest);
		}

		Recycle();
		if(Sprite != null)
		{
			Sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
			Sprite.enabled = true;
		}
	}
}
