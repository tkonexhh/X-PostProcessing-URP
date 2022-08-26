

Shader "Hidden/PostProcessing/ToneMapping"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" { }
    }
    
    HLSLINCLUDE

    #include "../../../Shader/PostProcessing.hlsl"


    static const float e = 2.71828;

    float W_f(float x, float e0, float e1)
    {
        if (x <= e0)
            return 0;
        if (x >= e1)
            return 1;
        float a = (x - e0) / (e1 - e0);
        return a * a * (3 - 2 * a);
    }
    float H_f(float x, float e0, float e1)
    {
        if (x <= e0)
            return 0;
        if (x >= e1)
            return 1;
        return(x - e0) / (e1 - e0);
    }

    float GranTurismoTonemapper(float x)
    {
        float P = 1;
        float a = 1;
        float m = 0.22;
        float l = 0.4;
        float c = 1.33;
        float b = 0;
        float l0 = (P - m) * l / a;
        float L0 = m - m / a;
        float L1 = m + (1 - m) / a;
        float L_x = m + a * (x - m);
        float T_x = m * pow(x / m, c) + b;
        float S0 = m + l0;
        float S1 = m + a * l0;
        float C2 = a * P / (P - S1);
        float S_x = P - (P - S1) * pow(e, - (C2 * (x - S0) / P));
        float w0_x = 1 - W_f(x, 0, m);
        float w2_x = H_f(x, m + l0, m + l0);
        float w1_x = 1 - w0_x - w2_x;
        float f_x = T_x * w0_x + L_x * w1_x + S_x * w2_x;
        return f_x;
    }


    //https://www.cnblogs.com/yaoling1997/p/16029385.html
    half4 FragGT(VaryingsDefault input): SV_Target
    {
        half4 sceneColor = GetScreenColor(input.uv);
        float r = GranTurismoTonemapper(sceneColor.r);
        float g = GranTurismoTonemapper(sceneColor.g);
        float b = GranTurismoTonemapper(sceneColor.b);
        sceneColor = float4(r, g, b, sceneColor.a);
        return sceneColor;
    }
    /////////////////////////////

    float3 ACESFilm(float3 color)
    {
        float a = 2.51f;
        float b = 0.03f;
        float c = 2.43f;
        float d = 0.59f;
        float e = 0.14f;
        return saturate((color * (a * color + b)) / (color * (c * color + d) + e));
    }

    half4 FragACES(VaryingsDefault input): SV_Target
    {
        half4 sceneColor = GetScreenColor(input.uv);
        return half4(ACESFilm(sceneColor.rgb), 1);
    }


    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" }
        
        Cull Off
        ZWrite Off
        ZTest Always

        
        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragGT

            ENDHLSL

        }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex VertDefault
            #pragma fragment FragACES

            ENDHLSL

        }
    }
}