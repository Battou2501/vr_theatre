Shader "Unlit/LightReceivingShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistanceCoef ("Distance coef", Range (1,50)) = 1
        _DistancePow ("Distance pow", Range (1,2)) = 1.5
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.normal = normalize(mul (unity_ObjectToWorld, v.normal));
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = 0;
                
                for(int j=0;j<60;j++)
                {
                    float3 p1 = _SamplePoints[j];
                    float dt1_1 = saturate(dot(i.normal,normalize(p1-i.worldPos)));
                    float dt1_2 = saturate(dot(float3(0,0,1),normalize(p1-i.worldPos)));
                    float dist1 = distance(i.worldPos, p1);
                    float s1 = _DistanceCoef/pow(dist1,_DistancePow);
                    fixed4 c1 = tex2Dlod(_MainTex, float4(_SamplePointsUV[j].xy,0,10));
                    col += c1*s1*(dt1_1*dt1_2*0.95+0.05);
                }

                return col;
            }
            ENDCG
        }
    }
}
