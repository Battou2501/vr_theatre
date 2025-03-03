Shader "Unlit/StretchablePanel"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
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
                //float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                //float4 color : COLOR;
            };

            float4 _Color;
            float _PanelWidth;
            float _PanelHeight;
            
            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.x += -_PanelWidth * (v.color.x*2-1) * 0.1;
                v.vertex.y += _PanelHeight * (v.color.y*2-1) * 0.1;
                //o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return _Color;
            }
            ENDCG
        }

        Pass
		{
			Name "SceneSelectionPass"
			Tags{ "LightMode" = "SceneSelectionPass" }
			ZWrite On
			ColorMask 0
			
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                //float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                //float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                //float4 color : COLOR;
            };
            
            float _PanelWidth;
            float _PanelHeight;

            int _ObjectId;
			int _PassValue;
            
            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.x += -_PanelWidth * (v.color.x*2-1) * 0.1;
                v.vertex.y += _PanelHeight * (v.color.y*2-1) * 0.1;
                //o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                //fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return float4( _ObjectId, _PassValue, 1.0, 1.0 );
            }
            ENDCG
			
        }
    }
}
