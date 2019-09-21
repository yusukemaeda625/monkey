Shader "CustomShader/GrayScale"
{
    Properties{
        _MainTex ("_MainTex", 2D) = "white"{}
    }
    
    SubShader{
        Cull Off
        ZTest Always
        ZWrite Off
        
        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag 
            
            uniform sampler2D _MainTex;
            
            struct v2f{
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct appdata{
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };
            
            v2f vert(appdata v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                fixed4 color = tex2D(_MainTex,i.uv);
                color.rgb = dot(color.rgb,half3(0.30,0.59,0.11));
                return color;
            }
            ENDCG
        }       
    }
}