Shader "Custom/FishEyeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion Strength", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            ZTest Always
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _SonarTex;
            float _Distortion;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                float2 uv = i.uv;

                return tex2D(_MainTex, uv) + tex2D(_SonarTex, uv) * 0.99 - float4(0.1, 0.1, 0.1, 0.0);
            }
            ENDCG
        }
    }
}