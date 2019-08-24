Shader "Unlit/UnlitTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.0
 
    }
    SubShader
    {  
        Tags {"RenderType" = "Opaque" "IgnoreProjector"="True" "RenderPipeline" = "LightweightPipeline"}
		
        //Depth only prepass
        Pass
        {
            ZWrite On
            ColorMask 0
        }
 
        //Main pass
        Pass
        {
            //Disable zwrite, disable cull, setup blending and make sure the color mask for this pass is correct
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask RGB
 
            CGPROGRAM
 
            //Define vertex and fragment function
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            //Basic input
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            //Basic fragment input
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
 
            //Vars
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Cutoff;
           
            //Basic vertex function
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
 
            //Alpha cutoff function
            void AlphaDiscard(half alpha, half cutoff)
            {
                clip(alpha - cutoff);
            }
           
            //Basic fragment function with alpha support
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                half3 color = texColor.rgb * _Color.rgb;
                half alpha = texColor.a * _Color.a;
                AlphaDiscard(alpha, _Cutoff);
 
                return half4(color, alpha);
            }
 
            ENDCG
        }
    }
}
