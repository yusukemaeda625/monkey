Shader "CustomShader/Toon" {
  Properties {
    _MainTex ("MainTex", 2D) = "white"{}
    _Color ("Color", Color) = (1,1,1,1)
    _DarkParam ("DarkAreaParam",Range(0,1)) = 0.8
    _MiddleParam ("MiddleAreaParam",Range(0,1)) = 0.3
    _MiddleColorParam ("MiddleColorParam", Range(0.0,1.0)) = 0.7
    _MiddleColor ("MiddleShadowColor", Color) = (1,1,1,1)
    _DarkColorParam ("DarkColorParam", Range(0.0,1.0)) = 0.5         
    _DarkColor ("DarkShadowColor", Color) = (1,1,1,1)
  }
  SubShader {
    
    Pass {
      Tags { "LightMode" = "ForwardBase" }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
                        
      uniform sampler2D _MainTex;
      uniform float4 _MainTex_ST;
      uniform float _MiddleColorParam;
      uniform float _DarkColorParam;
      uniform fixed4    _LightColor0;      
      uniform sampler2D _LightTexture0;
      uniform sampler2D _LightTextureB0;
      uniform float _DarkParam;
      uniform float _MiddleParam;
      uniform float4 _DarkColor;
      uniform float4 _MiddleColor;
      uniform float _White;
      uniform float4 _Color;
      uniform float4x4  unity_WorldToLight;
            
      struct appdata {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        float2 uv     : TEXCOORD0;
      };

     
      struct v2f {
        float4 pos    : SV_POSITION;
        float3 normal : NORMAL;
        float2 uv     : TEXCOORD0;              
      };      
      
      
      v2f vert(appdata v) {
        v2f o;
        o.pos    = UnityObjectToClipPos(v.vertex);
        o.normal = normalize(mul(v.normal, unity_WorldToObject).xyz);
        o.uv = v.uv;
        return o;
      }
      
      fixed4 frag(v2f i) : SV_Target {
        
        half3 normal   = normalize(i.normal);
        half3 lightDir = normalize(_WorldSpaceLightPos0.xyz);


        half NdotL = saturate(dot(normal, lightDir));
                
        fixed4 mainTex = tex2D(_MainTex, i.uv * _MainTex_ST.xy + _MainTex_ST.zw);         
        
        mainTex *= _Color;

        fixed3 toon = lerp(mainTex.rgb, mainTex.rgb * _DarkColorParam, step(NdotL, 0));                

        if(NdotL <= _DarkParam){
          toon = mainTex.rgb * _DarkColorParam * _DarkColor.rgb;
        }else if(NdotL <= _MiddleParam){
          toon = mainTex.rgb * _MiddleColorParam * _MiddleColor.rgb;
        }else{
          toon = mainTex.rgb;
        }

              
        fixed4 color = fixed4(toon, 1.0);
        
        return color;
      }
      ENDCG
    }
  }
}