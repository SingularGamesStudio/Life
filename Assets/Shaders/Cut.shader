Shader "Custom/Cut"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader{
        Tags { "RenderType" = "Opaque" }

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            sampler2D _MainTex;
            UNITY_INSTANCING_BUFFER_START(Inst)
            UNITY_DEFINE_INSTANCED_PROP(float4, _Rect)
            UNITY_INSTANCING_BUFFER_END(Inst)
            v2f vert(appdata v, uint instanceID: SV_InstanceID)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 myRect = UNITY_ACCESS_INSTANCED_PROP(Inst, _Rect);
                o.uv = (v.uv*myRect.z + myRect.xy) / 16.0;
                //o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
