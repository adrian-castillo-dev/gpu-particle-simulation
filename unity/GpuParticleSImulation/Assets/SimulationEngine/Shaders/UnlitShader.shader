Shader "Custom/UnlitShader"
{
    SubShader
    {
        Pass
        {
            
            HLSLPROGRAM

            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5

            #include "UnityCG.cginc"

            struct Particle{
                float3 position;
                float3 velocity;
                float mass;
                int type;
                float3 direction;
            };

            StructuredBuffer<Particle> particleBuffer;
            float _Radius;
            int nTypes;

            float3 HSVtoRGB(float3 hsv)
            {
                float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
                float3 p = abs(frac(hsv.xxx + K.xyz) * 6.0 - K.www);
                return hsv.z * lerp(K.xxx, saturate(p - K.xxx), hsv.y);
            }


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

                float hue = (float)particle.type / (float)nTypes;
                float3 rgb = HSVtoRGB(float3(hue, 1.0, 1.0));

                o.pos = mul(UNITY_MATRIX_VP, float4(worldPos, 1.0f));
                o.color = float4(rgb, 1.0);

                return o;
            }

            float4 frag(v2f i) : SV_Target{
                return i.color;
            }
            
            ENDHLSL
        }
    }
}
