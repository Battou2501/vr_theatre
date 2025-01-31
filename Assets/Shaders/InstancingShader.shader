Shader "Unlit/InstancingShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
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
            
            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            StructuredBuffer<float4x4> _TRS_Array;

            fixed4 _Color;
            float4 _Col;
            int _Instanced;

            v2f vert(appdata_base v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                //uint cmdID = GetCommandID(0);
                //uint instanceID = GetIndirectInstanceID(svInstanceID);
                
                float4 wpos = mul(_TRS_Array[svInstanceID], v.vertex);
                //float4 wpos = v.vertex;
                //if (_Instanced == 1)
                o.pos = mul(UNITY_MATRIX_VP, wpos) * _Instanced + (1-_Instanced) * UnityObjectToClipPos(v.vertex);
                //else
                //o.pos = UnityObjectToClipPos(v.vertex);
                
                //o.color = float4(0.5f,0.5f,0.5f,1.0f);// float4(cmdID & 1 ? 0.0f : 1.0f, cmdID & 1 ? 1.0f : 0.0f, instanceID / float(GetIndirectInstanceCount()), 0.0f);
                //return o;
                //v2f o;
                //o.pos = UnityObjectToClipPos(v.vertex);
                //o.pos = UnityObjectToClipPos(wpos);
                //uint x = unity_InstanceID;
                o.color = _Col * _Instanced + (1-_Instanced) * _Color;//svInstanceID==0;// float4(0.5f,0.5f,0.5f,1.0f);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
