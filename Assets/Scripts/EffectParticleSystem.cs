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

	IEnumerator _recycle()
	{
		yield return new WaitForSeconds((m_particle.duration + m_particle.startLifetime) / m_particle.playbackSpeed);
		m_particle?.Stop(true);
		Recycle();
	}
}
