Shader "Unlit/NewUnlitShader 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MovieTex ("Movie texture", 2D) = "white" {}
        //_LightStrength("Light strength", Range (0,1)) = 1
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
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv_movie : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MovieTex;
            float4 _MainTex_ST;
            float4 _MovieTex_ST;

            float _Aspect;

            float _LightStrength;

            int _AffectedByLight;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_movie = TRANSFORM_TEX(v.uv, _MovieTex);

                float2 aspect_greater = float2(o.uv_movie.x, (o.uv_movie.y - 0.5) * _Aspect + 0.5);
                float2 aspect_lower = float2((o.uv_movie.x - 0.5) * _Aspect + 0.5, o.uv_movie.y);

                int is_greater = _Aspect > 1;
                
                o.uv_movie = aspect_greater * is_greater + aspect_lower * (1-is_greater);

                o.normal = normalize(mul (unity_ObjectToWorld, v.normal));
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);


                half4 ambient = unity_AmbientSky * _LightStrength;
                half4 light = _LightColor0 * _LightStrength;
                fixed light_dot = saturate(dot(i.normal,normalize(_WorldSpaceLightPos0)));
                light_dot *= light_dot*0.3+0.7;
                
                col *=light*light_dot+ambient;
                
                fixed4 col_movie = tex2D(_MovieTex, i.uv_movie);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                //int border = i.uv.y>1;
                //border += i.uv.y<0;
                //border += i.uv.x<0;
                //border += i.uv.x>1;

                float f = saturate((i.uv_movie.y-1)*300);
                float f2 = saturate((1-i.uv_movie.y-1)*300);

                float f3 = saturate((i.uv_movie.x-1)*300);
                float f4 = saturate((1-i.uv_movie.x-1)*300);

                col_movie *= 1-saturate(f+f2+f3+f4);

                fixed light_coef = saturate((_LightStrength-0.0)*1) * _AffectedByLight;
                
                //return lerp(col_movie * 0.5, col, light_coef*light_coef) + col_movie * 0.5;//*(1-border);

                return col* light_coef + col_movie * 0.5 * (1.0 - (light_coef * light_coef)) + col_movie * 0.5;
            }
            ENDCG
        }
    }
}
