Shader "Custom/InteractiveWaterLit"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Tint("Tint (color)", Color) = (1, 1, 1, 1)
        _WaveSpeed ("Wave Speed", Range(0.1, 2.0)) = 0.5
        _WaveHeight ("Wave Height", Range(0.0, 3.0)) = 0.2
        _WaveFrequency ("Wave Frequency", Range(0.0, 10.0)) = 1.5
        _Transparency ("Transparency", Range(0.0, 1.0)) = 0.7

        [HideInInspector] _BufferTex ("Buffer Texture", 2D) = "black" {}  

        _RippleDistortion ("Ripple Distortion", Range(0.0, 1.0)) = 0.3
        _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        LOD 200
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        GrabPass { "_GrabTexture" }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        //#pragma target 3.5

        #include "UnityCG.cginc"
        #include "UnityStandardUtils.cginc"

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 uv : TEXCOORD0;
            float3 texcoord1 : TEXCOORD1;
            float3 texcoord2 : TEXCOORD2;
            fixed4 color : COLOR;
        };

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float4 GrabTexUV : TEXCOORD1;
            float3 worldPos : TEXCOORD2;
            float3 viewDir : TEXCOORD3;           
        };

        sampler2D _MainTex;
        sampler2D _BufferTex;
        sampler2D _GrabTexture;

        float4 _Tint;
        float _WaveSpeed;
        float _WaveHeight;
        float _WaveFrequency;
        float _Transparency;

        float _RippleDistortion;
        float _Smoothness;

        void vert(inout appdata v, out Input o) {
            //v.vertex.xyz = ... // do things to vertices
            o.uv_MainTex = v.uv;
            float4 hpos = UnityObjectToClipPos (v.vertex);
            o.GrabTexUV = ComputeGrabScreenPos(hpos); // compute the uvs for the grab texture (using Unity's utilities)

            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {            
            float2 uv = IN.uv_MainTex;
            float4 albedo = tex2D(_MainTex, uv) * _Tint;           
            float4 bufferData = tex2D(_BufferTex, uv);

            //float2 screenUV = IN.screenPosition.xy / IN.screenPosition.w;
            //screenUV = screenUV * 0.5 + 0.5;
            //float2 distortedUV = screenUV + bufferData.zw * _RippleDistortion;
            
            float3 grabColor = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(IN.GrabTexUV));
           
            float3 finalColor = grabColor;

            // Water wave normal for lighting
            float3 waterNormal = normalize(float3(-bufferData.z, 0.2, -bufferData.w));
            //finalColor += pow(max(0.0, dot(waterNormal, normalize(float3(-3, 10, 3)))), 60.0 * 1.);
            //finalColor.xyz = pow(finalColor.xyz, float3(0.4545, 0.4545, 0.4545)); // gamma correction  

            //o.Normal = waterNormal;
            o.Albedo = finalColor;
            o.Smoothness = _Smoothness;
            o.Alpha = _Transparency;
        }
        ENDCG
    }

    FallBack "Standard"
}
