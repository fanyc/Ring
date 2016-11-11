using UnityEngine;
using System.Collections;

public class EffectParticleSystem : Effect
{
	protected ParticleSystem[] m_particle;

	void Awake()
	{
		m_particle = GetComponentsInChildren<ParticleSystem>();
	}

	public override void Init(Vector3 position)
	{
		base.Init(position);
		for(int i= 0; i < m_particle.Length; ++i)
			m_particle[i].Play(true);
		StartCoroutine(_recycle());
	}

	public override void Pause()
	{
		for(int i= 0; i < m_particle.Length; ++i)
			m_particle[i].Pause();
	}
	public override void Resume()
	{
		for(int i= 0; i < m_particle.Length; ++i)
			m_particle[i].Play();	
	}

	IEnumerator _recycle()
	{	
		if(m_particle != null)
		{
			for(int i= 0; i < m_particle.Length; ++i)
			{
				if(m_particle[i] != null && m_particle[i].isPlaying)
				{
					i = -1;
					yield return null;
				}
			}
			
			for(int i= 0; i < m_particle.Length; ++i)
				m_particle[i]?.Stop(true);
		}

		Recycle();
	}
}
