Shader "Custom/WaterBuffer"
{
    Properties {       
        [HideInInspector] _BufferTex ("Buffer Texture", 2D) = "white" {}
        _mousePos("Mouse Pos", Vector) = (0, 0, 0, 0)
        _dampening("Dampening", float) = 0.04
        _rippleSize("Ripple Size", float) = 0.01
        _delta("Delta", float) = 1.0
       
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            //Texture2D<float4> _BufferTex;
            sampler2D _BufferTex;
            float4 _BufferTex_TexelSize;
            float4 _mousePos;
            float _dampening;
            float _rippleSize;  
            float _delta;     

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {           
                if(_delta == 0.0)
                    return float4(0.0, 0.0, 0.0, 1.0);

                float2 cellSize = float2(1.0 , 1.0) / 1080.0;
                float2 uv = i.uv; 
                
                int2 fragCoord = int2(uv * _BufferTex_TexelSize.zw); // Convert to texel coordinates
                
                // Sample current texel
                float4 texel = tex2Dlod(_BufferTex, float4(uv, 0, 0));
                float pressure = texel.x;
                float pVel = texel.y;

                // Neighbor texels
                float p_right = tex2Dlod(_BufferTex, float4(uv + float2(_BufferTex_TexelSize.x, 0), 0, 0)).x;
                float p_left  = tex2Dlod(_BufferTex, float4(uv - float2(_BufferTex_TexelSize.x, 0), 0, 0)).x;
                float p_up    = tex2Dlod(_BufferTex, float4(uv + float2(0, _BufferTex_TexelSize.y), 0, 0)).x;
                float p_down  = tex2Dlod(_BufferTex, float4(uv - float2(0, _BufferTex_TexelSize.y), 0, 0)).x;

                
                pVel += _delta * (-2.0 * pressure + p_right + p_left) / 4.0;               
                pVel += _delta * (-2.0 * pressure + p_up + p_down) / 4.0; 
                pressure += _delta * pVel;
                pVel -= 0.005 * _delta * pressure;    
                pVel *= 1.0 - _dampening * _delta;    
                pressure *= 0.999;
                
                //x = pressure. y = pressure velocity. Z and W = X and Y gradient
                float4 col = float4(pressure, pVel, (p_right - p_left) / 2.0, (p_up - p_down) / 2.0);    
                
                if(_mousePos.x < 10000.0){
                    float dist = length(_mousePos - (uv));
                    if (dist <= _rippleSize) {
                        col.x += 1.0 - dist / _rippleSize;                       
                    }
                }         

                return col;      
            }
            ENDCG
        }
    }
}
