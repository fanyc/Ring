Shader "Uslucad/Test"
{
	Properties {
        _MainTex("Texture", 2D) = "white" {}
    } 
 
    SubShader {
 
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent" }
 
      ////////////////////////////////////////////////////////
      //Vertex-Fragment Functionality shader - RED          //
      ////////////////////////////////////////////////////////
      Blend SrcAlpha OneMinusSrcAlpha
        Cull Back Lighting Off ZWrite Off
        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct vertexInput
            {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
            float4 color : COLOR;
            };

            struct vertexOutput
            {
            float4 pos : POSITION;
            float2 uv : TEXCOORD0;
            float4 color : COLOR;
            };

            vertexOutput vert(vertexInput v) {
                vertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                float3 norm   = mul ((float3x3)UNITY_MATRIX_IT_MV, v.normal);
	            float2 offset = TransformViewToProjection(norm.xy);
                o.pos.xy += offset * 5.0f;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            sampler2D _MainTex;

            fixed4 frag( vertexOutput i) : SV_Target {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }

        ////////////////////////////////////////////////////////
        //Vertex-Fragment Functionality shader - GREEN          //
        ////////////////////////////////////////////////////////

        // Pass {
        //     CGPROGRAM

        //     #pragma vertex vert
        //     #pragma fragment frag

        //     struct vertexInput
        //     {
        //         float4 vertex : POSITION;
        //         float4 color : COLOR;
        //     };

        //     struct vertexOutput
        //     {
        //         float4 pos : POSITION;
        //         float4 color : COLOR;
        //     };

        //     vertexOutput vert(vertexInput v) {
        //         vertexOutput o;
        //         o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        //         o.color = v.color;
        //         return o;
        //     }

        //     fixed4 frag( vertexOutput i) : COLOR {
        //         return i.color;
        //     }

        //     ENDCG

        // }
    }
 
    Fallback "Diffuse"
}