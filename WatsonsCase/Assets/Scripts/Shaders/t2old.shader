Shader "Custom/t2old" {
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}

	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Standard fullforwardshadows finalcolor:BlackAndWhite

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

		sampler2D _MainTex;
	uniform sampler2D _Hatch0;
	uniform sampler2D _Hatch1;

	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};



	// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
	// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
	// #pragma instancing_options assumeuniformscaling
	UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)


		fixed3 Hatching(float2 _uv, half3 col)
	{
		half3 hatch0 = tex2D(_Hatch0, _uv).rgb;
		half3 hatch1 = tex2D(_Hatch1, _uv).rgb;
		half _intensity = (col.r + col.g + col.b) / 3;

		half3 weightsA = saturate((_intensity * 25.0) + half3(-0, -1, -2));
		half3 weightsB = saturate((_intensity * 25.0) + half3(-3, -4, -5));

		weightsA.xy -= weightsA.yz;
		weightsA.z -= weightsB.x;
		weightsB.xy -= weightsB.zy;

		hatch0 = hatch0 * weightsA;
		hatch1 = hatch1 * weightsB;

		half3 hatching = hatch0.r +
			hatch0.g + hatch0.b +
			hatch1.r + hatch1.g +
			hatch1.b;

		return hatching;

	}



	void surf(Input IN, inout SurfaceOutputStandard o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}

	void BlackAndWhite(Input IN, SurfaceOutputStandard o, inout fixed4 color)
	{
		half c = (color.r + color.g + color.b) / 3;
		c += 0.1;
		c = saturate(0.5 + (c - 0.5)*1.2);
		color.r = c;
		color.g = c;
		color.b = c;
		//float2 pos = float2((abs(IN.worldPos.x)+abs(IN.worldPos.z)), abs(IN.worldPos.y));
		//color.rgb = Hatching(IN.uv_MainTex * 3, color.rgb);
		color.r = 1;
		color.g = 1;
		color.b = 1;
		color.a = 1;
		

	}
	ENDCG
	}
		//FallBack "Diffuse"
}