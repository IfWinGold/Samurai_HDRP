Shader "ColorBlit_RadialBlur"
{
    Properties
    {        
        _MainTex("MainTex",2D) = "white"{}
        _BlurPower("BlurPower",Range(0,1)) = 0.01
        _SampleingCount("SampleingCount",Range(1,64)) = 6      
        _Direction("Direction",Vector) = (0.5,0.5,0)
    }
    SubShader
    {
        Tags { "RenderType" = "Overlay" "RenderPipeline" = "UniversalPipeline" "Queue" = "Overlay"}
        ZTest Always ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
      
            struct Attributes
            {
                float4 positionHCS   : POSITION;
                float2 uv           : TEXCOORD0;                                
            };

            struct Varyings
            {
                float4  positionCS  : SV_POSITION;
                float2  uv          : TEXCOORD0;
                float2  noiseUV : TEXCOORD1;                
            };

            Varyings vert(Attributes input)
            {
                Varyings output;                
                output.positionCS = TransformObjectToHClip(input.positionHCS.xyz);
                output.uv = input.uv;                 
                return output;
            }

                                                  
            uniform Texture2D _MainTex;
            SAMPLER(sampler_MainTex);
            float2 _MainTex_TexelSize;
            float2 _Direction;
            float _SampleingCount;
            float _BlurPower;


            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);                                            
            float2 uvOffset;                        

            uvOffset = input.uv;
            //float2 direction = 0.5 - uvOffset;
            float2 direction = _Direction - uvOffset; 


            half4 color = (0, 0, 0,0);
            float f = 1.0 / float(_SampleingCount);

            for (int i = 0; i < _SampleingCount; ++i)
            {
                half3 col = _MainTex.Sample(sampler_MainTex, uvOffset - _BlurPower * direction * (float)i).rgb * f;                
                color.rgb += col;
            }                            
            return color;
            }
            ENDHLSL
        }
    }
}