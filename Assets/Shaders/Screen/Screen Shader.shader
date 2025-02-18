Shader "Unlit/Screen Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LightTex ("Texture Light", 2D) = "white" {}
        _SpecTex ("Texture Spec", 2D) = "black" {}
        _MovieTex ("Movie texture", 2D) = "black" {}
        _MovieMipTex ("Screen ambient texture", 2D) = "black" {}
        _GrainStrength ("Grain strength", Range(0,1)) = 0.5
        _GrainSize ("Grain size", Range(1,2)) = 1
        _Grain1 ("Grain 1", 2D) = "white" {}
        _Grain2 ("Grain 2", 2D) = "white" {}
        _Grain3 ("Grain 3", 2D) = "white" {}
        _Grain4 ("Grain 4", 2D) = "white" {}
        _Grain5 ("Grain 5", 2D) = "white" {}
        _Grain6 ("Grain 6", 2D) = "white" {}
        _Grain7 ("Grain 7", 2D) = "white" {}
        _Grain8 ("Grain 8", 2D) = "white" {}
        //_LightStrength("Light strength", Range (0,1)) = 1
        _LightMaxStrength ("Light Max strength", Range (0.1,2)) = 1
        _ScreenAmbientStrength("Screen ambient strength", Range(0,0.5)) = 0.08
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
                float2 uv_light : TEXCOORD1;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv_movie : TEXCOORD1;
                float2 uv_light : TEXCOORD3;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _LightTex;
            sampler2D _SpecTex;
            sampler2D _MovieTex;
            float4 _MainTex_ST;
            float4 _MovieTex_ST;
            sampler2D _MovieMipTex;

            sampler2D _Grain1;
            sampler2D _Grain2;
            sampler2D _Grain3;
            sampler2D _Grain4;
            sampler2D _Grain5;
            sampler2D _Grain6;
            sampler2D _Grain7;
            sampler2D _Grain8;

            float _GrainStrength;
            float _GrainSize;
            
            float _Aspect;
            float _OneOverAspect;

            float _LightStrength;

            int _AffectedByLight;

            float _LightMaxStrength;

            float _ScreenAmbientStrength;

            int _VideoType; // 0 - Flat, 1 - hSbS, 2 - SBS, 3 - hOU, 4 - OU
            float _xBorderCoef;
            float _yBorderCoef;
            int _InvertOrder;

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_light = v.uv;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv_movie = TRANSFORM_TEX(v.uv, _MovieTex);

                int eye_index = abs(_InvertOrder - unity_StereoEyeIndex);
                
                float2 aspect_greater = float2(o.uv_movie.x, (o.uv_movie.y - 0.5) * _Aspect + 0.5);
                float2 aspect_lower =   float2((o.uv_movie.x - 0.5) * 1.9 * _OneOverAspect + 0.5, (o.uv_movie.y - 0.5) * 1.9 + 0.5);

                //float2 aspect_greater = float2(o.uv_movie.x, (o.uv_movie.y - 0.5) * 2 + 0.5);
                //float2 aspect_lower =   float2((o.uv_movie.x - 0.5) * 1.9 * 0.5 + 0.5, (o.uv_movie.y - 0.5) * 1.9 + 0.5);

                int is_greater =  _Aspect >= 1.9;
                
                o.uv_movie = aspect_greater * is_greater + aspect_lower * (1-is_greater);

                o.uv_movie.x = lerp(o.uv_movie.x, o.uv_movie.x * 0.5 + 0.5 * eye_index, _VideoType == 1);
                o.uv_movie.y = lerp(o.uv_movie.y, o.uv_movie.y * 0.5 + 0.5 * eye_index, _VideoType == 3);
                
                //o.uv_movie.x = (o.uv_movie.x * 0.5);
                
                o.normal = normalize(mul (unity_ObjectToWorld, v.normal));
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                int eye_index = abs(_InvertOrder - unity_StereoEyeIndex);
                
                const fixed4 tex_screen = tex2D(_MainTex, i.uv);
                const fixed4 col = (tex_screen * tex2D(_LightTex, i.uv_light) * _LightMaxStrength + tex2D(_SpecTex, i.uv_light)*0.5);
                
                fixed4 tex_movie = tex2Dlod(_MovieTex, float4(i.uv_movie.xy,0,0));
                fixed4 tex_ambient = tex2Dlod(_MovieMipTex, float4(i.uv_light,0,5)) * tex_screen;

                const fixed l = saturate(sqrt(0.299 * tex_movie.r*tex_movie.r + 0.587 * tex_movie.g*tex_movie.g + 0.114 * tex_movie.b*tex_movie.b));
                //const fixed l_ambient = saturate(sqrt(0.299 * tex_ambient.r*tex_ambient.r + 0.587 * tex_ambient.g*tex_ambient.g + 0.114 * tex_ambient.b*tex_ambient.b));
                
                const fixed4 col_grain1 = tex2D(_Grain1, i.uv_movie*_GrainSize)*4;
                const fixed4 col_grain2 = tex2D(_Grain2, i.uv_movie*_GrainSize)*4;
                const fixed4 col_grain3 = tex2D(_Grain3, i.uv_movie*_GrainSize)*4;
                const fixed4 col_grain4 = tex2D(_Grain4, i.uv_movie*_GrainSize)*4;
                const fixed4 col_grain5 = tex2D(_Grain5, i.uv_movie*_GrainSize)*4;
                const fixed4 col_grain6 = tex2D(_Grain6, i.uv_movie*_GrainSize)*4;
                const fixed4 col_grain7 = tex2D(_Grain7, i.uv_movie*_GrainSize)*4;
                const fixed4 col_grain8 = tex2D(_Grain8, i.uv_movie*_GrainSize)*4;
                
                const float t = floor(frac(_Time.w) * 8);
                
                const fixed4 grain = lerp((
                    col_grain1 * (t==0)
                    + col_grain2 * (t==1)
                    + col_grain3 * (t==2)
                    + col_grain4 * (t==3)
                    + col_grain5 * (t==4)
                    + col_grain6 * (t==5)
                    + col_grain7 * (t==6)
                    + col_grain8 * (t==7)
                    ), 1, min(1,l+0.1));
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                const float f1 = saturate((i.uv_movie.y-lerp(_yBorderCoef, 1, eye_index))*300)>0;
                const float f2 = saturate((1-i.uv_movie.y-1 + lerp(0,1-_yBorderCoef,eye_index))*300)>0;
                const float f3 = saturate((i.uv_movie.x-lerp(_xBorderCoef, 1, eye_index))*300)>0;
                const float f4 = saturate((1-i.uv_movie.x-1+ lerp(0,1-_xBorderCoef,eye_index))*300)>0;
                const float border_factor = saturate(f1+f2+f3+f4);
                
                //const fixed4 col_movie = tex_movie * lerp(grain,1,1-_GrainStrength) * (1-border_factor) + tex_ambient * _ScreenAmbientStrength * pow(1-pow(l_ambient,0.4),0.6) * (1-saturate( l * (1-border_factor)*5)) ;
                const fixed4 col_movie = tex_movie * lerp(grain,1,1-_GrainStrength) * (1-border_factor) + pow(tex_ambient,0.4545) * 0.2 * _ScreenAmbientStrength * (1-saturate( l * (1-border_factor)*5));
                const fixed light_coef = pow(_LightStrength,1.5) * _AffectedByLight;

                //return border_factor;
                //return unity_StereoEyeIndex;
                //return lerp(grain,1,1-_GrainStrength);
                //return i.uv_movie.y;
                //fixed4 mcol = float4(tex_movie.rgb *(1-border_factor),1);
                //return mcol;
                //return f4;
                return  lerp(col_movie * lerp(1,tex_screen,0.6),col,light_coef) * 0.95 + col_movie * 0.05;
            }
            ENDCG
        }
    }
}
