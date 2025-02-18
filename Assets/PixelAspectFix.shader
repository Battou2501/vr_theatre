Shader "Hidden/PixelAspectFix"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _PixelAspectRatio;
            int _AspectType;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                int a = _PixelAspectRatio<0;
                
                o.uv.x = lerp(o.uv.x, (o.uv.x-0.5)*_PixelAspectRatio+0.5, _AspectType<0);
                //o.uv.x = (o.uv.x-0.5)/_PixelAspectRatio+0.5;
                o.uv.y = lerp(o.uv.y, (o.uv.y-0.5)*_PixelAspectRatio+0.5, _AspectType>0);
                
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return fixed4(col.rgb,1);
            }
            ENDCG
        }
    }
}
