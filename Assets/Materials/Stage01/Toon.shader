Shader "Unlit/Toon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_AmbientColor("Ambient Color", Color) = (0.4, 0.4, 0.4, 1)
    }
    SubShader
    {
        Tags { 
		"RenderType"="Opaque" 
		"LightMode"="ForwardBase" //request lighting data to be passed into shader
		}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert //vertex shader's name is vert
            #pragma fragment frag //fragment shader's name is frag
			#pragma multi_compile_fwdbase //instruct unity to compile all varients necessary for forward base rendering

            #include "UnityCG.cginc"
			#include "Lighting.cginc" //fixed4 _LightColor0 included
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;  //automatically populated
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
				float3 worldNormal : NORMAL;  //have to be populated in VS
				float3 viewDir : TEXCOORD1; 
				SHADOW_COORDS(2) //generate 4-dimention value and add it to TEXCOORD
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color, _AmbientColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = WorldSpaceViewDir(v.vertex);
				TRANSFER_SHADOW(o)
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				//shadow
				float shadow = SHADOW_ATTENUATION(i);

				//diffuse
				float3 normal = normalize(i.worldNormal);
				float NdotL = dot(_WorldSpaceLightPos0, normal); //normal dot lightDir
				float lightIntensity = NdotL * shadow;
				if (lightIntensity < 0.1) lightIntensity = 0.3;
				else if (lightIntensity < 0.5) lightIntensity = 0.6;
				else lightIntensity = 1;
				float4 light = lightIntensity * _LightColor0; //main directional light color

				//specular
				float3 viewDir = normalize(i.viewDir);
				float3 halfVector = normalize(_WorldSpaceLightPos0 + viewDir);
				float NdotH = dot(normal, halfVector);
				float SpecularIntensity = pow(NdotH * lightIntensity, 32*32);
				SpecularIntensity = smoothstep(0, 0.01, SpecularIntensity);

				//rim
				float4 rimLight = 1 - dot(viewDir, normal);
				rimLight = smoothstep(0.7, 0.8, rimLight);


                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				return col *(light);
            }
            ENDCG
        }
		UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"  //grabs pass from different shader and insert to ours
    }
}
