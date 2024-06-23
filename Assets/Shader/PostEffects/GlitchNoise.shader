Shader "CustomPostEffects/GlitchNoise"
{
    Properties
    {
        _MainTex ("Base", 2D) = ""{}
        _GlitchTex ("Glitch", 2D) = ""{}
        _BufferTex ("Buffer", 2D) = ""{}
        _Intensity ("Intensity", Float) = 1
    }

    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

    sampler2D _MainTex;
    sampler2D _GlitchTex;
    sampler2D _BufferTex;
    float _Intensity;

    struct appdata_img
    {
        float4 vertex : POSITION;
        half2 texcoord : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct v2f_img
    {
        float4 pos : SV_POSITION;
        half2 uv : TEXCOORD0;
        UNITY_VERTEX_INPUT_INSTANCE_ID
        UNITY_VERTEX_OUTPUT_STEREO
    };

    v2f_img vert_img(appdata_img v)
    {
        v2f_img o;
        ZERO_INITIALIZE(v2f_img, o);
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.pos = TransformObjectToHClip(v.vertex);
        o.uv = v.texcoord;
        return o;
    }

    float4 frag(v2f_img i) : SV_Target
    {
        float4 glitch = tex2D(_GlitchTex, i.uv);

        float thresh = 1.001 - _Intensity * 1.001;
        float w_d = step(thresh, pow(glitch.z, 2.5)); // Displacement glitch
        float w_b = step(thresh, pow(glitch.w, 2.5)); // Buffer glitch
        float w_c = step(thresh, pow(glitch.z, 3.5)); // Color glitch

        // Displacement.
        float2 uv = i.uv + glitch.xy * w_d;
        float4 source = tex2D(_MainTex, uv);

        // Mix with a buffer.
        float3 color = lerp(source, tex2D(_BufferTex, uv), w_b).rgb;

        // Shuffle color components.
        color = lerp(color, color - source.bbg * 2 + color.grr * 2, w_c);

        return float4(color, source.a);
    }
    ENDHLSL

    Subshader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off
            Fog
            {
                Mode off
            }
            HLSLPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert_img
            #pragma fragment frag
            ENDHLSL
        }
    }
}