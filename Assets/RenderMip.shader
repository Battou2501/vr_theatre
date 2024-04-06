Shader "Unlit/RenderMip"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MipLevel("Mip level", Range (0,10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _MipLevel;
            float _Aspect;

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                const float2 aspect_greater = float2(o.uv.x, (o.uv.y - 0.5) * _Aspect + 0.5);
                const float2 aspect_lower = float2((o.uv.x - 0.5) * _Aspect + 0.5, o.uv.y);

                const int is_greater = _Aspect > 1;
                
                o.uv = aspect_greater * is_greater + aspect_lower * (1-is_greater);

                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float f = saturate((i.uv.y-1)*300);
                float f2 = saturate((1-i.uv.y-1)*300);

                float f3 = saturate((i.uv.x-1)*300);
                float f4 = saturate((1-i.uv.x-1)*300);
                
                // sample the texture
                fixed4 col = tex2Dlod(_MainTex, float4(i.uv.xy,0,_MipLevel));
                col *= 1-saturate(f+f2+f3+f4);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
}
