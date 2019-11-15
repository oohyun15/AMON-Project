Shader "Custom/S_lightShader"
{
    Properties
    {
		[HDR]_Color ("Emission Color", Color) = (1,1,1,1)
		_RateFactor("Rate Factor", Range(0, 2)) = 1
		_Frequency("Frequency", Range(0, 1)) = .5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
		float _RateFactor, _Frequency;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float rate = frac(sin(_Time.y * _RateFactor) * 2);
			float rate1 = frac(_Time.y * _RateFactor) * 2 - 1;
			float rate2 = 0;
			if (rate1 < _Frequency)
				rate2 = 0;
			else
				rate2 = 1;
			o.Emission = _Color * rate2;

        }
        ENDCG
    }
    FallBack "Diffuse"
}
