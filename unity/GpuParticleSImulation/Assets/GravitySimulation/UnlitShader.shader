Shader "Custom/UnlitShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct Particle{
                float3 position;
                float3 velocity;
            };

            StructuredBuffer<Particle> particleBuffer;
            float _Radius;
            float4 _Color;
            
            struct appData{
                float4 vertex : POSITION;
                uint instanceID : SV_InstanceID;
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appData v){
                v2f o;
                
                Particle particle = particleBuffer[v.instanceID];
                float3 worldPos = v.vertex.xyz * _Radius + particle.position;
                
                o.pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0f));
                o.color = _Color;

                return o;
                
            }
            float4 frag (v2f i) : SV_Target{
                return i.color;
            }
            
            ENDHLSL
        }
    }
}
