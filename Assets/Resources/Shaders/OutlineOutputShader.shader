Shader "Hidden/OutlineOutputShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
        _OutlineCol("OutlineColour", Color) = (0.0,0.0,1.0,1.0)
        _GradientStrengthModifier("Strength Modifier", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back Lighting Off ZWrite Off

        Pass
        {
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                sampler2D _MainTex;
                fixed4 _OutlineCol;
                float _GradientStrengthModifier;
                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col;
                    col = _OutlineCol;
                    col.a *= tex2D(_MainTex, i.uv).x * _GradientStrengthModifier;

                    // if (unBlurCol.r == 0.0 && unBlurCol.g == 0.0 && unBlurCol.b == 1.0) {
                    // 	col.a = 0.0;
                    // }
                    return col;
                }
            ENDCG
        }
	}
}