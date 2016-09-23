using UnityEngine;
using System.Collections;

public class EffectParticleSystem : Effect
{
	protected ParticleSystem m_particle;

	void Awake()
	{
		m_particle = GetComponent<ParticleSystem>();
	}

	public override void Init(Vector3 position)
	{
		base.Init(position);
		m_particle?.Play(true);
		StartCoroutine(_recycle());
	}

	public override void Pause()
	{
		m_particle?.Pause();
	}
	public override void Resume()
	{
		if(m_particle.isPaused)
			m_particle.Play();	
	}

	IEnumerator _recycle()
	{	
		if(m_particle != null)
		{
			while(m_particle.isPlaying)
				yield return null;
			m_particle?.Stop(true);
		}

		Recycle();
	}
}
