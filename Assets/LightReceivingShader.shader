Shader "Unlit/LightReceivingShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex_White ("Texture (White)", 2D) = "white" {}
        _MainTex_Black ("Texture (Black)", 2D) = "white" {}
        _ScreenLightTex ("Screen Light Texture", 2D) = "white" {}
        _ScreenLightMult ("Screen light multiplier", Range (0.1,5)) = 1
        _Shininess("Shininess", Range (0,1)) = 0
        _ShininessBrightness("Shininess Brightness", Range (0,10)) = 1
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
            #pragma multi_compile_instancing
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
                fixed4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                //float spec : TEXCOORD4;
                float3 reflect_viewDir : TEXCOORD5;
                fixed4 color : COLOR0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex_White;
            sampler2D _MainTex_Black;
            sampler2D _ScreenLightTex;
            float4 _MainTex_White_ST;
            float4 _MainTex_Black_ST;
            float4 _ScreenLightTex_ST;

            float _ShininessBrightness;

            
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

            float4 _VecArr[60];
            float _VecArrX[60];
            float _VecArrY[60];
            float _VecArrZ[60];
            
            float _Aspect;
            float _Shininess;
            float _LightStrength;
            float _ScreenLightMult;
            fixed4 _Color;

            
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex_White);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.normal = normalize(mul (unity_ObjectToWorld, v.normal));
                const float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
                const float3 reflect_viewDir = -reflect(viewDir, o.normal);
                o.reflect_viewDir = reflect_viewDir;
                //o.spec = 0;
                //for(int j=0;j<60;j++)
                //{
                //    const float3 p1 = float3(_VecArrX[j],_VecArrY[j],_VecArrZ[j]);// * _ScreenScale;
                //    const float3 point_dir = normalize(p1-o.worldPos);
                //    //fixed pp = pow(lerp(1,saturate(dot(reflect(-point_dir, o.normal),viewDir)),_Shininess),lerp(1,100,_Shininess));//*lerp(1,5,pow(_Shininess,1.3));
                //    fixed pp = pow(lerp(1,saturate(dot(point_dir,reflect_viewDir)),_Shininess),100) * (dot(point_dir, o.normal)>0);//*lerp(1,5,pow(_Shininess,1.3));
                //    //fixed pp = pow(saturate(dot(reflect(-point_dir, o.normal),viewDir)),lerp(1,100,_Shininess)) * (dot(point_dir, o.normal)>0);//*lerp(1,5,pow(_Shininess,1.3));
                //    //pp*=pp;
                //    //pp=pow(saturate(dot(reflect(-point_dir, o.normal),viewDir)),60);
                //    //pp*=lerp(1,60,pow(_Shininess,1.7));
                //    o.spec += pp;//*0.016666*lerp(0,10,_Shininess);
                //}
                //
                //o.spec = 1-exp(-o.spec);

                o.color = v.color;
                //o.spec = pow(o.spec,2.2);
                
                //o.spec *= lerp(1,_ShininessBrightness,pow(_Shininess,1.7));
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                const float3 norm = normalize(i.normal);
                const float3 reflect_viewDir = normalize(i.reflect_viewDir);

                const half4 ambient = unity_AmbientSky * _LightStrength;
                const half4 light = _LightColor0 * _LightStrength;
                const float3 light_dir = normalize(_WorldSpaceLightPos0);
                
                const fixed4 tex_white = tex2D(_MainTex_White,i.uv*_MainTex_White_ST) * _Color * i.color.x;
                const fixed4 tex_black = tex2D(_MainTex_Black,i.uv*_MainTex_Black_ST) * _Color * (1.0-i.color.x);
                const fixed4 tex = tex_black+tex_white;
                
                const fixed light_dot = saturate(dot(norm,light_dir));
                const fixed light_reflect_dot = saturate(dot(reflect_viewDir,light_dir));
                

                const fixed4 specular_light=pow(light_reflect_dot,lerp(1,50,_Shininess)) * lerp(0,5,_Shininess) * light * light_dot;


                float specular_screen=0;
                half4 screen_col_ambient = 0;
                half4 screen_col_dots = 0;
                
                for(int j=0;j<60;j++)
                {
                    const float3 p1 = float3(_VecArrX[j],_VecArrY[j],_VecArrZ[j]);
                    const float3 point_vec = p1-i.worldPos;
                    const float3 point_dir = normalize(point_vec);
                    
                    const float PdotN =  saturate(dot(norm,            point_dir));
                    const float PdotSN = saturate(dot(float3(0,0,1),   point_dir));
                    const float PdotRV = saturate(dot(reflect_viewDir, point_dir));
                    
                    const float dist1 = dot(point_vec,point_vec);
                    const float s1 = 1 / dist1;
                    //const fixed4 c1 = tex2Dlod(_ScreenLightTex, float4(_SamplePointsUV[j].xy,0,_MipLevel));
                    const fixed4 c1 = tex2D(_ScreenLightTex, _SamplePointsUV[j].xy);

                    screen_col_ambient += c1 * min(s1 , 0.001);
                    screen_col_dots += c1 * min(s1 , 0.999) * PdotSN * PdotN;
                    specular_screen += pow(PdotRV,lerp(1,80,_Shininess)) * _Shininess * _ShininessBrightness;
                }
                
                const fixed4 specular = lerp(specular_screen * screen_col_dots, specular_light, _LightStrength);
                const fixed4 screen_light = tex*(screen_col_ambient + screen_col_dots)*_ScreenLightMult;
                const fixed4 normal_light = tex*(light*light_dot+ambient);

                return lerp(screen_light * 0.9, normal_light, _LightStrength) + screen_light * 0.1 + specular;
            }
            ENDCG
        }
    }
}
