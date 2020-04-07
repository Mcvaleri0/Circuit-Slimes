// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/oldSolderSlime"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0.490566,0.490566,0.490566,1)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.03
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_FresnelScale("FresnelScale", Range( 0 , 10)) = 0.5
		_FresnelPower("FresnelPower", Range( 0 , 10)) = 0.5
		_EyeMask("EyeMask", 2D) = "white" {}
		_FresnelColor("FresnelColor", Color) = (0,0.9999995,1,0)
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_FaceMask("FaceMask", 2D) = "white" {}
		_wave("wave", 2D) = "white" {}
		_Metalness("Metalness", Range( 0 , 1)) = 1.258824
		_MetalLightness("MetalLightness", Float) = 1.85
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		float4 _ASEOutlineColor;
		float _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _FaceMask;
		uniform float4 _FaceMask_ST;
		uniform sampler2D _wave;
		uniform sampler2D _EyeMask;
		uniform float4 _EyeMask_ST;
		uniform sampler2D _FaceTexture;
		uniform float4 _FaceTexture_ST;
		uniform float _MetalLightness;
		uniform float4 _FresnelColor;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _Metalness;
		uniform float _Smoothness;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1);
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1);
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv0_FaceMask = i.uv_texcoord * _FaceMask_ST.xy + _FaceMask_ST.zw;
			float2 temp_output_2_0_g2 = uv0_FaceMask;
			float2 break6_g2 = temp_output_2_0_g2;
			float temp_output_25_0_g2 = ( pow( 0.3 , 3.0 ) * 0.1 );
			float2 appendResult8_g2 = (float2(( break6_g2.x + temp_output_25_0_g2 ) , break6_g2.y));
			float4 tex2DNode14_g2 = tex2D( _FaceMask, temp_output_2_0_g2 );
			float temp_output_4_0_g2 = 0.5;
			float3 appendResult13_g2 = (float3(1.0 , 0.0 , ( ( tex2D( _FaceMask, appendResult8_g2 ).g - tex2DNode14_g2.g ) * temp_output_4_0_g2 )));
			float2 appendResult9_g2 = (float2(break6_g2.x , ( break6_g2.y + temp_output_25_0_g2 )));
			float3 appendResult16_g2 = (float3(0.0 , 1.0 , ( ( tex2D( _FaceMask, appendResult9_g2 ).g - tex2DNode14_g2.g ) * temp_output_4_0_g2 )));
			float3 normalizeResult22_g2 = normalize( cross( appendResult13_g2 , appendResult16_g2 ) );
			o.Normal = normalizeResult22_g2;
			float2 uv_TexCoord437 = i.uv_texcoord * float2( 4,1 );
			float2 panner435 = ( _Time.y * float2( 0,0 ) + uv_TexCoord437);
			float2 panner455 = ( _Time.y * float2( 0,-0.2 ) + i.uv_texcoord);
			float simplePerlin2D452 = snoise( panner455*1.51 );
			simplePerlin2D452 = simplePerlin2D452*0.5 + 0.5;
			float clampResult515 = clamp( ( 1.0 - (0.0 + (tex2D( _wave, panner435 ).r - simplePerlin2D452) * (1.0 - 0.0) / (simplePerlin2D452 - simplePerlin2D452)) ) , 0.0 , 1.0 );
			float2 uv_EyeMask = i.uv_texcoord * _EyeMask_ST.xy + _EyeMask_ST.zw;
			float4 tex2DNode377 = tex2D( _EyeMask, uv_EyeMask );
			float temp_output_360_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float2 uv_FaceTexture = i.uv_texcoord * _FaceTexture_ST.xy + _FaceTexture_ST.zw;
			float4 temp_output_378_0 = ( ( 1.0 - ( tex2DNode377.b * temp_output_360_0 ) ) * tex2D( _FaceTexture, uv_FaceTexture ) );
			float temp_output_532_0 = ( clampResult515 - temp_output_378_0.a );
			Gradient gradient519 = NewGradient( 0, 3, 2, float4( 1, 1, 1, 0.3088273 ), float4( 0.08078849, 0.08078849, 0.08078849, 0.9058824 ), float4( 0.05660379, 0.05660379, 0.05660379, 0.997055 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			Gradient gradient23 = NewGradient( 0, 2, 2, float4( 0.2358491, 0.2358491, 0.2358491, 0 ), float4( 0.7264151, 0.7264151, 0.7264151, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 lerpResult528 = lerp( ( temp_output_532_0 * SampleGradient( gradient519, i.uv_texcoord.y ) * _MetalLightness ) , SampleGradient( gradient23, i.uv_texcoord.y ) , float4( 0.8301887,0.8301887,0.8301887,0 ));
			float4 break195 = temp_output_378_0;
			float4 appendResult201 = (float4(break195.r , break195.g , break195.b , break195.a));
			float4 lerpResult35 = lerp( lerpResult528 , appendResult201 , break195.a);
			o.Albedo = lerpResult35.xyz;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV18 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode18 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV18, _FresnelPower ) );
			o.Emission = ( ( ( tex2DNode377 * ( 1.0 - temp_output_360_0 ) ) * _FresnelColor ) + ( fresnelNode18 * _FresnelColor ) ).rgb;
			o.Metallic = ( temp_output_532_0 * _Metalness );
			o.Smoothness = ( ( 1.0 - temp_output_378_0.a ) * _Smoothness );
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17900
288.8;73.6;1246;710;8465.487;3612.659;8.21235;True;False
Node;AmplifyShaderEditor.CommentaryNode;376;-4821.264,-100.255;Inherit;False;1674.985;730.8376;;14;412;397;378;379;395;82;382;381;360;377;367;369;333;413;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;333;-4786.327,367.4234;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;369;-4603.636,366.2891;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;367;-4458.47,366.5682;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;360;-4275.965,329.8663;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.96;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;377;-4334.072,104.0768;Inherit;True;Property;_EyeMask;EyeMask;5;0;Create;True;0;0;False;0;-1;ce02c6bba51f71349bd1782d79c9bf23;ce02c6bba51f71349bd1782d79c9bf23;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;381;-4009.972,111.4403;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;382;-3799.319,155.3098;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;82;-3807.462,327.6896;Inherit;True;Property;_FaceTexture;FaceTexture;7;0;Create;True;0;0;False;0;-1;45590d454fb37c444866c18ea64776b7;a3b7ef34dcb18ac449363241fa2e114c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;538;-3207.524,-2543.839;Inherit;False;2239.86;819.7412;;18;453;455;437;452;434;435;454;436;535;530;548;532;515;536;507;544;531;438;MetalGoop;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;379;-3566.782,187.3146;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;454;-3097.072,-2305.904;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;436;-3109.756,-2037.868;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;437;-3148.715,-2175.296;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;4,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;378;-3409.98,172.07;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;453;-3142.031,-2439.332;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;455;-2870.771,-2376.204;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RelayNode;322;-3239.441,-827.8638;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;435;-2909.89,-2156.503;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;533;-3106.494,-1627.24;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;434;-2685.66,-2164.246;Inherit;True;Property;_wave;wave;9;0;Create;True;0;0;False;0;-1;6aa1ee79ccbab5241830162561a5eb5b;6aa1ee79ccbab5241830162561a5eb5b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;452;-2665.647,-2429.683;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;531;-2034.045,-1990.324;Inherit;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TFHCRemapNode;438;-2287.227,-2246.108;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.5377358;False;2;FLOAT;0.5849056;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;395;-3999.654,519.018;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;544;-1619.777,-1898.033;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;397;-3950.343,527.2363;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;507;-2007.865,-2241.772;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;240;-2977.449,-95.88268;Inherit;False;1731.597;741.7811;;19;104;58;45;214;18;212;211;213;19;20;215;210;48;385;386;400;401;396;402;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;244;-2901.911,-1561.509;Inherit;False;1564.114;950.8055;;17;519;521;520;522;537;35;528;201;316;26;202;28;23;203;248;195;549;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.WireNode;536;-1591.736,-1925.695;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;515;-1820.37,-2242.047;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;396;-2906.358,521.6454;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;401;-2884.527,502.3481;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;532;-1534.149,-2241.069;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;195;-2571.241,-816.2127;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ColorNode;48;-2257.717,452.1035;Inherit;False;Property;_FresnelColor;FresnelColor;6;0;Create;True;0;0;False;0;0,0.9999995,1,0;0.509434,0.509434,0.509434,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;248;-2166.097,-716.1972;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;519;-2855.414,-1501.485;Inherit;False;0;3;2;1,1,1,0.3088273;0.08078849,0.08078849,0.08078849,0.9058824;0.05660379,0.05660379,0.05660379,0.997055;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.WireNode;402;-2883.229,229.8216;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;548;-1327.537,-2034.976;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;521;-2857.926,-1408.137;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;414;-3155.626,859.064;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-2580.846,-1011.567;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;239;-2626.756,789.6791;Inherit;False;971.5482;440.1462;;4;60;13;84;340;Smoothness;0.1367925,0.8419735,1,1;0;0
Node;AmplifyShaderEditor.WireNode;549;-2234.884,-1422.616;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;537;-2842.626,-1241.234;Inherit;False;Property;_MetalLightness;MetalLightness;11;0;Create;True;0;0;False;0;1.85;13.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;210;-1943.362,481.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;419;-3119.66,888.3066;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;530;-1496.048,-1821.802;Inherit;False;Property;_Metalness;Metalness;10;0;Create;True;0;0;False;0;1.258824;0.24;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;23;-2578.647,-1111.303;Inherit;False;0;2;2;0.2358491,0.2358491,0.2358491,0;0.7264151,0.7264151,0.7264151,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.WireNode;203;-2143.791,-732.2613;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;400;-2884.154,212.4765;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;412;-4020.624,25.28154;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;520;-2580.028,-1503.948;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;215;-2003.606,472.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;522;-2215.06,-1385.973;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;340;-2566.654,875.4913;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WireNode;213;-1935.362,458.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;202;-2141.486,-802.7752;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;413;-3972.488,7.777082;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;386;-2809.774,173.5846;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2594.803,204.268;Inherit;False;Property;_FresnelScale;FresnelScale;3;0;Create;True;0;0;False;0;0.5;2.02;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-2594.719,292.7679;Inherit;False;Property;_FresnelPower;FresnelPower;4;0;Create;True;0;0;False;0;0.5;2.09;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;26;-2318.685,-1102.145;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;535;-1208.986,-2235.379;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;243;-2564.697,-491.6529;Inherit;False;763.95;291.7495;;2;161;392;Normals;0.7745169,0.487184,0.8679245,1;0;0
Node;AmplifyShaderEditor.WireNode;211;-1997.606,446.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;84;-2193.522,846.5474;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;214;-1932.873,339.4728;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FresnelNode;18;-2309.294,204.7357;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;161;-2492.772,-448.9744;Inherit;True;Property;_FaceMask;FaceMask;8;0;Create;True;0;0;False;0;0c1158d9d4389144eb45ff513591a518;0c1158d9d4389144eb45ff513591a518;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;316;-2128.505,-824.2957;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;528;-1964.902,-1122.701;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.8301887,0.8301887,0.8301887,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;201;-2274.627,-881.9007;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2226.119,1094.578;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;False;0;0.5;0.622;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;212;-1984.873,41.47329;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;-2456.058,-37.57633;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;534;-911.056,-2199.49;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1912.497,-37.21352;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1888.587,846.1091;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;540;-880.3864,-2158.525;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;392;-2130.105,-443.3719;Inherit;True;NormalCreate;0;;2;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.3;False;4;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1888.874,265.4731;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;35;-1625.528,-906.4466;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;417;-1212.31,-330.7133;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;539;-873.0903,153.0291;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-1549.726,110.2616;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;529;-1110.152,-682.3856;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;416;-1042.047,739.5701;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;411;-813.3829,66.13342;Float;False;True;-1;3;ASEMaterialInspector;0;0;Standard;Slimes/oldSolderSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.03;0.490566,0.490566,0.490566,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;369;0;333;0
WireConnection;367;0;369;0
WireConnection;360;0;367;0
WireConnection;381;0;377;3
WireConnection;381;1;360;0
WireConnection;382;0;381;0
WireConnection;379;0;382;0
WireConnection;378;0;379;0
WireConnection;378;1;82;0
WireConnection;455;0;453;0
WireConnection;455;1;454;0
WireConnection;322;0;378;0
WireConnection;435;0;437;0
WireConnection;435;1;436;0
WireConnection;533;0;322;0
WireConnection;434;1;435;0
WireConnection;452;0;455;0
WireConnection;531;0;533;0
WireConnection;438;0;434;1
WireConnection;438;1;452;0
WireConnection;438;2;452;0
WireConnection;395;0;360;0
WireConnection;544;0;531;3
WireConnection;397;0;395;0
WireConnection;507;0;438;0
WireConnection;536;0;544;0
WireConnection;515;0;507;0
WireConnection;396;0;397;0
WireConnection;401;0;396;0
WireConnection;532;0;515;0
WireConnection;532;1;536;0
WireConnection;195;0;322;0
WireConnection;248;0;195;3
WireConnection;402;0;401;0
WireConnection;548;0;532;0
WireConnection;414;0;378;0
WireConnection;549;0;548;0
WireConnection;210;0;48;0
WireConnection;419;0;414;0
WireConnection;203;0;248;0
WireConnection;400;0;402;0
WireConnection;412;0;377;0
WireConnection;520;0;519;0
WireConnection;520;1;521;2
WireConnection;215;0;48;0
WireConnection;522;0;549;0
WireConnection;522;1;520;0
WireConnection;522;2;537;0
WireConnection;340;0;419;0
WireConnection;213;0;210;0
WireConnection;202;0;203;0
WireConnection;413;0;412;0
WireConnection;386;0;400;0
WireConnection;26;0;23;0
WireConnection;26;1;28;2
WireConnection;535;0;532;0
WireConnection;535;1;530;0
WireConnection;211;0;215;0
WireConnection;84;0;340;3
WireConnection;214;0;213;0
WireConnection;18;2;19;0
WireConnection;18;3;20;0
WireConnection;316;0;202;0
WireConnection;528;0;522;0
WireConnection;528;1;26;0
WireConnection;201;0;195;0
WireConnection;201;1;195;1
WireConnection;201;2;195;2
WireConnection;201;3;195;3
WireConnection;212;0;211;0
WireConnection;385;0;413;0
WireConnection;385;1;386;0
WireConnection;534;0;535;0
WireConnection;58;0;385;0
WireConnection;58;1;212;0
WireConnection;60;0;84;0
WireConnection;60;1;13;0
WireConnection;540;0;534;0
WireConnection;392;1;161;0
WireConnection;45;0;18;0
WireConnection;45;1;214;0
WireConnection;35;0;528;0
WireConnection;35;1;201;0
WireConnection;35;2;316;0
WireConnection;417;0;392;0
WireConnection;539;0;540;0
WireConnection;104;0;58;0
WireConnection;104;1;45;0
WireConnection;529;0;35;0
WireConnection;416;0;60;0
WireConnection;411;0;529;0
WireConnection;411;1;417;0
WireConnection;411;2;104;0
WireConnection;411;3;539;0
WireConnection;411;4;416;0
ASEEND*/
//CHKSM=0D8E3866F5E3F60FF44656E66BFB0861FD536377