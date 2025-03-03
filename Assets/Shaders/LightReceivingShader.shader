Shader "Unlit/LightReceivingShader"
{
    Properties
    {
        _ColorB ("Base Color", Color) = (1, 1, 1, 1)
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex_White ("Texture (White)", 2D) = "white" {}
        _MainTex_Black ("Texture (Black)", 2D) = "white" {}
        _LightTex ("Texture Light", 2D) = "white" {}
        _LightMaxStrength ("Light Max strength", Range (0.0001,5)) = 1
        _AOTex ("Texture AO", 2D) = "white" {}
        _AOStrength ("AO strength", Range (0,1)) = 1
        _LightUV ("Light UV Index", Int) = 0
        _ScreenLightTex ("Screen Light Texture", 2D) = "white" {}
        _ScreenLightMult ("Screen light multiplier", Range (0.0001,5)) = 1
        [KeywordEnum(Mate,Glossy,Metalic)] _Type("Material Type",int) = 0
        _Shininess("Shininess", Range (0,1)) = 0
        _ShininessBrightness("Shininess Brightness", Range (0,10)) = 1
        [Toggle]_ToggleLight("Toggle Light", Range (0,1)) = 1
        _MipLevel("Mip level", Range (0,10)) = 1
        [KeywordEnum(Scene,Above)] _Light_From("Dir light source",int) = 0
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

            #pragma shader_feature _TYPE_MATE _TYPE_GLOSSY _TYPE_METALIC
            #pragma shader_feature _LIGHT_FROM_SCENE _LIGHT_FROM_ABOVE
            
            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float2 uv3 : TEXCOORD2;
                float2 uv4 : TEXCOORD3;
                float3 normal : NORMAL;
                fixed4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float2 uv_light : TEXCOORD4;
                float3 reflect_viewDir : TEXCOORD5;
                float3 viewDir : TEXCOORD6;
                float3 lightDir : TEXCOORD7;
                fixed3 color_mod : TEXCOORD8;
                fixed4 color : COLOR0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct instance_data
            {
                float4x4 trs;
                float col_mod;
                float hue_mod;
                float alpha_mod;
            };

            sampler2D _MainTex_White;
            sampler2D _MainTex_Black;
            sampler2D _LightTex;
            sampler2D _AOTex;
            sampler2D _ScreenLightTex;
            float4 _MainTex_White_ST;
            float4 _MainTex_Black_ST;
            float4 _ScreenLightTex_ST;

            float _ShininessBrightness;
            float _AOStrength;
            float _LightMaxStrength;
            int _LightUV;

            StructuredBuffer<instance_data> _TRS_Array;
            int _Instanced;
            
            static float2 _SamplePointsUV[60]=
            {
                float2(0.05,0.7192),
                float2(0.15,0.7192),
                float2(0.25,0.7192),
                float2(0.35,0.7192),
                float2(0.45,0.7192),
                float2(0.55,0.7192),
                float2(0.65,0.7192),
                float2(0.75,0.7192),
                float2(0.85,0.7192),
                float2(0.95,0.7192),

                float2(0.05,0.6315),
                float2(0.15,0.6315),
                float2(0.25,0.6315),
                float2(0.35,0.6315),
                float2(0.45,0.6315),
                float2(0.55,0.6315),
                float2(0.65,0.6315),
                float2(0.75,0.6315),
                float2(0.85,0.6315),
                float2(0.95,0.6315),

                float2(0.05,0.5438),
                float2(0.15,0.5438),
                float2(0.25,0.5438),
                float2(0.35,0.5438),
                float2(0.45,0.5438),
                float2(0.55,0.5438),
                float2(0.65,0.5438),
                float2(0.75,0.5438),
                float2(0.85,0.5438),
                float2(0.95,0.5438),

                float2(0.05,0.4561),
                float2(0.15,0.4561),
                float2(0.25,0.4561),
                float2(0.35,0.4561),
                float2(0.45,0.4561),
                float2(0.55,0.4561),
                float2(0.65,0.4561),
                float2(0.75,0.4561),
                float2(0.85,0.4561),
                float2(0.95,0.4561),

                float2(0.05,0.3684),
                float2(0.15,0.3684),
                float2(0.25,0.3684),
                float2(0.35,0.3684),
                float2(0.45,0.3684),
                float2(0.55,0.3684),
                float2(0.65,0.3684),
                float2(0.75,0.3684),
                float2(0.85,0.3684),
                float2(0.95,0.3684),

                float2(0.05,0.2807),
                float2(0.15,0.2807),
                float2(0.25,0.2807),
                float2(0.35,0.2807),
                float2(0.45,0.2807),
                float2(0.55,0.2807),
                float2(0.65,0.2807),
                float2(0.75,0.2807),
                float2(0.85,0.2807),
                float2(0.95,0.2807)
            };

            float4 _VecArr[60];
            float _VecArrX[60];
            float _VecArrY[60];
            float _VecArrZ[60];
            
            float _Shininess;
            float _LightStrength;
            float _ScreenLightMult;
            fixed4 _Color;
            fixed4 _ColorB;

            float _ToggleLight;
            int _MipLevel;

            
            v2f vert (appdata v, uint svInstanceID : SV_InstanceID)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.color_mod.x = lerp(1,_TRS_Array[svInstanceID].col_mod, _Instanced);
                o.color_mod.y = lerp(1,_TRS_Array[svInstanceID].hue_mod, _Instanced);
                o.color_mod.z = lerp(1,_TRS_Array[svInstanceID].alpha_mod, _Instanced);
                //o.color_mod = svInstanceID==10;
                float4 wpos = mul(_TRS_Array[svInstanceID].trs, v.vertex + float4(0,0.5,1,0)*_TRS_Array[svInstanceID].alpha_mod*0.05)  * _Instanced + (1-_Instanced) * mul (unity_ObjectToWorld, v.vertex);
                //o.color_mod = _TRS_Array[svInstanceID][0][2] > 0;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = mul(UNITY_MATRIX_VP, wpos) * _Instanced + (1-_Instanced) * UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex_White);
                //o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.worldPos = wpos;
                o.normal = normalize(mul (_TRS_Array[svInstanceID].trs * _Instanced + (1-_Instanced) * unity_ObjectToWorld, v.normal));
                const float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);

                #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                const float3 reflect_viewDir = 2*dot(o.normal,viewDir)*o.normal-viewDir;
                o.reflect_viewDir = reflect_viewDir;
                #else
                o.reflect_viewDir = 0;
                #endif
                
                o.viewDir = viewDir;

                //#if defined(_LIGHT_FROM_SCENE)
                //o.lightDir = normalize(_WorldSpaceLightPos0);
                //#else
                //o.lightDir = float3(0,1,0);
                //#endif
                
                o.color = v.color;
                o.uv_light = v.uv * (_LightUV == 0) + v.uv2 * (_LightUV == 1) + v.uv3 * (_LightUV == 2) + v.uv4 * (_LightUV == 3);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                const float3 norm = normalize(i.normal);

                const float4 light_tex = tex2D(_LightTex, i.uv_light)*2;
                const float4 ao_tex = tex2D(_AOTex, i.uv_light)*2;
                const float ao = lerp(1,ao_tex.r * i.color.x + ao_tex.g * (1- i.color.x),_AOStrength);
                const float light = (light_tex.r * i.color.x + light_tex.g * (1- i.color.x)) * _LightMaxStrength;
                
                
                const float4 tex_white = tex2D(_MainTex_White,i.uv*_MainTex_White_ST) * i.color.x * fixed4(i.color_mod.xxx,1) * fixed4(i.color_mod.y,i.color_mod.y,i.color_mod.z,1);
                const float4 tex_black = tex2D(_MainTex_Black,i.uv*_MainTex_Black_ST) * (1.0-i.color.x) * fixed4(i.color_mod.xxx,1) * fixed4(i.color_mod.y,1,0.8 + (1-i.color_mod.y),1);
                const float tex_white_col_coef = lerp(pow(tex_white.a,0.4), 1 , i.color_mod.z*0.6+0.2);
                const float tex_black_col_coef = lerp(pow(tex_black.a,0.4), 1 , i.color_mod.z*0.6+0.2);
                
                #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                const float3 reflect_viewDir = normalize(i.reflect_viewDir);
                const float3 viewDir = normalize(i.viewDir);
                const float r_factor = 1-((1-_Shininess)*(1-_Shininess));
                const float4 reflections = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflect_viewDir, lerp(6,0,r_factor)) * lerp(0.3,_LightStrength,0.5);
                #endif

                #ifdef _TYPE_GLOSSY
                const float view_norm_dot = 1-saturate(dot(viewDir,norm));
                const float4 tex = lerp(tex_black * lerp(1,_Color, tex_black_col_coef) + tex_white * lerp(1,_Color, tex_white_col_coef), reflections, r_factor * view_norm_dot);
                #elif _TYPE_METALIC
                const float4 tex = lerp(reflections*0.6+0.4,reflections,_Shininess) * (tex_black * lerp(1,_Color, tex_black_col_coef) + tex_white * lerp(1,_Color, tex_white_col_coef));
                #else 
                const float4 tex = tex_black * lerp(1,_Color* _ColorB, tex_black_col_coef) + tex_white * lerp(1,_Color* _ColorB, tex_white_col_coef);
                #endif
                
                
                #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                //const float3 light_dir = normalize(i.lightDir);
                //const fixed light_dot = saturate(dot(norm,light_dir)*3);
                //const float light_reflect_dot = saturate(dot(light_dir, reflect_viewDir));

                //const float specular_light=pow(light_reflect_dot,lerp(1,80,_Shininess*_Shininess*_Shininess)) * _Shininess * _ShininessBrightness * 2 * light_dot *0;
                #endif
                


                #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                float4 specular_screen=0;
                float spec_pow = lerp(1,100,_Shininess);
                #endif
                
                float4 screen_col_ambient = 0;
                float4 screen_col_dots = 0;

                #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                float spec_multiplier = _Shininess * _ShininessBrightness * 0.15;
                #endif
                
                for(int j=0;j<60;j++)
                {
                    const float3 p1 = float3(_VecArrX[j],_VecArrY[j],_VecArrZ[j]);
                    const float3 point_vec = p1-i.worldPos;
                    const float3 point_dir = normalize(point_vec);
                    
                    const float PdotN =  saturate(dot(norm,            point_dir));
                    const float PdotSN = saturate(dot(float3(0,0,-1),   -point_dir));

                    #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                    const float PdotRV = saturate(dot(point_dir, reflect_viewDir));
                    #endif
                    
                    const float dist1 = dot(point_vec,point_vec);
                    const float s1 = 1 / dist1;
                    const float4 c1 = tex2Dlod(_ScreenLightTex, float4(_SamplePointsUV[j].xy,0,_MipLevel));

                    screen_col_ambient += c1 * min(s1 , 0.001);
                    screen_col_dots += c1 * min(s1 , 0.999) * PdotSN * PdotN;

                    #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                    specular_screen += pow(PdotRV,spec_pow) * spec_multiplier * c1 * saturate(PdotSN*3) * saturate(PdotN*3);
                    #endif
                    
                }

                #if defined(_TYPE_GLOSSY) || defined(_TYPE_METALIC)
                //const fixed4 specular = lerp(specular_screen, specular_light, _LightStrength);
                const float4 specular = lerp(specular_screen, 0, _LightStrength);
                #endif

                const float4 screen_light = tex * (screen_col_ambient + screen_col_dots) * _ScreenLightMult;
                const float4 normal_light = tex * light;

                #ifdef _TYPE_GLOSSY
                return (lerp(screen_light * 0.9, normal_light, _LightStrength) + screen_light * 0.1) * (1-saturate(specular)) * ao + specular;
                #elif _TYPE_METALIC
                return ((lerp(screen_light * 0.9, normal_light, _LightStrength) + screen_light * 0.1) * (1-saturate(specular)) * ao + specular ) * _Color;
                #else


                //return tex_white;
                //return fixed4(i.color_mod.y,1,0.5 + (1-i.color_mod.y),1);
                //return lerp(screen_light * 0.9, normal_light, _LightStrength) + screen_light * 0.1;
                return (lerp(screen_light * 0.9, normal_light, _LightStrength) + screen_light * 0.1) * ao * _ToggleLight + tex*(1-_ToggleLight)*ao;
                #endif
                
            }
            ENDCG
        }
    }
}
