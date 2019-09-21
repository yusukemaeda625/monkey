Shader "CustomShader/MotionBlur" {
  Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _Alpha ("Alpha", Range (0, 1)) = 0.5
  }
  SubShader {
    Cull Off ZWrite Off ZTest Always

    Pass{
      Blend SrcAlpha OneMinusSrcAlpha

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      
      uniform sampler2D _MainTex;
      uniform half      _Alpha;
      
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
                        
      fixed4 frag(v2f i) : SV_Target {
        return fixed4(tex2D(_MainTex, i.uv).rgb, _Alpha);
      }
      ENDCG
    }
  }
}