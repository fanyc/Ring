using UnityEngine;
using System.Collections;

public class ProjectileCurve : Projectile
{
	public AnimationCurve[] Curve;

	public Transform RotateObject;
	public ParticleSystem Particle;
	protected ParticleSystem.EmitParams m_origParticle;
	protected ParticleSystem.Particle[] m_particles;

	void Awake()
	{
		ParticleSystem.EmissionModule em = Particle.emission;
		em.enabled = false;
		
		m_origParticle.startColor = Particle.startColor;
		m_origParticle.startLifetime = Particle.startLifetime;
		m_origParticle.startSize = Particle.startSize;

		m_particles = new ParticleSystem.Particle[Particle.maxParticles];
	}
	protected override IEnumerator _move(Vector3 targetPos)
	{
		AnimationCurve curve = Curve[Random.Range(0, Curve.Length)];
		Vector3 pos = cachedTransform.position;
		Vector3 vecDist = targetPos - cachedTransform.position;
		float fDist = vecDist.magnitude;
		float duration = fDist / Speed;
		float per = 0.0f;
		//RotateObject.eulerAngles = new Vector3(0.0f, 0.0f, );
		// for(int i = 0; i < 720; ++i)
		// {
		// 	m_particles[i] = m_origParticle;
		// 	m_particles[i].position = cachedTransform.position + new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad)) * 5.0f;
		// 	m_particles[i].velocity = new Vector3(Mathf.Cos((i + 90) * Mathf.Deg2Rad), Mathf.Sin((i + 90) * Mathf.Deg2Rad)) * 0.2f;
		// 	m_particles[i].lifetime = 1.0f;
		// }
		// Particle.SetParticles(m_particles, 360);
		while(per < 1.0f)
		{	
			float step = Time.smoothDeltaTime / duration;
			per += step;
			if(per > 1.0f) per = 1.0f;

			if(per > 0.0f)
			{
				int size = Particle.GetParticles(m_particles);
				Particle.Clear();
				int count = Mathf.CeilToInt(Particle.emission.rate.constantMax * fDist * step);
				Particle.Emit(count);
				ParticleSystem.Particle[] buf = new ParticleSystem.Particle[count];
				Particle.GetParticles(buf);
				
				for(int i = 0; i < count; ++i)
				{
					float p = (1.0f - i / (float)count);
					
					Vector3 newPos = pos + vecDist * (per - (step * p)) + new Vector3(0.0f, curve.Evaluate(per - (step * p)));
					float angle = 0.0f;
					Vector3 vecStep = newPos - m_origParticle.position;
					angle = Mathf.Atan2(vecStep.y, vecStep.x) * Mathf.Rad2Deg;

					buf[i].position = newPos;
					buf[i].lifetime -= (Time.smoothDeltaTime * p);
					buf[i].velocity = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 0.2f;//Particle.velocityOverLifetime.x.Evaluate();
				}
				
				System.Array.ConstrainedCopy(buf, 0, m_particles, size, count);
				Particle.SetParticles(m_particles, size + count);
			}

			cachedTransform.position = pos + vecDist * per + new Vector3(0.0f, curve.Evaluate(per));
			yield return null;
		}

		cachedTransform.position = targetPos;
	}

	protected override IEnumerator _afterMove()
	{
		Particle.Stop();
		if(Particle != null)
		{
			while(Particle.isPlaying)
				yield return null;
		}

		System.Array.Clear(m_particles, 0, m_particles.Length);
	}
}
