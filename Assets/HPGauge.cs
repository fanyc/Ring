using UnityEngine;
using UnityEngine.UI;

public class HPGauge : ObjectBase {

	protected static RectTransform m_Root;
	
	public static RectTransform Root
	{
		get
		{
			if(m_Root == null)
			{
				m_Root = GameObject.Find("HPGaugeRoot").GetComponent<RectTransform>();
			}
			return m_Root;
		}
	}

	private static bool m_CanvasRectInit = false;
	private static Rect m_CanvasRect;
	private static Rect CanvasRect
	{
		get
		{
			if(m_CanvasRectInit == false)
			{
				m_CanvasRectInit = true;
				m_CanvasRect = Root.parent.GetComponent<RectTransform>().rect;
			}

			return m_CanvasRect;
		}
	}


	protected RectTransform m_RectTransform;

	protected Character m_Target;

	public Image Gauge;

	void Awake()
	{
		m_RectTransform = GetComponent<RectTransform>();
	}
	public void Init(Character target)
	{
		m_Target = target;
		m_RectTransform.SetParent(Root, false);
		
		UpdatePosition();
		UpdateRatio();

		Show();
	}

	public void Show()
	{
		cachedGameObject.SetActive(true);
	}

	public void Hide()
	{
		cachedGameObject.SetActive(false);
	}

	public void UpdateRatio()
	{
		if(m_Target != null)
		{
			Gauge.fillAmount = m_Target.HP / m_Target.MaxHP;
		}
	}

	void LateUpdate()
	{
		UpdatePosition();
	}

	void UpdatePosition()
	{
		Vector2 ViewportPosition=CameraController.Instance.cachedCamera.WorldToViewportPoint(m_Target.cachedTransform.position);
		Vector2 WorldObject_ScreenPosition=new Vector2(
		((ViewportPosition.x*CanvasRect.size.x)-(CanvasRect.size.x*0.5f)),
		((ViewportPosition.y*CanvasRect.size.y)-(CanvasRect.size.y*0.5f)) + m_Target.HPGaugeHeight / CameraController.Instance.Scale);

		m_RectTransform.anchoredPosition = WorldObject_ScreenPosition;
		m_RectTransform.localScale = Vector3.one / CameraController.Instance.Scale;
	}
}
