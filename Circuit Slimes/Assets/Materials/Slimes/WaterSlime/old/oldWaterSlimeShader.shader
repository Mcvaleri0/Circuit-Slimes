// Upgrade NOTE: upgraded instancing buffer 'SlimesoldWaterSlime' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/oldWaterSlime"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.03
		_ASEOutlineColor( "Outline Color", Color ) = (0,0.278462,0.764151,1)
		_FaceOffset("FaceOffset", Vector) = (0,0,0,0)
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_FaceMask("FaceMask", 2D) = "white" {}
		_EyeMask("EyeMask", 2D) = "white" {}
		_CausticsTexture("CausticsTexture", 2D) = "white" {}
		_WaterSpeed("WaterSpeed", Range( 0 , 1)) = 0.1294118
		_WaterIntensity("WaterIntensity", Range( 0 , 1)) = 1
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowScale("ShadowScale", Float) = 0
		_ShadowOffset("ShadowOffset", Float) = 0
		_RimLightColor("RimLightColor", Color) = (0,0,0,0)
		_RimOffset("RimOffset", Float) = 0
		_RimPower("RimPower", Range( 0 , 4)) = 0
		_SpecularIntensity("SpecularIntensity", Range( 0 , 1)) = 0.2204734
		_SpecularGloss("SpecularGloss", Range( 0 , 1)) = 0.81
		_EmissionColor("EmissionColor", Color) = (0.5235849,1,0.9553949,0)
		_EmissionStrength("EmissionStrength", Float) = 0
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
		UNITY_INSTANCING_BUFFER_START(SlimesoldWaterSlime)
		UNITY_DEFINE_INSTANCED_PROP( float4, _ASEOutlineColor )
#define _ASEOutlineColor_arr SlimesoldWaterSlime
		UNITY_DEFINE_INSTANCED_PROP( float, _ASEOutlineWidth )
#define _ASEOutlineWidth_arr SlimesoldWaterSlime
		UNITY_INSTANCING_BUFFER_END(SlimesoldWaterSlime)
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineWidth_arr, _ASEOutlineWidth) );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineColor_arr, _ASEOutlineColor).rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
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
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _EyeMask;
		uniform float4 _EmissionColor;
		uniform float _EmissionStrength;
		uniform sampler2D _CausticsTexture;
		uniform float _WaterSpeed;
		uniform float _WaterIntensity;
		uniform sampler2D _FaceTexture;
		uniform sampler2D _ToonRamp;
		uniform sampler2D _FaceMask;
		uniform float _ShadowScale;
		uniform float _ShadowOffset;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimLightColor;
		uniform float _SpecularGloss;
		uniform float _SpecularIntensity;

		UNITY_INSTANCING_BUFFER_START(SlimesoldWaterSlime)
			UNITY_DEFINE_INSTANCED_PROP(float2, _FaceOffset)
