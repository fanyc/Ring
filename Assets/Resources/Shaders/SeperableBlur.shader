Shader "Hidden/SeperableBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "black" {}
	}

	CGINCLUDE
	
	#include "UnityCG.cginc"
	#pragma glsl
	
	sampler2D	_MainTex;
	float		_SizeX;
	float		_SizeY;
	float4		_BlurDir;
	half		_BlurSpread;
	float4		_ChannelWeight;
	half2 		blurSize;
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;

		float4 uv1 : TEXCOORD1;
		float4 uv2 : TEXCOORD2;
		float4 uv3 : TEXCOORD3;
		float4 uv4 : TEXCOORD4;
		float4 uv5 : TEXCOORD5;
		float4 uv6 : TEXCOORD6;
		
	};
	
	//Common Vertex Shader
	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;

		half2 blurSize = _BlurDir.xy * _BlurSpread;
		
		o.uv1 =  v.texcoord.xyxy + half4(blurSize.xy, -blurSize.xy);
		o.uv2 =  v.texcoord.xyxy + half4(blurSize.xy, -blurSize.xy) * 2.0;
		o.uv3 =  v.texcoord.xyxy + half4(blurSize.xy, -blurSize.xy) * 3.0;
		o.uv4 =  v.texcoord.xyxy + half4(blurSize.xy, -blurSize.xy) * 4.0;
		o.uv5 =  v.texcoord.xyxy + half4(blurSize.xy, -blurSize.xy) * 5.0;
		o.uv6 =  v.texcoord.xyxy + half4(blurSize.xy, -blurSize.xy) * 6.0;

		return o;
	} 
	
	half4 frag(v2f IN) : COLOR
	{		
		half Scene = tex2D( _MainTex, IN.uv ).x * 0.1438749;
		
		Scene += tex2D( _MainTex, IN.uv1.xy ).x * 0.1367508;
		Scene += tex2D( _MainTex, IN.uv2.xy ).x * 0.1167897;
		Scene += tex2D( _MainTex, IN.uv3.xy ).x * 0.08794503;
		//Scene += tex2D( _MainTex, IN.uv4.xy ).x * 0.05592986;
		//Scene += tex2D( _MainTex, IN.uv5.xy ).x * 0.02708518;
		//Scene += tex2D( _MainTex, IN.uv6.xy ).x * 0.007124048;
		
		Scene += tex2D( _MainTex, IN.uv1.zw ).x * 0.1367508;
		Scene += tex2D( _MainTex, IN.uv2.zw ).x * 0.1167897;
		Scene += tex2D( _MainTex, IN.uv3.zw ).x * 0.08794503;
		//Scene += tex2D( _MainTex, IN.uv4.zw ).x * 0.05592986;
		//Scene += tex2D( _MainTex, IN.uv5.zw ).x * 0.02708518;
		//Scene += tex2D( _MainTex, IN.uv6.zw ).x * 0.007124048;

		return float4( Scene,0,0,0 );
	}
	ENDCG
	
	 
	Subshader {

		ZTest Off
		Cull Off
		ZWrite Off
		Fog { Mode off }
		
		//Pass 0 Blur
		Pass 
		{
			Name "Blur"
		
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest 
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		} 
	}
}
