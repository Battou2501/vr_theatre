Shader "Unlit/CinemaRoomShader"
{
    Properties
    {
        _Color0  ("Floor", Color) = (1, 1, 1, 1)
        _Color1  ("Side", Color) = (1, 1, 1, 1)
        _Color2  ("Wall", Color) = (1, 1, 1, 1)
        _Color3  ("Ceiling", Color) = (1, 1, 1, 1)
        _Color4  ("Tris Wall Left", Color) = (1, 1, 1, 1)
        _Color5  ("Tris Wall Right", Color) = (1, 1, 1, 1)
        _Color6  ("Tris Wall Top", Color) = (1, 1, 1, 1)
        _Color7  ("Tris Top Left", Color) = (1, 1, 1, 1)
        _Color8  ("Tris Top Righ", Color) = (1, 1, 1, 1)
        _Color9  ("Tris Top Top", Color) = (1, 1, 1, 1)
        _Color10 ("Speakers", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _LineTex ("Line Texture", 2D) = "white" {}
        _LightTex1 ("Light Texture 1", 2D) = "white" {}
        _LightTex2 ("Light Texture 2", 2D) = "white" {}
        _SpecTex1 ("Specular Texture 1", 2D) = "white" {}
        _SpecTex2 ("Specular Texture 2", 2D) = "white" {}
        _AOTex1 ("AO Texture 1", 2D) = "white" {}
        _AOTex2 ("AO Texture 2", 2D) = "white" {}
        _ScreenLightTex ("Screen Light Texture", 2D) = "white" {}
        _ScreenLightMult ("Screen light multiplier", Range (0.1,5)) = 1
        _LightMaxStrength ("Light Max strength", Range (0.1,5)) = 1
        _AOStrength ("AO strength", Range (0,1)) = 1
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_line : TEXCOORD1;
                float2 uv_mapping : TEXCOORD2;
                float2 uv_light : TEXCOORD3;
                float3 normal : NORMAL;
                fixed4 color : COLOR0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 normal : TEXCOORD2;
                float2 uv_line : TEXCOORD3;
                float2 color_line : TEXCOORD4;
                float3 reflect_viewDir : TEXCOORD5;
                float2 uv_light : TEXCOORD6;
                fixed4 color : COLOR0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            sampler2D _LineTex;
            sampler2D _LightTex1;
            sampler2D _LightTex2;
            sampler2D _SpecTex1;
            sampler2D _SpecTex2;
            sampler2D _AOTex1;
            sampler2D _AOTex2;
            sampler2D _ScreenLightTex;
            
            float4 _MainTex_ST;
            float4 _LineTex_ST;
            float4 _ScreenLightTex_ST;

            
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
            
            float _LightStrength = 1;
            float _ScreenLightMult;
            float _LightMaxStrength;
            float _AOStrength; 
            
            fixed4 _Color0;
            fixed4 _Color1;
            fixed4 _Color2;
            fixed4 _Color3;
            fixed4 _Color4;
            fixed4 _Color5;
            fixed4 _Color6;
            fixed4 _Color7;
            fixed4 _Color8;
            fixed4 _Color9;
            fixed4 _Color10;

            
            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_line = TRANSFORM_TEX(v.uv_line, _LineTex);
                o.uv_light = v.uv_light;
                o.color_line = v.uv_mapping;
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.normal = normalize(mul (unity_ObjectToWorld, v.normal));
                const float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
                const float3 reflect_viewDir = -reflect(viewDir, o.normal);
                o.reflect_viewDir = reflect_viewDir;

                o.color = v.color;
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                const float3 norm = normalize(i.normal);
                //const float3 reflect_viewDir = normalize(i.reflect_viewDir);

                //const half4 ambient = unity_AmbientSky * _LightStrength;
                //const half4 light = _LightColor0 * _LightStrength;
                //const float3 light_dir = normalize(_WorldSpaceLightPos0);

                const fixed4 tex_line = tex2D(_LineTex,i.uv_line);

                const int light_tex_idx = floor(i.color.r * 6);
                const int light_tex_channel_idx = floor(i.color.g * 3);
                
                const fixed4 tex_light = tex2D(_LightTex1,i.uv_light) * (light_tex_idx < 3) + tex2D(_LightTex2,i.uv_light) * (light_tex_idx > 2);
                const fixed4 tex_ao = tex2D(_AOTex1,i.uv_light) * (light_tex_idx < 3) + tex2D(_AOTex2,i.uv_light) * (light_tex_idx > 2);
                const fixed4 tex_spec = tex2D(_SpecTex1,i.uv_light) * (light_tex_idx < 3) + tex2D(_SpecTex2,i.uv_light) * (light_tex_idx > 2);

                const fixed ao = (
                      tex_ao.r * (light_tex_channel_idx==0)
                    + tex_ao.g * (light_tex_channel_idx==1)
                    + tex_ao.b * (light_tex_channel_idx==2)
                    //+ tex_ao.a * (light_tex_channel_idx==3)
                    )*_AOStrength+(1-_AOStrength);
                
                const float light = (
                      tex_light.r * (light_tex_channel_idx==0)
                    + tex_light.g * (light_tex_channel_idx==1)
                    + tex_light.b * (light_tex_channel_idx==2)
                    //+ tex_light.a * (light_tex_channel_idx==3)
                    ) * 5 * _LightMaxStrength * ao;

                const fixed spec = (
                      tex_spec.r * (light_tex_channel_idx==0)
                    + tex_spec.g * (light_tex_channel_idx==1)
                    + tex_spec.b * (light_tex_channel_idx==2)
                    //+ tex_spec.a * (light_tex_channel_idx==3)
                    ) * _LightMaxStrength * ao;

                
                const int col_idx = floor(i.color_line.x);
                const float line_idx = saturate(floor(i.color_line.y)*tex_line.a);
                const fixed4 col =
                  _Color0 * (col_idx==0)
                + _Color1 * (col_idx==1)
                + _Color2 * (col_idx==2)
                + _Color3 * (col_idx==3)
                + _Color4 * (col_idx==4)
                + _Color5 * (col_idx==5)
                + _Color6 * (col_idx==6)
                + _Color7 * (col_idx==7)
                + _Color8 * (col_idx==8)
                + _Color9 * (col_idx==9)
                + _Color10 * (col_idx==10)
                ;
                
                
                const fixed4 tex = tex2D(_MainTex,i.uv*_MainTex_ST) * col * (1-line_idx) + tex_line * line_idx;

                
                
                //const fixed light_dot = saturate(dot(norm,light_dir));
                //const fixed light_reflect_dot = saturate(dot(reflect_viewDir,light_dir));
                

                //const fixed4 specular_light=pow(light_reflect_dot,lerp(1,50,_Shininess)) * lerp(0,5,_Shininess) * light * light_dot;


                //float specular_screen=0;
                half4 screen_col_ambient = 0;
                half4 screen_col_dots = 0;
                
                for(int j=0;j<60;j++)
                {
                    const float3 p1 = float3(_VecArrX[j],_VecArrY[j],_VecArrZ[j]);
                    const float3 point_vec = p1-i.worldPos;
                    const float3 point_dir = normalize(point_vec);
                    
                    const float PdotN =  saturate(dot(norm,            point_dir));
                    const float PdotSN = saturate(dot(float3(0,0,-1),   -point_dir));
                    //const float PdotRV = saturate(dot(reflect_viewDir, point_dir));
                    
                    const float dist1 = dot(point_vec,point_vec);
                    const float s1 = 1 / dist1;
                    const fixed4 c1 = tex2D(_ScreenLightTex, _SamplePointsUV[j].xy);

                    screen_col_ambient += c1 * min(s1 , 0.001);
                    screen_col_dots += c1 * min(s1 , 0.999) * PdotSN * PdotN;
                    //specular_screen += pow(PdotRV,lerp(1,80,_Shininess)) * _Shininess * _ShininessBrightness;
                }
                
                //const fixed4 specular = lerp(specular_screen * screen_col_dots, specular_light, _LightStrength);
                const fixed4 screen_light = tex*(screen_col_ambient + screen_col_dots)*_ScreenLightMult*ao;
                //const fixed4 normal_light = (tex * (tex_light*5) + tex_spec)*tex_ao ;
                const fixed4 normal_light = tex*light+spec*0.5;

                
                //return lerp(screen_light * 0.9, normal_light, _LightStrength) + screen_light * 0.1 + specular;
                return lerp(screen_light * 0.9, normal_light, _LightStrength) + screen_light * 0.1;
                //return tex*light+spec*0.5;//tex * light + spec;
            }
            ENDCG
        }
    }
}
