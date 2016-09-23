using UnityEngine;
using System.Collections;
using Spine.Unity;

public class EffectSpine : Effect
{
	protected SkeletonAnimation m_cachedAnimation;
	public SkeletonAnimation SpineAnimation
	{
		get
		{
			return m_cachedAnimation;
		}
	}
    protected float timeScale = 1.0f;
	void Awake()
	{
		m_cachedAnimation = GetComponent<SkeletonAnimation>();
	}

	public override void Init(Vector3 position)
	{
		base.Init(position);
        timeScale = m_cachedAnimation.timeScale;
	}

	public override void Pause()
	{
        timeScale = m_cachedAnimation.timeScale;
		m_cachedAnimation.timeScale = 0.0f;
	}
	public override void Resume()
	{
        m_cachedAnimation.timeScale = timeScale;
	}

	public void PlayAnimation(string name, bool isLoop = false)
	{
		m_cachedAnimation.state.SetAnimation(0, name, isLoop);
	}

	public void AddAnimation(string name, bool isLoop = false)
	{
		m_cachedAnimation.state.AddAnimation(0, name, isLoop, 0.0f);
	}

	public bool IsEndAnimation()
	{
		return m_cachedAnimation.state.GetCurrent(0) == null || m_cachedAnimation.state.GetCurrent(0).Time >= m_cachedAnimation.state.GetCurrent(0).EndTime;
	}
}
