using UnityEngine;
using System.Collections;

public class ProjectileBoundingBox : Projectile
{
	protected ParticleSystem[] m_particle;

	protected System.Action<Collider2D> m_trigger;

	protected Collider2D m_cachedCollider;

	void Awake()
	{
		m_particle = GetComponentsInChildren<ParticleSystem>();
		m_cachedCollider = GetComponentInChildren<Collider2D>();
	}
	public void Init(Vector3 startPos, Vector3 targetPos, System.Action<Collider2D> trigger, System.Action release)
	{
		for(int i= 0; i < m_particle.Length; ++i)
		{
			m_particle[i].Clear();
			m_particle[i].Stop();
			m_particle[i].Play();
		}

		m_trigger = trigger;
		if(m_cachedCollider != null)
			m_cachedCollider.enabled = true;
		base.Init(startPos, targetPos, release);
	}

	protected override IEnumerator _afterMove()
	{	
		if(m_cachedCollider != null)
			m_cachedCollider.enabled = false;
		
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
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(m_trigger != null)
			m_trigger(col);
	}
}