#define _FaceOffset_arr SlimesoldWaterSlime
		UNITY_INSTANCING_BUFFER_END(SlimesoldWaterSlime)


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


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 temp_cast_1 = (_WaterSpeed).xx;
			float2 panner204 = ( _Time.y * temp_cast_1 + i.uv_texcoord);
			float4 tex2DNode207 = tex2D( _CausticsTexture, panner204 );
			float3 temp_cast_2 = (tex2DNode207.a).xxx;
			float4 temp_cast_4 = (saturate( ( 1.0 - ( ( distance( temp_cast_2 , tex2DNode207.rgb ) - -2.33 ) / max( 4.32 , 1E-05 ) ) ) )).xxxx;
			float div212=256.0/float(28);
			float4 posterize212 = ( floor( temp_cast_4 * div212 ) / div212 );
			float2 _FaceOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(_FaceOffset_arr, _FaceOffset);
			float2 uv_TexCoord261 = i.uv_texcoord * float2( 3.86,1.43 ) + ( float2( -2.38,-0.26 ) + _FaceOffset_Instance );
			float2 FaceUV278 = uv_TexCoord261;
			float4 tex2DNode229 = tex2D( _EyeMask, FaceUV278 );
			float temp_output_228_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float4 temp_output_242_0 = saturate( ( ( 1.0 - ( tex2DNode229.b * temp_output_228_0 ) ) * tex2D( _FaceTexture, FaceUV278 ) ) );
			Gradient gradient219 = NewGradient( 0, 2, 2, float4( 0, 0.08320529, 0.7924528, 0 ), float4( 0, 0.8330406, 0.8911465, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 break214 = temp_output_242_0;
			float4 appendResult222 = (float4(break214.r , break214.g , break214.b , break214.a));
			float4 lerpResult223 = lerp( SampleGradient( gradient219, i.uv_texcoord.y ) , appendResult222 , break214.a);
			float4 Albedo239 = ( ( posterize212 * _WaterIntensity * ( 1.0 - temp_output_242_0.a ) ) + lerpResult223 );
			float2 temp_output_2_0_g2 = FaceUV278;
			float2 break6_g2 = temp_output_2_0_g2;
			float temp_output_25_0_g2 = ( pow( 0.5 , 3.0 ) * 0.1 );
			float2 appendResult8_g2 = (float2(( break6_g2.x + temp_output_25_0_g2 ) , break6_g2.y));
			float4 tex2DNode14_g2 = tex2D( _FaceMask, temp_output_2_0_g2 );
			float temp_output_4_0_g2 = 0.5;
			float3 appendResult13_g2 = (float3(1.0 , 0.0 , ( ( tex2D( _FaceMask, appendResult8_g2 ).g - tex2DNode14_g2.g ) * temp_output_4_0_g2 )));
			float2 appendResult9_g2 = (float2(break6_g2.x , ( break6_g2.y + temp_output_25_0_g2 )));
			float3 appendResult16_g2 = (float3(0.0 , 1.0 , ( ( tex2D( _FaceMask, appendResult9_g2 ).g - tex2DNode14_g2.g ) * temp_output_4_0_g2 )));
			float3 normalizeResult22_g2 = normalize( cross( appendResult13_g2 , appendResult16_g2 ) );
			float3 Normal34 = normalizeResult22_g2;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult2 = dot( normalize( (WorldNormalVector( i , Normal34 )) ) , ase_worldlightDir );
			float NormalLightDir7 = dotResult2;
			float2 temp_cast_7 = ((NormalLightDir7*_ShadowScale + _ShadowOffset)).xx;
			float4 Shadow17 = ( Albedo239 * tex2D( _ToonRamp, temp_cast_7 ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 Lighting50 = ( Shadow17 * ase_lightColor );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult6 = dot( normalize( (WorldNormalVector( i , Normal34 )) ) , ase_worldViewDir );
			float NormalViewDir8 = dotResult6;
			float4 RimLight88 = ( saturate( ( pow( ( 1.0 - saturate( ( _RimOffset + NormalViewDir8 ) ) ) , _RimPower ) * ( NormalLightDir7 * ase_lightAtten ) ) ) * ase_lightColor * _RimLightColor );
			float2 _SpecRange = float2(1.3,1.3);
			float dotResult119 = dot( ( ase_worldViewDir + _WorldSpaceLightPos0.xyz ) , (WorldNormalVector( i , Normal34 )) );
			float smoothstepResult123 = smoothstep( _SpecRange.x , _SpecRange.y , pow( dotResult119 , _SpecularGloss ));
			float4 FaceMask254 = tex2D( _FaceMask, FaceUV278 );
			float4 Specularity130 = ( ase_lightAtten * ( smoothstepResult123 * FaceMask254 ) * _SpecularIntensity );
			c.rgb = ( ( Lighting50 + RimLight88 ) + Specularity130 ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 _FaceOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(_FaceOffset_arr, _FaceOffset);
			float2 uv_TexCoord261 = i.uv_texcoord * float2( 3.86,1.43 ) + ( float2( -2.38,-0.26 ) + _FaceOffset_Instance );
			float2 FaceUV278 = uv_TexCoord261;
			float4 tex2DNode229 = tex2D( _EyeMask, FaceUV278 );
			float temp_output_228_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float4 Emission158 = ( ( tex2DNode229 * ( 1.0 - temp_output_228_0 ) ) * _EmissionColor * _EmissionStrength );
			o.Emission = Emission158.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
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
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
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
6.4;0.8;1523;741;1489.793;251.012;2.958883;True;False
Node;AmplifyShaderEditor.CommentaryNode;275;-3510.569,-3123.607;Inherit;False;896.6765;579.775;Adjusted UV mapping to be able to control face posiiton;6;261;277;274;264;276;278;Face UVs;0.4575472,0.5343004,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;276;-3421.061,-2682.494;Inherit;False;InstancedProperty;_FaceOffset;FaceOffset;2;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;264;-3434.373,-2829.177;Inherit;False;Constant;_BaseFaceOffset;BaseFaceOffset;18;0;Create;True;0;0;False;0;-2.38,-0.26;-2.38,-0.26;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;224;-3511.252,-2332.387;Inherit;False;1860.597;582.574;;12;235;234;231;230;229;228;227;226;225;242;232;279;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.Vector2Node;274;-3433.387,-3020.177;Inherit;False;Constant;_BaseFaceTilling;BaseFaceTilling;19;0;Create;True;0;0;False;0;3.86,1.43;3.86,1.43;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;277;-3209.061,-2738.494;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;225;-3477.45,-1976.854;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;261;-3070.842,-3026.54;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-3294.759,-1976.329;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;278;-2823.575,-3017.274;Inherit;False;FaceUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;279;-3348.307,-2220.548;Inherit;False;278;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;227;-3149.593,-1976.05;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;229;-3065.02,-2243.521;Inherit;True;Property;_EyeMask;EyeMask;5;0;Create;True;0;0;False;0;-1;ce02c6bba51f71349bd1782d79c9bf23;ce02c6bba51f71349bd1782d79c9bf23;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;228;-3005.255,-2007.773;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.96;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-2674.545,-2237.817;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;36;-2756.488,-885.2923;Inherit;False;1117.222;525.9695;;6;255;254;250;34;249;280;NormalMap;0.9137255,0.8822335,0.4117648,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;280;-2587.861,-535.9679;Inherit;False;278;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;249;-2703.74,-819.7184;Inherit;True;Property;_FaceMask;FaceMask;4;0;Create;True;0;0;False;0;0c1158d9d4389144eb45ff513591a518;0c1158d9d4389144eb45ff513591a518;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.BreakToComponentsNode;231;-2463.891,-2193.948;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CommentaryNode;199;-1396.688,-2650.267;Inherit;False;1720.987;685.7514;;11;213;212;211;210;209;208;207;204;203;202;201;Water;0,1,0.9590418,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-1364.006,-2310.967;Inherit;False;Property;_WaterSpeed;WaterSpeed;7;0;Create;True;0;0;False;0;0.1294118;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;232;-2326.854,-2041.754;Inherit;True;Property;_FaceTexture;FaceTexture;3;0;Create;True;0;0;False;0;-1;45590d454fb37c444866c18ea64776b7;45590d454fb37c444866c18ea64776b7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;202;-1350.794,-2430.951;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;201;-1356.889,-2559.876;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;234;-2231.354,-2161.943;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;250;-2273.464,-812.267;Inherit;True;NormalCreate;0;;2;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;10;-2509.571,354.7487;Inherit;False;882.4052;396.7347;;5;8;6;4;5;37;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;235;-2074.551,-2177.188;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;204;-1120.599,-2560.818;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.38,0.32;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-1980.088,-817.666;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;207;-930.7518,-2589.728;Inherit;True;Property;_CausticsTexture;CausticsTexture;6;0;Create;True;0;0;False;0;-1;d85b3c19dd309a84d9f47f9de16cbae6;d85b3c19dd309a84d9f47f9de16cbae6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;37;-2466.436,421.2468;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;242;-1852.169,-2177.271;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2512.817,-181.815;Inherit;False;875.0286;393.7782;;5;7;2;1;3;35;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-2495.015,-91.55676;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;208;-561.3282,-2585.398;Inherit;True;Color Mask;-1;;3;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;-2.33;False;5;FLOAT;4.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;200;-1378.764,-1715.736;Inherit;False;1702.787;654.1746;;9;220;223;222;219;218;214;237;239;243;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-2230.371,582.4294;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;4;-2234.893,410.1799;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;209;-730.1929,-2176.207;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;211;-296.9682,-2330.755;Inherit;False;Property;_WaterIntensity;WaterIntensity;8;0;Create;True;0;0;False;0;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;218;-1239.752,-1493.949;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;210;-149.325,-2199.363;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;6;-2038.953,502.4399;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-2270.523,-131.8148;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosterizeNode;212;-257.2831,-2584.054;Inherit;True;28;2;1;COLOR;0,0,0,0;False;0;INT;28;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;214;-1230.147,-1298.595;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2284.046,21.04974;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GradientNode;219;-1237.04,-1625.908;Inherit;False;0;2;2;0,0.08320529,0.7924528,0;0,0.8330406,0.8911465,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.DotProductOpNode;2;-2033.078,-82.56937;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1867.531,503.8023;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;220;-977.5909,-1584.527;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;222;-933.5323,-1364.284;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;54.25584,-2583.135;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2499.748,1019.1;Inherit;False;1952.439;560.0481;;16;81;82;83;84;85;87;92;93;94;86;95;98;96;90;91;88;RimLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;243;-786.9946,-1223.814;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2444.2,1081.989;Inherit;False;Property;_RimOffset;RimOffset;13;0;Create;True;0;0;False;0;0;0.71;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;223;-616.9697,-1392.257;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1367.223,-221.0813;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;12;45;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1864.989,-85.97635;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2446.134,1182.637;Inherit;False;8;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;238;169.4218,-1891.176;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1311.115,-64.28828;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-2228.638,1087.03;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;237;-259.6084,-1394.661;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1312.137,23.48779;Inherit;False;Property;_ShadowScale;ShadowScale;10;0;Create;True;0;0;False;0;0;1.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1310.137,119.4886;Inherit;False;Property;_ShadowOffset;ShadowOffset;11;0;Create;True;0;0;False;0;0;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;-2499.774,1797.082;Inherit;False;2112.512;733.7065;;16;130;126;129;127;132;123;121;122;119;117;115;114;120;116;137;256;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;239;62.31837,-1401.979;Inherit;True;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;84;-2084.638,1087.03;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2461.681,1848.475;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;120;-2458.777,2149.928;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-1069.804,26.83496;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2463.777,2026.928;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldNormalVector;117;-2231.779,2153.928;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;85;-1933.752,1087.376;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-835.5488,-158.3675;Inherit;False;239;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2206.779,1887.928;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1773.752,1372.378;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-2105.679,1211.799;Inherit;False;Property;_RimPower;RimPower;14;0;Create;True;0;0;False;0;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;93;-1772.371,1462.044;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-842.1296,-17.15424;Inherit;True;Property;_ToonRamp;ToonRamp;9;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;255;-2302.441,-571.4197;Inherit;True;Property;_TextureSample1;Texture Sample 1;18;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-474.5376,-90.03545;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1559.773,1409.844;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;86;-1734.637,1087.03;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-1988.778,2000.928;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2104.163,2337.633;Inherit;False;Property;_SpecularGloss;SpecularGloss;16;0;Create;True;0;0;False;0;0.81;0.62;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-228.8186,-94.8006;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;121;-1831.533,2000.644;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;137;-1709.698,2299.241;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;-1976.928,-572.8184;Inherit;False;FaceMask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1362.376,408.5224;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1461.208,1088.666;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;98;-1249.33,1086.919;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;47;-1194.284,573.2311;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;157;-2802.941,-1640.083;Inherit;False;1166.403;574.338;;7;252;158;244;155;245;257;258;Emission;0.6917149,1,0.1273585,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;256;-1365.624,2288.246;Inherit;False;254;FaceMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;90;-1263.807,1206.799;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;96;-1274.524,1364.348;Inherit;False;Property;_RimLightColor;RimLightColor;12;0;Create;True;0;0;False;0;0,0,0,0;0.3820752,1,0.9830373,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1191.586,474.4097;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1408.679,2001.083;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-978.5381,1088.006;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;257;-2637.18,-1516.995;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1050.539,2329.524;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;15;0;Create;True;0;0;False;0;0.2204734;0.12;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-816.6182,473.5143;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-1050.406,2003.114;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;252;-2651.954,-1619.971;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;129;-1064.588,1879.499;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;258;-2382.533,-1157.196;Inherit;False;Property;_EmissionStrength;EmissionStrength;18;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-2385.655,-1566.906;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-374.6225,469.9103;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-764.3749,1083.214;Inherit;False;RimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-795.5772,1981.648;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;245;-2382.06,-1340.423;Inherit;False;Property;_EmissionColor;EmissionColor;17;0;Create;True;0;0;False;0;0.5235849,1,0.9553949,0;0.514151,0.9580836,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;244;-2059.99,-1472.175;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;454.0932,590.9183;Inherit;False;88;RimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;453.81,498.123;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-604.763,1977.048;Inherit;False;Specularity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;655.3284,530.0111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-1885.32,-1559.616;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;454.4358,707.3589;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;453.3627,384.5375;Inherit;False;158;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;847.6417,576.3377;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1046.583,343.494;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Slimes/oldWaterSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.03;0,0.278462,0.764151,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;277;0;264;0
WireConnection;277;1;276;0
WireConnection;261;0;274;0
WireConnection;261;1;277;0
WireConnection;226;0;225;0
WireConnection;278;0;261;0
WireConnection;227;0;226;0
WireConnection;229;1;279;0
WireConnection;228;0;227;0
WireConnection;230;0;229;3
WireConnection;230;1;228;0
WireConnection;231;0;230;0
WireConnection;232;1;279;0
WireConnection;234;0;231;0
WireConnection;250;1;249;0
WireConnection;250;2;280;0
WireConnection;235;0;234;0
WireConnection;235;1;232;0
WireConnection;204;0;201;0
WireConnection;204;2;203;0
WireConnection;204;1;202;0
WireConnection;34;0;250;0
WireConnection;207;1;204;0
WireConnection;242;0;235;0
WireConnection;208;1;207;0
WireConnection;208;3;207;4
WireConnection;4;0;37;0
WireConnection;209;0;242;0
WireConnection;210;0;209;3
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;1;0;35;0
WireConnection;212;1;208;0
WireConnection;214;0;242;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;8;0;6;0
WireConnection;220;0;219;0
WireConnection;220;1;218;2
WireConnection;222;0;214;0
WireConnection;222;1;214;1
WireConnection;222;2;214;2
WireConnection;222;3;214;3
WireConnection;213;0;212;0
WireConnection;213;1;211;0
WireConnection;213;2;210;0
WireConnection;243;0;214;3
WireConnection;223;0;220;0
WireConnection;223;1;222;0
WireConnection;223;2;243;0
WireConnection;7;0;2;0
WireConnection;238;0;213;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;237;0;238;0
WireConnection;237;1;223;0
WireConnection;239;0;237;0
WireConnection;84;0;83;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;117;0;120;0
WireConnection;85;0;84;0
WireConnection;115;0;114;0
WireConnection;115;1;116;1
WireConnection;13;1;19;0
WireConnection;255;0;249;0
WireConnection;255;1;280;0
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;17;0;45;0
WireConnection;121;0;119;0
WireConnection;121;1;122;0
WireConnection;254;0;255;0
WireConnection;95;0;86;0
WireConnection;95;1;94;0
WireConnection;98;0;95;0
WireConnection;123;0;121;0
WireConnection;123;1;137;1
WireConnection;123;2;137;2
WireConnection;91;0;98;0
WireConnection;91;1;90;0
WireConnection;91;2;96;0
WireConnection;257;0;228;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;132;0;123;0
WireConnection;132;1;256;0
WireConnection;252;0;229;0
WireConnection;155;0;252;0
WireConnection;155;1;257;0
WireConnection;50;0;49;0
WireConnection;88;0;91;0
WireConnection;126;0;129;0
WireConnection;126;1;132;0
WireConnection;126;2;127;0
WireConnection;244;0;155;0
WireConnection;244;1;245;0
WireConnection;244;2;258;0
WireConnection;130;0;126;0
WireConnection;100;0;18;0
WireConnection;100;1;99;0
WireConnection;158;0;244;0
WireConnection;134;0;100;0
WireConnection;134;1;131;0
WireConnection;0;2;159;0
WireConnection;0;13;134;0
ASEEND*/
//CHKSM=6A17DB6CABFD0D1601294D42863EA3330ACFD3CD