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
        float angle = Random.Range(0.0f, 360.0f) * Mathf.Deg2Rad;
        vel.x = Mathf.Cos(angle);
        vel.y = Mathf.Sin(angle);
        vel.z = 0.0f;
        vel *= 24.0f * Random.Range(0.5f, 1.0f);
        m_vecVelocity = vel;
        m_fVelocity2 = 0.0f;
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
            dist = (Vector2)(m_tfTarget.position - pos);
            
            // pos += (Vector3)(dist.normalized * (24.0f * Time.smoothDeltaTime));
            // m_vecVelocity += (Vector3)(dist.normalized * (48.0f * Time.smoothDeltaTime));
            // m_vecVelocity = Vector3.ClampMagnitude(m_vecVelocity, 24.0f);
            
            pos += (Vector3)(dist.normalized * (m_fVelocity2 * Time.smoothDeltaTime));
            m_fVelocity2 += 48.0f * Time.smoothDeltaTime;
            
            if(m_vecVelocity.sqrMagnitude > Mathf.Pow(Time.smoothDeltaTime * 48.0f, 2.0f))
            {
                m_vecVelocity -= m_vecVelocity.normalized * (Time.smoothDeltaTime * 48.0f);
            }
            else
            {
                m_vecVelocity = Vector3.zero;
            }
            
            cachedTransform.position = pos;
            
            if(dist.sqrMagnitude < 10.0f * Time.smoothDeltaTime)
            {
                break;
            }
        }
        
        Release();
    }
    protected virtual void Release()
    {
        Recycle();
    }
}