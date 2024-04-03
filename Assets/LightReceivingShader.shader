Shader "Unlit/LightReceivingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenLightTex ("Screen Light Texture", 2D) = "white" {}
        _DistanceCoef ("Distance coef", Range (1,50)) = 1
        _DistancePow ("Distance pow", Range (1,2)) = 1.5
        _ScreenScale ("Screen Scale (1.0 - 160m wide)", Range (0.01,2)) = 0.2
        _Shininess("Shininess", Range (0,1)) = 0
        _ShininessBrightness("Shininess Brightness", Range (0,100)) = 40
        _LightStrength("Light strength", Range (0,1)) = 1
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
            #pragma target 3.0
            
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
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float spec : TEXCOORD4;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _ScreenLightTex;
            float4 _MainTex_ST;
            float4 _ScreenLightTex_ST;

            float _ScreenScale;
            float _ShininessBrightness;

            float int_pow(float i, int p)
            {
                float r = i;
                for(int j=1;j<p;j++)
                {
                    r*=i;
                }

                return r;
            }
            
            //float4 _SamplePoints[15];
            static float3 _SamplePoints[60] =
            {
                float3(-72, 33.333, 96),
                float3(-56, 33.333, 98.5),
                float3(-40, 33.333, 100.5),
                float3(-24, 33.333, 101.5),
                float3( -8, 33.333, 102),
                float3(  8, 33.333, 102),
                float3( 24, 33.333, 101.5),
                float3( 40, 33.333, 100.5),
                float3( 56, 33.333, 98.5),
                float3( 72, 33.333, 96),

                float3(-72, 20, 96),
                float3(-56, 20, 98.5),
                float3(-40, 20, 100.5),
                float3(-24, 20, 101.5),
                float3( -8, 20, 102),
                float3(  8, 20, 102),
                float3( 24, 20, 101.5),
                float3( 40, 20, 100.5),
                float3( 56, 20, 98.5),
                float3( 72, 20, 96),

                float3(-72, 6.666, 96),
                float3(-56, 6.666, 98.5),
                float3(-40, 6.666, 100.5),
                float3(-24, 6.666, 101.5),
                float3( -8, 6.666, 102),
                float3(  8, 6.666, 102),
                float3( 24, 6.666, 101.5),
                float3( 40, 6.666, 100.5),
                float3( 56, 6.666, 98.5),
                float3( 72, 6.666, 96),

                float3(-72,-6.666, 96),
                float3(-56,-6.666, 98.5),
                float3(-40,-6.666, 100.5),
                float3(-24,-6.666, 101.5),
                float3( -8,-6.666, 102),
                float3(  8,-6.666, 102),
                float3( 24,-6.666, 101.5),
                float3( 40,-6.666, 100.5),
                float3( 56,-6.666, 98.5),
                float3( 72,-6.666, 96),

                float3(-72,-20, 96),
                float3(-56,-20, 98.5),
                float3(-40,-20, 100.5),
                float3(-24,-20, 101.5),
                float3( -8,-20, 102),
                float3(  8,-20, 102),
                float3( 24,-20, 101.5),
                float3( 40,-20, 100.5),
                float3( 56,-20, 98.5),
                float3( 72,-20, 96),

                float3(-72,-33.333, 96),
                float3(-56,-33.333, 98.5),
                float3(-40,-33.333, 100.5),
                float3(-24,-33.333, 101.5),
                float3( -8,-33.333, 102),
                float3(  8,-33.333, 102),
                float3( 24,-33.333, 101.5),
                float3( 40,-33.333, 100.5),
                float3( 56,-33.333, 98.5),
                float3( 72,-33.333, 96)
                
            };
            
            static float2 _SamplePointsUV[60]=
            {
                float2(0.05,0.91666),
                float2(0.15,0.91666),
                float2(0.25,0.91666),
                float2(0.35,0.91666),
                float2(0.45,0.91666),
                float2(0.55,0.91666),
                float2(0.65,0.91666),
                float2(0.75,0.91666),
                float2(0.85,0.91666),
                float2(0.95,0.91666),

                float2(0.05,0.75),
                float2(0.15,0.75),
                float2(0.25,0.75),
                float2(0.35,0.75),
                float2(0.45,0.75),
                float2(0.55,0.75),
                float2(0.65,0.75),
                float2(0.75,0.75),
                float2(0.85,0.75),
                float2(0.95,0.75),

                float2(0.05,0.58333),
                float2(0.15,0.58333),
                float2(0.25,0.58333),
                float2(0.35,0.58333),
                float2(0.45,0.58333),
                float2(0.55,0.58333),
                float2(0.65,0.58333),
                float2(0.75,0.58333),
                float2(0.85,0.58333),
                float2(0.95,0.58333),

                float2(0.05,0.41666),
                float2(0.15,0.41666),
                float2(0.25,0.41666),
                float2(0.35,0.41666),
                float2(0.45,0.41666),
                float2(0.55,0.41666),
                float2(0.65,0.41666),
                float2(0.75,0.41666),
                float2(0.85,0.41666),
                float2(0.95,0.41666),

                float2(0.05,0.25),
                float2(0.15,0.25),
                float2(0.25,0.25),
                float2(0.35,0.25),
                float2(0.45,0.25),
                float2(0.55,0.25),
                float2(0.65,0.25),
                float2(0.75,0.25),
                float2(0.85,0.25),
                float2(0.95,0.25),

                float2(0.05,0.08333),
                float2(0.15,0.08333),
                float2(0.25,0.08333),
                float2(0.35,0.08333),
                float2(0.45,0.08333),
                float2(0.55,0.08333),
                float2(0.65,0.08333),
                float2(0.75,0.08333),
                float2(0.85,0.08333),
                float2(0.95,0.08333)
            };
            float _MaxLightedZ;
            float _Aspect;

            float _DistanceCoef;
            float _DistancePow;
            float _Shininess;

            float _LightStrength;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.normal = normalize(mul (unity_ObjectToWorld, v.normal));
                const float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
                o.spec = 0;
                for(int j=0;j<60;j++)
                {
                    const float3 p1 = _SamplePoints[j] * _ScreenScale;
                    const float3 point_dir = normalize(p1-o.worldPos);
                    fixed pp = pow(lerp(1,saturate(dot(reflect(-point_dir, o.normal),viewDir)),_Shininess),lerp(1,15,_Shininess));//*lerp(1,5,pow(_Shininess,1.3));
                    pp*=pp;
                    //pp*=lerp(1,60,pow(_Shininess,1.7));
                    o.spec += pp/60;
                }

                o.spec *= lerp(1,_ShininessBrightness,pow(_Shininess,1.7));
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                const float3 norm = normalize(i.normal);
                
                half4 screen_col_ambient = 0;
                half4 screen_col_dots = 0;
                //float3 dir = 0;

                half4 ambient = unity_AmbientSky * _LightStrength;
                half4 light = _LightColor0 * _LightStrength;
                
                const fixed4 tex = tex2D(_MainTex,i.uv*_MainTex_ST);
                //fixed light_dot = saturate(dot(norm,normalize(_WorldSpaceLightPos0.xyz - i.worldPos)));
                fixed light_dot = saturate(dot(norm,normalize(_WorldSpaceLightPos0)));
                light_dot *= light_dot*0.3+0.7;
                
                
                
                for(int j=0;j<60;j++)
                {
                    const float3 p1 = _SamplePoints[j] * _ScreenScale;
                    const float3 point_dir = normalize(p1-i.worldPos);
                    
                    const float dt1_1 = saturate(dot(norm, point_dir));
                    const float dt1_2 = saturate(dot(float3(0,0,1), point_dir));
                    const float dist1 = distance(i.worldPos, p1) / _ScreenScale;
                    const float s1 = _DistanceCoef / pow(dist1,_DistancePow);
                    const fixed4 c1 = tex2Dlod(_ScreenLightTex, float4(_SamplePointsUV[j].xy,0,10));
                    screen_col_ambient += c1 * s1 * 0.1;
                    screen_col_dots += c1 * s1 * dt1_2 * dt1_1 * 0.9;
                }

                const fixed4 screen_light = tex * (screen_col_ambient + screen_col_dots * i.spec);

                const fixed4 normal_light = tex*(light*light_dot+ambient);
                
                return lerp(screen_light*0.9, normal_light, _LightStrength)+screen_light*0.1;
            }
            ENDCG
        }
    }
}
