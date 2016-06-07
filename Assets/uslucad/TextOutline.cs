using UnityEngine;
using System.Collections;

public class TextOutline : MonoBehaviour
{

    public float pixelSize = 1;
    public Color outlineColor = Color.black;
    public bool resolutionDependant = false;
    public int doubleResolution = 1024;

    private TextMesh textMesh;
    private Transform textMeshTransform;
    private MeshRenderer meshRenderer;

    private TextMesh[] m_Outline = new TextMesh[8];
    private MeshRenderer[] m_OutlineRenderer = new MeshRenderer[8];

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        textMeshTransform = textMesh.transform;
        meshRenderer = GetComponent<MeshRenderer>();

        for(int i = 0; i < 8; i++)
        {
            GameObject outline = new GameObject("outline", typeof(TextMesh));
            outline.transform.parent = transform;
            outline.transform.localScale = new Vector3(1, 1, 1);

            m_Outline[i] = outline.GetComponent<TextMesh>();

            MeshRenderer otherMeshRenderer = outline.GetComponent<MeshRenderer>();
            otherMeshRenderer.material = new Material(meshRenderer.material);
            otherMeshRenderer.castShadows = false;
            otherMeshRenderer.receiveShadows = false;
            otherMeshRenderer.sortingLayerID = meshRenderer.sortingLayerID;
            otherMeshRenderer.sortingLayerName = meshRenderer.sortingLayerName;

            m_OutlineRenderer[i] = otherMeshRenderer;
        }
    }

    void LateUpdate()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        outlineColor.a = textMesh.color.a * textMesh.color.a;

        // copy attributes
        for(int i = 0; i < m_Outline.Length; i++)
        {
            TextMesh other = m_Outline[i];
            other.color = outlineColor;
            other.text = textMesh.text;
            other.alignment = textMesh.alignment;
            other.anchor = textMesh.anchor;
            other.characterSize = textMesh.characterSize;
            other.font = textMesh.font;
            other.fontSize = textMesh.fontSize;
            other.fontStyle = textMesh.fontStyle;
            other.richText = textMesh.richText;
            other.tabSize = textMesh.tabSize;
            other.lineSpacing = textMesh.lineSpacing;
            other.offsetZ = textMesh.offsetZ;

            bool doublePixel = resolutionDependant && (Screen.width > doubleResolution || Screen.height > doubleResolution);
            Vector3 pixelOffset = GetOffset(i) * (doublePixel ? 2.0f * pixelSize : pixelSize);
            pixelOffset.x *= textMeshTransform.lossyScale.x;
            pixelOffset.y *= textMeshTransform.lossyScale.y;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint + pixelOffset);
            other.transform.position = worldPoint;

            MeshRenderer otherMeshRenderer = m_OutlineRenderer[i];
            otherMeshRenderer.sortingLayerID = meshRenderer.sortingLayerID;
            otherMeshRenderer.sortingLayerName = meshRenderer.sortingLayerName;
        }
    }

    Vector3 GetOffset(int i)
    {
        switch(i % 8)
        {
        case 0:
            return new Vector3(0, 1, 0);
        case 1:
            return new Vector3(1, 1, 0);
        case 2:
            return new Vector3(1, 0, 0);
        case 3:
            return new Vector3(1, -1, 0);
        case 4:
            return new Vector3(0, -1, 0);
        case 5:
            return new Vector3(-1, -1, 0);
        case 6:
            return new Vector3(-1, 0, 0);
        case 7:
            return new Vector3(-1, 1, 0);
        default:
            return Vector3.zero;
        }
    }
}
