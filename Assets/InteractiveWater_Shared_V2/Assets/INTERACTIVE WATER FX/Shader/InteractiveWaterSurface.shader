Shader "Custom/InteractiveWaterSurface"
{
    Properties
    {
        [Header(Default)][Space(10)]
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HideInInspector] _BufferTex ("Buffer Texture", 2D) = "black" {}  
        _Tint("Tint (color)", Color) = (1, 1, 1, 1)
        _NormalTex ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Map Strength", float) = 1.0
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
        _Metallic ("Metallic", Range(0.001, 1.0)) = 0.5
        _Transparency ("Transparency", Range(0.0, 1.0)) = 0.7

        [Header(Wave (vertex))][Space(10)]
        _WaveSpeed ("Wave Speed", Range(0.1, 2.0)) = 0.5
        _WaveHeight ("Wave Height", Range(0.0, 3.0)) = 0.2
        _WaveFrequency ("Wave Frequency", Range(0.0, 10.0)) = 1.5     

        [Header(Reflection)][Space(10)]
        _FresnelPower ("Fresnel Power", Range(0.0, 5.0)) = 2.0    
        _Cube ("Environment Reflection", CUBE) = "_Skybox" {}     
        _ReflectionAmount ("Reflection Amount", Range(0.0, 1.0)) = 0.5        

        [Header(Distortion)][Space(10)]
        _DistortionTexture("Distortion Texture", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", Range(0, 1)) = 0.3

        [Header(Murkiness)][Space(10)]
        _MurkinessColor("Murkiness Color", Color) = (0, 0, 0, 1)
        _Murkiness("Murkiness", Range(0.0, 2.0)) = 0.1

        [Header(Foam)][Space(10)]
        _FoamyColor("Foamy Color", Color) = (1, 1, 1, 1)
        _MaxFoamDistance("Max Foam Distance", Float) = 0.4
        _MinFoamDistance("Min Foam Distance", Float) = 0.04
        _NoiseTexture("Noise Texture", 2D) = "white" {}
        _NoiseScrollSpeed("Noise Scroll Speed", Vector) = (0.03, 0.03, 0, 0)
        _NoiseCutoffThreshold("Noise Cutoff Threshold", Range(0, 1)) = 0.777

        [Header(Caustics)][Space(10)]
        _CausticsColor("Caustics Color", Color) = (1, 1, 1, 1)
        _CausticsTile("Caustics Tile", Float) = 10.0
        _CausticsSpeed("Caustics Speed", Float) = 0.5
        _CausticsIntensity("Caustics Intensity", Range(0.0, 1.0)) = 0.5

        [Header(Flow Direction)][Space(10)]
        _WaterStaticVelocity("Water static velocity", Vector) = (0.03, 0.03, 0.0, 0.0)
        _WaterFlowVelocity("Water flow velocity", Vector) = (0.03, 0.03, 0.0, 0.0)
        _CausticsVelocity("Caustics velocity", Vector) = (0.03, 0.03, 0.0, 0.0)
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        
        GrabPass { }

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert
        #pragma target 3.0

        #include "/Include/WaterUtils.cginc"
        
        //textures
            sampler2D _MainTex;            
            sampler2D _BufferTex;
            sampler2D _GrabTexture;
            sampler2D _NormalTex;
            sampler2D _CameraDepthTexture;
            sampler2D _DistortionTexture;
            sampler2D _NoiseTexture;
            samplerCUBE _Cube;

            //default
            float4 _Tint;
            float _NormalStrength;
            float _Smoothness;
            float _Metallic;
            float _Transparency;

            //water wave (vertex)            
            float _WaveSpeed;
            float _WaveHeight;
            float _WaveFrequency; 

            //reflection
            float _FresnelPower;
            float _ReflectionAmount;            

            //distortion
            float _DistortionStrength;

            //foam
            float4 _FoamyColor;
            float _NoiseCutoffThreshold;
            float2 _NoiseScrollSpeed;
            float _MaxFoamDistance;
            float _MinFoamDistance;

            //caustics
            float4 _CausticsColor;
            float _CausticsTile;
            float _CausticsSpeed;
            float _CausticsIntensity;

            ///murkiness
            float4 _MurkinessColor;
            float _Murkiness;

            //flow direction
            float4 _WaterStaticVelocity;
            float4 _WaterFlowVelocity;
            float4 _CausticsVelocity;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalTex;
            float2 uv_DistortionTexture;
            float2 uv_NoiseTexture;
            float3 worldPos;
            float3 viewDir;
            float4 screenPos;
            //float3 WorldNormalVector;
            float3 worldNormal;
            //INTERNAL_DATA
        };

        float Wave(float3 pos)
        {
            return sin(pos.x * _WaveFrequency + _Time.y * _WaveSpeed) * 
                   cos(pos.z * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveHeight;
        }

        void vert(inout appdata_full v)
        {
            float wave = Wave(v.vertex.xyz);
            v.vertex.y += wave;


        }

        float2 AnimatedUV(float2 uv, float2 speed){
                float2 offset = speed * _Time.y;
                return uv + offset;
        }

        float3 NormalStrength(float3 In, float Strength)
        {
            return In.rg * Strength, lerp(1, In.b, saturate(Strength));
        }

        float3 NormalBlend_Reoriented(float3 A, float3 B)
        {
            float3 t = A.xyz + float3(0.0, 0.0, 1.0);
            float3 u = B.xyz * float3(-1.0, -1.0, 1.0);
            return (t / t.z) * dot(t, u) - u;
        }

        float water_caustics(float3 pos) {
            float4 n = snoise( pos );

            pos -= 0.07*n.xyz;
            pos *= 1.62;
            n = snoise( pos );

            pos -= 0.07*n.xyz;
            n = snoise( pos );

            pos -= 0.07*n.xyz;
            n = snoise( pos );
            return n.w;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            
            float4 finalCol = float4(0.0, 0.0, 0.0, 1.0);

            float4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Tint;

            float3 normal1 = UnpackScaleNormal(tex2D(_NormalTex, AnimatedUV(IN.uv_NormalTex, _WaterStaticVelocity.xy + _WaterFlowVelocity.xy)), _NormalStrength);
            float3 normal2 = UnpackScaleNormal(tex2D(_NormalTex, AnimatedUV(IN.uv_NormalTex, -_WaterStaticVelocity.xy)), _NormalStrength);
            float3 normal = lerp(normal1, normal2, 0.4);
           
            // interactive ripples from buffer tex
            float4 bufferData = tex2D(_BufferTex, IN.uv_MainTex);   
            float3 RippleNormal = float3(-bufferData.z, 0.2, -bufferData.w);
            float3 RippleNormalize = normalize(float3(-bufferData.z, 0.2, -bufferData.w));
            NormalStrength(RippleNormal, 1.0);
            normal = normalize(normal + RippleNormal);            
            float Ripple = pow(max(0.0, dot(RippleNormalize, normalize(float3(-3, 10, 3)))), 60.0);
            float4 RippleColor = float4(Ripple, Ripple, Ripple, 0.0);

            //Reflection
            float fresnel = pow(1.0 - saturate(dot(normalize(IN.worldNormal), normalize(IN.viewDir))), _FresnelPower);
            float3 reflectionVector = reflect(IN.viewDir, normal);
            float4 reflectionColor = texCUBE(_Cube, reflectionVector);

            //Distortion & screen color
            float2 distortionOffset = tex2D(_DistortionTexture, AnimatedUV(IN.uv_DistortionTexture, _WaterStaticVelocity.xy)).rg * 2.0 - 1.0;
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float2 distortedUV = screenUV  + distortionOffset * _DistortionStrength * 0.1;
            float4 ScreenColor = tex2D(_GrabTexture, distortedUV );

            //depth
            float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)).r;
            float depthDifference = LinearEyeDepth(existingDepth01) - IN.screenPos.w;

            // murkiness
            float murkiness = saturate(depthDifference * _Murkiness / 10.0);                
            ScreenColor = lerp(ScreenColor, _MurkinessColor, murkiness);

            //foam
            float foamFactor = saturate(depthDifference / _MaxFoamDistance);
            float foamCutoff = foamFactor * _NoiseCutoffThreshold;
            float noiseSample = tex2D(_NoiseTexture, AnimatedUV(IN.uv_NoiseTexture, _NoiseScrollSpeed)).r;
            float noiseValue = smoothstep(foamCutoff - 0.01, foamCutoff + 0.01, noiseSample);
            float4 foamColor = _FoamyColor;
            foamColor.a *= noiseValue;

            //caustics
            float2 waterUv = IN.uv_MainTex * _CausticsTile + _CausticsVelocity.xy * _Time.y;;
            float w = lerp(water_caustics(float3(waterUv, _Time.y*_CausticsSpeed)), water_caustics(float3(waterUv, _Time.y*_CausticsSpeed) + 1.), 0.5);
            float caustic = exp(w*3. - 0.5) * _CausticsIntensity;            
            
            //final color
            finalCol = lerp(ScreenColor, reflectionColor, _ReflectionAmount);
            finalCol = lerp(finalCol, foamColor, foamColor.a);
            finalCol = lerp(finalCol, _CausticsColor, caustic);
            finalCol *= albedo;
            finalCol += RippleColor;

            o.Albedo = finalCol;
            o.Normal = normal;
            o.Smoothness = _Smoothness;
            o.Metallic = _Metallic;
            o.Alpha = _Transparency;

            
        }
        ENDCG
    }
    Fallback "Diffuse"
}
