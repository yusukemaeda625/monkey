Shader "CustomShader/bloom"
{
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Bloom ("Bloom", Range (0, 1)) = 0.5
    _Radius ("Radius", Float) = 9
  }

  CGINCLUDE  
  struct appdata {
    float4 vertex : POSITION;
    float2 uv     : TEXCOORD0;
  };

  
  struct v2f {
    float4 pos : SV_POSITION;
    float2 uv  : TEXCOORD0;
  };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv  = v.uv;
    return o;
  }
  ENDCG

  SubShader {
    Cull Off ZWrite Off ZTest Always
    
    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      
      uniform sampler2D _MainTex;
      uniform half      _Bloom;


      fixed4 frag(v2f i) : SV_Target {
        fixed4 color = tex2D(_MainTex, i.uv);
        
        half y = dot(color.rgb, half3(0.30, 0.59, 0.11));
        color.rgb *= step(_Bloom, y);

        return color;
      }
      ENDCG
    }
    
    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      
      uniform sampler2D _MainTex;
      uniform float4    _MainTex_TexelSize;
      uniform half2     _Direction;
      uniform half      _Radius;

      fixed4 frag(v2f i) : SV_Target {
        half4 color = tex2D(_MainTex, i.uv);
        
        float weights[5] = { 0.22702702702, 0.19459459459, 0.12162162162, 0.05405405405, 0.01621621621 };
        float2 offset = _Direction * _MainTex_TexelSize.xy * _Radius;
        color.rgb *= weights[0];
        color.rgb += tex2D(_MainTex, i.uv + offset      ).rgb * weights[1];
        color.rgb += tex2D(_MainTex, i.uv - offset      ).rgb * weights[1];
        color.rgb += tex2D(_MainTex, i.uv + offset * 2.0).rgb * weights[2];
        color.rgb += tex2D(_MainTex, i.uv - offset * 2.0).rgb * weights[2];
        color.rgb += tex2D(_MainTex, i.uv + offset * 3.0).rgb * weights[3];
        color.rgb += tex2D(_MainTex, i.uv - offset * 3.0).rgb * weights[3];
        color.rgb += tex2D(_MainTex, i.uv + offset * 4.0).rgb * weights[4];
        color.rgb += tex2D(_MainTex, i.uv - offset * 4.0).rgb * weights[4];

        return color;
      }
      ENDCG
    }
    
    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      
      uniform sampler2D _MainTex;
      uniform sampler2D _BloomTex;

      fixed4 frag(v2f i) : SV_Target {
        fixed4 color = tex2D(_MainTex, i.uv);
        
        color.rgb = 1.0 - (1.0 - color.rgb) * (1.0 - tex2D(_BloomTex, i.uv).rgb);

        return color;
      }
      ENDCG
    }
  }
}