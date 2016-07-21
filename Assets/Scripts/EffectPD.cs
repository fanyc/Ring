using UnityEngine;
using System.Collections;

public class EffectPD : Effect
{
	protected SeventyOneSquared.PDUnity m_PD;

	void Awake()
	{
		m_PD = GetComponent<SeventyOneSquared.PDUnity>();
	}

	public override void Init(Vector3 position)
	{
		base.Init(position);
		m_PD.Running = true;
		StartCoroutine(_recycle());
	}

	IEnumerator _recycle()
	{
		while(m_PD.Running == true) yield return null;
		yield return new WaitForSeconds(m_PD.EmitterConfig.lifeSpan + m_PD.EmitterConfig.lifeSpanVariance);

		Recycle();
	}
}
