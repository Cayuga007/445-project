Shader "Custom/InteractiveWater"
{
    Properties
    {
        [Header(Default)][Space(10)]
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [HideInInspector] _BufferTex ("Buffer Texture", 2D) = "black" {}  
        _Tint("Tint (color)", Color) = (1, 1, 1, 1)
        _NormalTex ("Normal Map", 2D) = "bump" {}
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

        //[Header(Interactive Ripple)][Space(10)]
        //_rippleDistortion ("Ripple Distortion", Range(0.0, 1.0)) = 0.3

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

        [Header(Murkiness)][Space(10)]
        _MurkinessColor("Murkiness Color", Color) = (0, 0, 0, 1)
        _Murkiness("Murkiness", Range(0.0, 2.0)) = 0.1

        [Header(Flow Direction)][Space(10)]
        _WaterStaticVelocity("Water static velocity", Vector) = (0.03, 0.03, 0.0, 0.0)
        _WaterFlowVelocity("Water flow velocity", Vector) = (0.03, 0.03, 0.0, 0.0)
        _FoamVelocity("Foam velocity", Vector) = (0.03, 0.03, 0.0, 0.0)
  
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        GrabPass { "_GrabTexture" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityStandardUtils.cginc"
            #include "/Include/WaterUtils.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;                
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float3 viewDir : TEXCOORD3;
                float4 screenPosition : TEXCOORD4;
            };

            //textures
            sampler2D _MainTex;            
            sampler2D _BufferTex;
            sampler2D _GrabTexture;
            sampler2D _NormalTex;
            sampler2D _CameraDepthTexture;
            sampler2D _DistortionTexture;
            sampler2D _NoiseTexture;
            samplerCUBE _Cube;

            float4 _MainTex_ST;
            float4 _NormalTex_ST;
            float4 _DistortionTexture_ST;
            float4 _NoiseTexture_ST;

            //default
            float4 _Tint;
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

            //water Ripple
            //float _rippleDistortion;

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
            float4 _FoamVelocity;

            float Wave(float3 vertex){
                return sin(vertex.x * _WaveFrequency + _Time.y * _WaveSpeed) * cos(vertex.z * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveHeight;
            }


            v2f vert(appdata v)
            {
                v2f o;
                float wave = Wave(v.vertex.xyz);   
                float3 newPosition = v.vertex + v.normal * wave;            
                o.pos = UnityObjectToClipPos(newPosition);
                o.uv = v.uv;
                o.normal =  UnityObjectToWorldNormal(v.normal);;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
                o.screenPosition = ComputeScreenPos(o.pos);
                return o;
            }

            float2 TransformUV(float2 uv, float4 TileOffset){
                return uv * TileOffset.xy + TileOffset.zw;
            }

            float2 AnimatedUV(float2 uv, float4 TileOffset, float2 speed){
                float2 offset = TileOffset.zw + speed * _Time.y;
                return uv * TileOffset.xy + offset;
            }

            float3 NormalStrength(float3 In, float Strength)
            {
                return In.rg * Strength, lerp(1, In.b, saturate(Strength));
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

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 screenUV = i.screenPosition.xy / i.screenPosition.w;
                float3 vNormal = normalize(i.normal);
                float4 finalCol = float4(0.0, 0.0, 0.0, 1.0);

                float4 albedo = tex2D(_MainTex, TransformUV(uv, _MainTex_ST)) * _Tint;

                float3 normal1 = normalize(tex2D(_NormalTex, AnimatedUV(uv, _NormalTex_ST, _WaterStaticVelocity.xy + _WaterFlowVelocity.xy)).rgb * 2.0 - 1.0);
                float3 normal2 = normalize(tex2D(_NormalTex, AnimatedUV(uv, _NormalTex_ST, -_WaterStaticVelocity.xy)).rgb * 2.0 - 1.0);
                float3 normal = lerp(normal1, normal2, 0.4);
                NormalStrength(normal, 1.0);
                normal = normalize(float3(normal.rg + vNormal.rg, normal.b * vNormal.b));

                // interactive ripples from buffer tex
                float4 bufferData = tex2D(_BufferTex, uv);   
                float3 waterNormal = normalize(float3(-bufferData.z, 0.2, -bufferData.w));
                NormalStrength(waterNormal, 1.5);
                normal = normalize(float3(normal.rg + waterNormal.rg, normal.b * waterNormal.b));
                
                //Reflection
                float fresnel = pow(1.0 - saturate(dot(normalize(vNormal), normalize(i.viewDir))), _FresnelPower);
                float3 reflectionVector = reflect(-i.viewDir, normal);
                float4 reflectionColor = texCUBE(_Cube, reflectionVector);
                reflectionColor.rgb = lerp(albedo.rgb, reflectionColor.rgb, _Smoothness) * _Metallic;
                finalCol = lerp(albedo, reflectionColor, 1.0);

                //depth
                float existingDepth01 = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPosition)).r;
                float depthDifference = LinearEyeDepth(existingDepth01) - i.screenPosition.w;


                //distortion
                float2 distortionOffset = tex2D(_DistortionTexture, AnimatedUV(uv, _DistortionTexture_ST, _WaterStaticVelocity.xy)).rg * 2.0 - 1.0;
                float2 distortedUV = screenUV + bufferData.zw * 0.3 + distortionOffset * _DistortionStrength * 0.1;

                // murkiness and screen color
                float murkiness = saturate(depthDifference * _Murkiness / 10.0);                
                float4 ScreenColor = tex2D(_GrabTexture, distortedUV );
                ScreenColor = lerp(ScreenColor, _MurkinessColor, murkiness);
                finalCol = lerp(ScreenColor, finalCol, _ReflectionAmount);
                //finalCol = lerp(finalCol, _MurkinessColor, murkiness);

                //foam
                float foamFactor = saturate(depthDifference / _MaxFoamDistance);
                float foamCutoff = foamFactor * _NoiseCutoffThreshold;
                float noiseSample = tex2D(_NoiseTexture, AnimatedUV(uv,_NoiseTexture_ST, _NoiseScrollSpeed)).r;
                float noiseValue = smoothstep(foamCutoff - 0.01, foamCutoff + 0.01, noiseSample);
                float4 foamColor = _FoamyColor;
                foamColor.a *= noiseValue;
                finalCol = lerp(finalCol, foamColor, foamColor.a);

                //caustics
                float2 waterUv = uv * _CausticsTile + _FoamVelocity.xy * _Time.y;;
                float w = lerp(water_caustics(float3(waterUv, _Time.y*_CausticsSpeed)), water_caustics(float3(waterUv, _Time.y*_CausticsSpeed) + 1.), 0.5);
                float caustic = exp(w*3. - 0.5) * _CausticsIntensity;
                finalCol = lerp(finalCol, _CausticsColor, caustic);

                
                //finalCol = float4(murkiness, murkiness, murkiness, 1.0);

                //finalCol.xyz = pow(finalCol.xyz, float3(0.4545, 0.4545, 0.4545)); // gamma correction  

                finalCol.a = _Transparency;

                return finalCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
