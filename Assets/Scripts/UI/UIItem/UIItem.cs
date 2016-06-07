using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UIItem : ObjectBase {

	public Image Thumbnail;
    
    public RectTransform Info;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Desc;
    
    public Button Function;
    
    public TextMeshProUGUI ButtonText;
    
    protected virtual void Awake()
    {
    }
    
    protected virtual void OnEnable()
    {
    }
    
    public virtual void Init()
    {
        
    }
    
    public virtual void _Function()
    {
        
    }
    
    // void Reset()
    // {
        
    //     Transform find;
        
    //     find = cachedTransform.FindChild("Thumbnail");
    //     if(find == null)
    //     {
    //         Thumbnail = new GameObject("Thumbnail").AddComponent<Image>();
    //         RectTransform rt = Thumbnail.GetComponent<RectTransform>();
    //         rt.SetParent(cachedTransform, false);
    //         rt.anchorMin = new Vector2(0.0f, 0.5f);
    //         rt.anchorMax = new Vector2(0.0f, 0.5f);
    //         rt.pivot = new Vector2(0.0f, 0.5f);
    //         rt.anchoredPosition = new Vector2(13.5f, 0.0f);
    //         rt.sizeDelta = new Vector2(192.0f, 192.0f);
    //     }
    //     else
    //     {
    //         Thumbnail = find.GetComponent<Image>();
    //     }
        
    //     find = cachedTransform.FindChild("Info");
    //     if(find == null)
    //     {
    //         Info = new GameObject("Info").AddComponent<RectTransform>();
    //         Info.SetParent(cachedTransform, false);
    //         Info.anchoredPosition = new Vector2(-59.75f, 0.0f);
    //         Info.sizeDelta = new Vector2(518.5f, 219.0f);
    //     }
    //     else
    //     {
    //         Info = find.GetComponent<RectTransform>();
    //     }
        
    //     find = Info.FindChild("Name");
    //     if(find == null)
    //     {
    //         Name = new GameObject("Name").AddComponent<Text>();
    //         RectTransform rt = Name.GetComponent<RectTransform>();
    //         rt.SetParent(Info, false);
    //         rt.anchorMin = new Vector2(0.0f, 0.5f);
    //         rt.anchorMax = new Vector2(1.0f, 0.5f);
    //         rt.pivot = new Vector2(0.0f, 1.0f);
    //         rt.anchoredPosition = new Vector2(20.0f, 75.0f);
    //         rt.sizeDelta = new Vector2(-0.5f, 40.0f);
            
    //         Name.font = Resources.Load<Font>("Fonts/NanumGothic");
    //         Name.fontStyle = FontStyle.Bold;
    //         Name.fontSize = 40;
    //         Name.horizontalOverflow = HorizontalWrapMode.Overflow;
    //         Name.verticalOverflow = VerticalWrapMode.Overflow;
    //     }
        
    //     find = Info.FindChild("Name");
    //     if(find == null)
    //     {
    //         Name = new GameObject("Name").AddComponent<Text>();
    //         RectTransform rt = Name.GetComponent<RectTransform>();
    //         rt.SetParent(Info, false);
    //         rt.anchorMin = new Vector2(0.0f, 0.5f);
    //         rt.anchorMax = new Vector2(1.0f, 0.5f);
    //         rt.pivot = new Vector2(0.0f, 1.0f);
    //         rt.anchoredPosition = new Vector2(20.0f, 75.0f);
    //         rt.sizeDelta = new Vector2(-0.5f, 40.0f);
            
    //         Name.font = Resources.Load<Font>("Fonts/NanumGothic");
    //         Name.fontStyle = FontStyle.Bold;
    //         Name.fontSize = 40;
    //         Name.horizontalOverflow = HorizontalWrapMode.Overflow;
    //         Name.verticalOverflow = VerticalWrapMode.Overflow;
    //     }
    // }
    
   
}
