using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SeventyOneSquared;

public class UIRingButton : MonoSingleton<UIRingButton> {
    
    public PDUnity[] Normal;
    public PDUnity[] Boost;
    
    protected bool IsEnter = false;
    protected bool IsPressed = false;
    public bool IsCharging
    {
        get { return IsPressed && IsEnter; }
    }
    
    public void Press()
    {
        IsPressed = true;
        
        if(IsEnter && IsPressed)
        {
            for(int i = 0; i < Normal.Length; ++i)
                Normal[i].Running = false;
            for(int i = 0; i < Boost.Length; ++i)
                Boost[i].Running = true;
        }
    }
    
    public void Release()
    {
        IsPressed = false;
        
        if(!IsEnter || !IsPressed)
        {
            for(int i = 0; i < Normal.Length; ++i)
                Normal[i].Running = true;
            for(int i = 0; i < Boost.Length; ++i)
                Boost[i].Running = false;
        }
    }
    
	public void Enter()
    {
        IsEnter = true;
        
        if(IsEnter && IsPressed)
        {
            for(int i = 0; i < Normal.Length; ++i)
                Normal[i].Running = false;
            for(int i = 0; i < Boost.Length; ++i)
                Boost[i].Running = true;
        }
    }
    
    public void Exit()
    {
        IsEnter = false;
        
        if(!IsEnter || !IsPressed)
        {
            for(int i = 0; i < Normal.Length; ++i)
                Normal[i].Running = true;
            for(int i = 0; i < Boost.Length; ++i)
                Boost[i].Running = false;
        }
    }
    
    void Update()
    {
        if(IsCharging)
        {
            List<CharacterPlayer> list = GameManager.Instance.GetPlayers();
            
            for(int i = 0; i < list.Count; ++i)
            {
                list[i].MP += 1.0f * Time.deltaTime;
            }
        }
    }
}
