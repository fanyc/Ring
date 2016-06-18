using UnityEngine;
using System.Collections;

public class UISkillList : ObjectBase {
	public CharacterPlayer TargetCharacter;

	void Awake()
	{
		Init();
	}

	public void Init()
	{
		int count = TargetCharacter.ListSkillData.Count;

		for(int i = 0; i < count; ++i)
		{
			UIItemSkill item = ObjectPool<UIItemSkill>.Spawn("@ItemSkill");
			item.cachedTransform.SetParent(cachedTransform, false);
			item.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, i * -153.0f);
			item.Init(TargetCharacter.ListSkillData[i]);
		}

		GetComponent<RectTransform>().sizeDelta = new Vector2(0.0f, count * 153.0f - 10.0f);
	}
}
