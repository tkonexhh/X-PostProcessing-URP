#pragma once

#include "./Global.hlsl"

half3 _FogColor;
float _FogGlobalDensity;
float _FogFallOff;
float _FogHeight;
float _FogStartDistance;
float _FogGradientDistance;
float _FogInscatteringExp;
half3 _InscatteringColor;
float _InscatteringStartDistance;


float _FogOpacity;





///////////////////////////////////////////////////////////////////////////////
//                               FOG                                         //
///////////////////////////////////////////////////////////////////////////////
half3 IQFog(half3 originColor, float3 positionWS, Light light)
{
    half3 rayDir = normalize(positionWS - _WorldSpaceCameraPos);
    half rayLength = max(distance(positionWS, _WorldSpaceCameraPos), 0);

    half heightFallOff = _FogFallOff * 0.01;
    float falloff = heightFallOff * (positionWS.y - _WorldSpaceCameraPos.y - _FogHeight) * 0.6;
    float fogGlobalDensity = _FogGlobalDensity * 0.002;

    float fogDensity = fogGlobalDensity * exp2(-falloff) ;
    float fogFactor = (1 - exp2(-falloff)) / falloff ;
    float distanceFactor = max(((rayLength - _FogStartDistance)) / _FogGradientDistance, 0) ;
    // return lerp(originColor, _FogColor, distanceFactor);
    // return saturate(distanceFactor);

    float fog = saturate(fogFactor * fogDensity * distanceFactor);
    // return fog;

    float3 lightDirection = normalize(_LightDir);
    // float3 lightDirection = light.direction;

    // float fogAmount = 1.0 - exp(-rayLength * fogDensity);
    float sunAmount = max(dot(rayDir, lightDirection), 0.0);
    float inscatterFactor = pow(sunAmount, _FogInscatteringExp);
    
    float upDot = (dot(float3(0, -1, 0), lightDirection));
    inscatterFactor = lerp(inscatterFactor, 0, upDot * 3);
    // return inscatterFactor;
    float dirExponentialHeightLineIntegral = max(rayLength - _InscatteringStartDistance, 0);
    // inscatterFactor *= max(rayLength - _InscatteringStartDistance, 0);
    inscatterFactor *= (1 - saturate(exp2(-dirExponentialHeightLineIntegral)));
    inscatterFactor = saturate(inscatterFactor);
    // return inscatterFactor;

    // half3 fogColor = lerp(_FogColor, _InscatteringColor, inscatterFactor);
    half3 fogColor = lerp(_FogColor, light.color, inscatterFactor);
    // return fogColor;
    return lerp(originColor, fogColor, saturate(min(fog, _FogOpacity)));
}


half3 ApplyFog(half3 col, half3 positionWS)
{
    Light mainLight = GetMainLight();
    return IQFog(col, positionWS, mainLight);
}

half3 ApplyFogSkyBox(half3 col, half3 positionWS, Light light, half fogUV, half LightMask)
{
    float3 rayDir = normalize(positionWS - _WorldSpaceCameraPos);
    float heightFallOff = _FogFallOff * 0.001;
    float falloff = heightFallOff * ((positionWS.y - _WorldSpaceCameraPos.y));


    // float fogGlobalDensity = _FogGlobalDensity * 0.002;
    // float fogDensity = fogGlobalDensity * exp2(-falloff);

    // float fogFactor = (1 - exp2(-falloff)) / falloff;
    float fogFactor = exp(-falloff);
    // return fogFactor;
    float fog = saturate(fogFactor);
    // return fog;


    // float sunAmount = max(dot(rayDir, light.direction), 0.0);
    // float inscatterFactor = pow(sunAmount, _FogInscatteringExp);
    // float upDot = dot(float3(0, -1, 0), light.direction);
    // inscatterFactor = lerp(inscatterFactor, 0, upDot * 3);
    // float dirExponentialHeightLineIntegral = max(rayLength - _InscatteringStartDistance, 0);
    // inscatterFactor *= max(rayLength - _InscatteringStartDistance, 0);
    // inscatterFactor *= (1 - saturate(exp2(-dirExponentialHeightLineIntegral)));
    // return inscatterFactor;


    _FogColor = lerp(_FogColor, light.color.rgb, saturate(LightMask));
    col = lerp(col, _FogColor, saturate(fogUV) * _FogOpacity);

    // return half4(finalFogColor, 1);
    // col = lerp(col, _FogColor, min(fog, _FogOpacity));
    // col = lerp(col, _InscatteringColor, saturate(inscatterFactor));
    return col;
}