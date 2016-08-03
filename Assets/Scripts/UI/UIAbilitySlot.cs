using UnityEngine;

public class UIAbilitySlot : MonoSingleton<UIAbilitySlot>
{
    public float MP;

    void Update()
    {
        MP += Time.deltaTime;
    }
}