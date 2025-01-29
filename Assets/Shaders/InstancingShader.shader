Shader "Unlit/InstancingShader"
{
        SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 color : COLOR0;
            };

            StructuredBuffer<float4x4> _TRS_Array;

            float4 _Col;

            v2f vert(appdata_base v, uint svInstanceID : SV_InstanceID)
            {
                InitIndirectDrawArgs(0);
                v2f o;
                //uint cmdID = GetCommandID(0);
                //uint instanceID = GetIndirectInstanceID(svInstanceID);
                float4 wpos = mul(_TRS_Array[svInstanceID], v.vertex);
                o.pos = mul(UNITY_MATRIX_VP, wpos);
                o.color = _Col;//float4(0.5f,0.5f,0.5f,1.0f);// float4(cmdID & 1 ? 0.0f : 1.0f, cmdID & 1 ? 1.0f : 0.0f, instanceID / float(GetIndirectInstanceCount()), 0.0f);
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
