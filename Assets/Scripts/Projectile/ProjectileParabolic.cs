using UnityEngine;
using System.Collections;

public class ProjectileParabolic : Projectile
{
	public GameObject MainObject;
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

	protected override IEnumerator _move(Vector3 targetPos)
	{
		
		Vector3 vecDist = targetPos - cachedTransform.position;
		float fDist = vecDist.magnitude;
		float angle = (cachedTransform.eulerAngles.z + 360.0f) % 360.0f;

		float cross;
		Vector3 center;

		float direction = ((vecDist.x > 0.0f) ^ (angle > 180.0f) ? 1.0f : -1.0f);
		
		cross = (angle - 90.0f * direction);
		center = new Vector3(fDist * direction, fDist * direction * Mathf.Tan(cross * Mathf.Deg2Rad));

		float radius = center.magnitude;
		float arcAngle = (Mathf.Abs(180.0f - Mathf.Abs(cross) * 2.0f) + 360.0f) % 360.0f;
		float arcLength = 2.0f * Mathf.PI * radius * (arcAngle / 360.0f);
		float stepAngle = arcAngle * (Speed / arcLength);
		float stepLength = 2.0f * Mathf.PI * radius * (stepAngle / 360.0f);
		
		angle = cross + 180.0f;
		Vector3 start = cachedTransform.position;
		Vector3 position;
		while(true)
		{
			angle += stepAngle * Time.smoothDeltaTime * -direction;
			position = start + center + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * radius;
			cachedTransform.eulerAngles = new Vector3(0.0f, 0.0f, angle - 90.0f * direction );
			cachedTransform.position = position;

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
						count = Mathf.CeilToInt(m_listParticle[i].emission.rate.constantMax * stepLength * Time.smoothDeltaTime);
						break;
					}

					case ParticleSystemEmissionType.Time:
					{
						count = Mathf.CeilToInt(m_listParticle[i].emission.rate.constantMax * Time.smoothDeltaTime);
						break;
					}
				}
				
				if(count > 0)
				{
					m_listParticle[i].Emit(count);
					ParticleSystem.Particle[] buf = new ParticleSystem.Particle[count];
					m_listParticle[i].GetParticles(buf);
					
					for(int j = 0; j < count; ++j)
					{
						float p = (1.0f - j / (float)count);

						float a = (angle - stepAngle * Time.smoothDeltaTime * -direction * p);
						Vector3 newPos = start + center + new Vector3(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad)) * radius;
						a -= 90.0f * direction;
						buf[j].position = newPos;
						buf[j].lifetime -= (Time.smoothDeltaTime * p);
						buf[j].velocity = new Vector3(Mathf.Cos(a * Mathf.Deg2Rad), Mathf.Sin(a * Mathf.Deg2Rad)) * 0.2f;//Particle.velocityOverLifetime.x.Evaluate();
					}
					
					System.Array.ConstrainedCopy(buf, 0, m_buffer, size, count);
					m_listParticle[i].SetParticles(m_buffer, size + count);
				}
			}
			
			if(Mathf.Abs(targetPos.x - position.x) < Mathf.Abs(stepLength * Time.smoothDeltaTime))
			{
				break;
			}

			yield return null;
		}

		yield return null;
		MainObject?.SetActive(false);

		yield break;
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
		MainObject?.SetActive(true);
	}
}
