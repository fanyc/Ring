using UnityEngine;
using System.Collections;

public class ProjectileCurve : Projectile
{
	public AnimationCurve[] Curve;

	public Transform MainObject;
	public bool IsRotate = false;
	public ParticleSystem[] m_listParticle;
	protected ParticleSystem.EmitParams m_origParticle;
	protected ParticleSystem.Particle[] m_buffer;

	void Awake()
	{
		m_listParticle = GetComponentsInChildren<ParticleSystem>();

		int bufSize = 0;
		for(int i = 0; i < m_listParticle.Length; ++i)
		{
			if(m_listParticle[i].emission.type != ParticleSystemEmissionType.Distance) continue;
			ParticleSystem.EmissionModule em = m_listParticle[i].emission;
			em.enabled = false;

			if(bufSize < m_listParticle[i].maxParticles)
			{
				bufSize = m_listParticle[i].maxParticles;
			}
		}

		m_buffer = new ParticleSystem.Particle[bufSize];
	}

	public override void Init(Vector3 startPos, Vector3 targetPos, System.Action callback)
	{
		base.Init(startPos, targetPos, callback);
		MainObject?.gameObject.SetActive(true);
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
				for(int i = 0; i < m_listParticle.Length; ++i)
				{
					if(m_listParticle[i].emission.type != ParticleSystemEmissionType.Distance) continue;
					System.Array.Clear(m_buffer, 0, m_buffer.Length);
					int size = m_listParticle[i].GetParticles(m_buffer);
					m_listParticle[i].Clear();
					int count = 0;
					switch(m_listParticle[i].emission.type)
					{
						case ParticleSystemEmissionType.Distance:
						{
							count = Mathf.CeilToInt(m_listParticle[i].emission.rate.constantMax * fDist * step);
							break;
						}

						case ParticleSystemEmissionType.Time:
						{
							count = Mathf.CeilToInt(m_listParticle[i].emission.rate.constantMax * Time.smoothDeltaTime);
							break;
						}
					}
					m_listParticle[i].Emit(count);
					ParticleSystem.Particle[] buf = new ParticleSystem.Particle[count];
					m_listParticle[i].GetParticles(buf);
					
					for(int j = 0; j < count; ++j)
					{
						float p = (1.0f - j / (float)count);
						
						Vector3 newPos = pos + vecDist * (per - (step * p)) + new Vector3(0.0f, curve.Evaluate(per - (step * p)));
						float angle = 0.0f;
						Vector3 vecStep = newPos - m_origParticle.position;
						angle = Mathf.Atan2(vecStep.y, vecStep.x) * Mathf.Rad2Deg;

						buf[j].position = newPos;
						buf[j].lifetime -= (Time.smoothDeltaTime * p);
						buf[j].velocity = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * 0.2f;//Particle.velocityOverLifetime.x.Evaluate();
					}
					
					System.Array.ConstrainedCopy(buf, 0, m_buffer, size, count);
					m_listParticle[i].SetParticles(m_buffer, size + count);
				}
			}

			cachedTransform.position = pos + vecDist * per + new Vector3(0.0f, curve.Evaluate(per));
			yield return null;
		}

		cachedTransform.position = targetPos;
		MainObject?.gameObject.SetActive(false);
	}

	protected override IEnumerator _afterMove()
	{
		float wait = 0.0f;

		for(int i = 0; i < m_listParticle.Length; ++i)
		{
			m_listParticle[i].Stop();

			if(wait < m_listParticle[i].startLifetime)
			{
				wait = m_listParticle[i].startLifetime;
			}
		}

		yield return new WaitForSeconds(wait);
		Recycle();
	}
}
