Shader "Unlit/Fire"
{
    Properties
    {
		_Color1("Color 01", Color) = (1, 1, 1, 1)
		_Color2("Color 02", Color) = (1,1,1,1)
		_Color3("Rim Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
		_GradientTex("Gradient Texture", 2D) = "white"{}
		_DistortionTex("Distortion Texture", 2D) = "white"{}
		_Distortion("Distortion Value", Range(0,1)) = 0.1
		_Edge("edge", Range(0,1)) = 0.1
		_CutOff("Cut off", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100
		Cull Off

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

			fixed4 _Color1, _Color2, _Color3;
            sampler2D _MainTex, _DistortionTex, _GradientTex;
            float4 _MainTex_ST, _DistortionTex_ST, _GradientTex_ST;
			float _Distortion, _Edge, _CutOff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed grad = tex2D(_GradientTex, i.uv).a;
				grad = lerp(1, -1, i.uv.y);
				fixed4 dist = tex2D(_DistortionTex, i.uv);
				fixed4 colorGrad = lerp(_Color1, _Color2, i.uv.y*2);
				
                fixed4 main = tex2D(_MainTex, fixed2(i.uv.x - dist.g*_Distortion, i.uv.y - dist.r*_Distortion - _Time.y*0.5));
				main.a = main.x;
				main += grad; 
				main = clamp(main, 0, 1);

				fixed4 fire = smoothstep(_CutOff, _CutOff + 0.01, main);
				fixed4 fireRim = smoothstep(_CutOff - _Edge, _CutOff + 0.01 - _Edge, main) - fire;
				
				//main = saturate(main);
				//main *= main*main*main*main;
				//main = smoothstep(0.2, 0.25, main);

				

				fire *= colorGrad;
				fireRim *= _Color3 * 2;

				UNITY_APPLY_FOG(i.fogCoord, col);
                return fire + fireRim;
            }
            ENDCG
        }
    }
}
