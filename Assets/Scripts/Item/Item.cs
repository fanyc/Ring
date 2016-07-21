using UnityEngine;
using System.Collections;

public class Item : ObjectBase
{
    protected Transform m_tfTarget;
    protected Vector3 m_vecVelocity;
    protected float m_fVelocity2;
    
    
    protected void Init()
    {
        Vector3 vel;
        // float f = Random.Range(-1.0f, 1.0f);
        // Vector2 dist = (Vector2)(m_tfTarget.position - cachedTransform.position);
        // float angle = Mathf.Atan2(dist.y, dist.x) + (f * 180.0f) * Mathf.Deg2Rad;
        // vel.x = Mathf.Cos(angle);
        // vel.y = Mathf.Sin(angle);
        // vel.z = 0.0f;
        // vel *= Mathf.Abs(f) * 24.0f * Random.Range(0.5f, 1.0f);
        float f = Random.Range(0.0f, 1.0f);
        float angle = (60.0f + 20.0f * f) * Mathf.Deg2Rad;
        vel.x = Mathf.Cos(angle);
        vel.y = Mathf.Sin(angle);
        vel.z = 0.0f;
        vel *= 6.0f * (2.0f + f * 1.0f + Random.Range(0.0f, 1.0f));
        m_vecVelocity = vel;
        m_fVelocity2 = 10.0f;
        StartCoroutine("_update");
    }
       
    protected IEnumerator _update()
    {
        Vector3 pos = cachedTransform.position;
        Vector2 dist;
        while(true)
        {
            yield return null;
            pos += (m_vecVelocity) * Time.smoothDeltaTime;
            
            if(pos.y > 0.0f || m_vecVelocity.y > 0.0f)
            {
                m_vecVelocity.y -= 9.8f * 6.0f * Time.smoothDeltaTime;
            }
            else
            {
                pos.y = 0.0f;
                break;
            }

            cachedTransform.position = pos;
        }

        while(true)
        {
            yield return null;

            dist = (Vector2)(m_tfTarget.position - pos);

            if(dist.sqrMagnitude < (m_fVelocity2 * Time.smoothDeltaTime) * (m_fVelocity2 * Time.smoothDeltaTime))
            {
                break;
            }
            
            pos += (Vector3)(dist.normalized * (m_fVelocity2 * Time.smoothDeltaTime));
            m_fVelocity2 += 100.0f * Time.smoothDeltaTime;

            cachedTransform.position = pos;
        }
        
        Release();
    }
    protected virtual void Release()
    {
        Recycle();
    }
}