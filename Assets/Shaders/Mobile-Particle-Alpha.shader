// Simplified Alpha Blended Particle shader. Differences from regular Alpha Blended Particle one:
// - no Tint color
// - no Smooth particle support
// - no AlphaTest
// - no ColorMask

Shader "Mobile/Particles/Alpha Blended Color" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Particle Texture", 2D) = "white" {}
    }
    
    Category {
        
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Back Lighting Off ZWrite Off
        
        BindChannels {
            Bind "Color", color
            Bind "Vertex", vertex
            Bind "TexCoord", texcoord
        }
        
        SubShader {        
            Pass {
                SetTexture[_MainTex] {
                    ConstantColor [_Color]
                    Combine Texture * constant
                }
            }
        }
    }    
}