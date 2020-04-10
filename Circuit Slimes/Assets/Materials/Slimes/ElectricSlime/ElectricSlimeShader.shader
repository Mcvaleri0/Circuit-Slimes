// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/ElectricSlime"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.03
		_ASEOutlineColor( "Outline Color", Color ) = (0.854902,0.3532894,0.0235294,1)
		_FaceOffset1("FaceOffset", Vector) = (0,0,0,0)
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_FaceMask("FaceMask", 2D) = "white" {}
		_EyeMask("EyeMask", 2D) = "white" {}
		_LightningTexture("LightningTexture", 2D) = "white" {}
		_LightningColor("LightningColor", Color) = (0.3396226,0.3396226,0.3396226,0)
		_LightninglFresnelScale("LightninglFresnelScale", Range( 0 , 10)) = 0.5
		_LightningFresnelPower("LightningFresnelPower", Range( 0 , 10)) = 0.5
		_LightPower("LightPower", Range( 0 , 1)) = 0.3118221
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowScale("ShadowScale", Float) = 0
		_ShadowOffset("ShadowOffset", Float) = 0
		_RimLightColor("RimLightColor", Color) = (0,0,0,0)
		_RimOffset("RimOffset", Float) = 0
		_RimPower("RimPower", Range( 0 , 4)) = 0
		_SpecularIntensity("SpecularIntensity", Range( 0 , 1)) = 0.2204734
		_SpecularGloss("SpecularGloss", Range( 0 , 1)) = 0.81
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
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
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
		uniform float2 _FaceOffset1;
		uniform float4 _LightningColor;
		uniform float _LightninglFresnelScale;
		uniform float _LightningFresnelPower;
		uniform sampler2D _LightningTexture;
		uniform float _LightPower;
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
			Gradient gradient259 = NewGradient( 0, 3, 2, float4( 0.8584906, 0.3748198, 0, 0 ), float4( 1, 0.6719017, 0, 0.4470588 ), float4( 1, 0.9120437, 0, 1 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 uv_TexCoord313 = i.uv_texcoord * float2( 3.86,1.43 ) + ( float2( -2.38,-0.26 ) + _FaceOffset1 );
			float2 FaceUV314 = uv_TexCoord313;
			float4 tex2DNode229 = tex2D( _EyeMask, FaceUV314 );
			float temp_output_228_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float4 break214 = saturate( ( ( 1.0 - ( tex2DNode229.b * temp_output_228_0 ) ) * tex2D( _FaceTexture, FaceUV314 ) ) );
			float4 appendResult222 = (float4(break214.r , break214.g , break214.b , break214.a));
			float4 lerpResult223 = lerp( SampleGradient( gradient259, i.uv_texcoord.y ) , appendResult222 , break214.a);
			float4 Albedo239 = lerpResult223;
			float2 temp_output_2_0_g2 = FaceUV314;
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
			float2 temp_cast_5 = ((NormalLightDir7*_ShadowScale + _ShadowOffset)).xx;
			float4 Shadow17 = ( Albedo239 * tex2D( _ToonRamp, temp_cast_5 ) );
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
			float4 FaceMask254 = tex2D( _FaceMask, FaceUV314 );
			float4 Specularity130 = ( ase_lightAtten * ( smoothstepResult123 * FaceMask254 ) * _SpecularIntensity );
			c.rgb = ( ( Lighting50 + RimLight88 ) + Specularity130 ).xyz;
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
			float2 uv_TexCoord313 = i.uv_texcoord * float2( 3.86,1.43 ) + ( float2( -2.38,-0.26 ) + _FaceOffset1 );
			float2 FaceUV314 = uv_TexCoord313;
			float4 tex2DNode229 = tex2D( _EyeMask, FaceUV314 );
			float temp_output_228_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV289 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode289 = ( 0.0 + _LightninglFresnelScale * pow( 1.0 - fresnelNdotV289, _LightningFresnelPower ) );
			float4 appendResult264 = (float4(i.uv_texcoord.y , i.uv_texcoord.x , 0.0 , 0.0));
			float2 panner265 = ( 1.0 * _Time.y * float2( -0.8,0 ) + appendResult264.xy);
			float2 break266 = panner265;
			float4 appendResult270 = (float4(break266.x , (break266.y*-0.26 + 0.51) , 0.0 , 0.0));
			float2 break267 = panner265;
			float4 appendResult271 = (float4(break267.x , (break267.y*-0.26 + -0.22) , 0.0 , 0.0));
			float div276=256.0/float(111);
			float4 posterize276 = ( floor( ( tex2D( _LightningTexture, appendResult270.xy ) + tex2D( _LightningTexture, appendResult271.xy ) ) * div276 ) / div276 );
			float4 Emission158 = ( ( tex2DNode229 * ( 1.0 - temp_output_228_0 ) * _LightningColor ) + ( ( fresnelNode289 * posterize276 * _LightningColor ) + ( _LightningColor * _LightPower ) ) );
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
288.8;73.6;1246;710;8302.763;5259.054;9.942534;True;False
Node;AmplifyShaderEditor.CommentaryNode;308;-4611.892,-2665.71;Inherit;False;896.6765;579.775;Adjusted UV mapping to be able to control face posiiton;6;314;313;312;311;310;309;Face UVs;0.4575472,0.5343004,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;309;-4522.384,-2224.597;Inherit;False;Property;_FaceOffset1;FaceOffset;2;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;310;-4535.696,-2371.28;Inherit;False;Constant;_BaseFaceOffset1;BaseFaceOffset;18;0;Create;True;0;0;False;0;-2.38,-0.26;-2.38,-0.26;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;224;-2679.958,-2584.834;Inherit;False;1860.597;582.574;;13;235;232;234;231;230;229;228;227;226;225;242;315;316;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.Vector2Node;312;-4534.71,-2562.28;Inherit;False;Constant;_BaseFaceTilling1;BaseFaceTilling;19;0;Create;True;0;0;False;0;3.86,1.43;3.86,1.43;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;311;-4310.384,-2280.597;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;225;-2646.156,-2229.301;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;313;-4172.165,-2568.643;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-2463.465,-2228.776;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;314;-3916.894,-2573.841;Inherit;False;FaceUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;36;-134.7748,-1624.91;Inherit;False;1117.222;525.9695;;6;255;254;250;34;249;317;NormalMap;0.9137255,0.8822335,0.4117648,1;0;0
Node;AmplifyShaderEditor.SinOpNode;227;-2318.299,-2228.497;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;315;-2469.704,-2471.577;Inherit;False;314;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;317;30.2019,-1215.887;Inherit;False;314;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;249;-82.02667,-1559.336;Inherit;True;Property;_FaceMask;FaceMask;4;0;Create;True;0;0;False;0;0c1158d9d4389144eb45ff513591a518;503735f12f3c6a94daf5ce78ee32c1ca;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;229;-2233.726,-2495.968;Inherit;True;Property;_EyeMask;EyeMask;5;0;Create;True;0;0;False;0;-1;ce02c6bba51f71349bd1782d79c9bf23;7dde2c7fb377a084ba4436f623a28ce8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;228;-2173.961,-2260.22;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.96;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;250;348.2491,-1551.885;Inherit;True;NormalCreate;0;;2;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-1843.251,-2490.264;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;641.6254,-1557.284;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;10;-2592.468,204.7446;Inherit;False;882.4052;396.7347;;5;8;6;4;5;37;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;262;-4635.917,-1766.13;Inherit;False;1834.013;905.8936;;14;305;274;271;269;267;276;275;273;270;268;266;265;264;263;Lightning;1,0.901886,0,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;316;-1828.976,-2206.497;Inherit;False;314;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;231;-1632.597,-2446.395;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.CommentaryNode;9;-2595.714,-331.8191;Inherit;False;875.0286;393.7782;;5;7;2;1;3;35;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;263;-4585.917,-1716.13;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;232;-1640.74,-2274.013;Inherit;True;Property;_FaceTexture;FaceTexture;3;0;Create;True;0;0;False;0;-1;45590d454fb37c444866c18ea64776b7;64c1e1a0b32d115469a69023fe41d68e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;37;-2549.333,271.2427;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;234;-1400.06,-2414.39;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-2317.79,260.1758;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;235;-1243.257,-2429.635;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;264;-4325.01,-1547.292;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-2577.912,-241.5608;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-2313.268,432.4253;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;242;-1020.875,-2429.718;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;265;-4222.089,-1716.016;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.8,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2366.943,-128.9543;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;1;-2353.42,-281.8189;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;6;-2121.85,352.4358;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;200;-660.2883,-2599.697;Inherit;False;1332.22;636.8583;;8;239;261;259;260;223;222;243;214;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2582.645,869.0961;Inherit;False;1952.439;560.0481;;16;81;82;83;84;85;87;92;93;94;86;95;98;96;90;91;88;RimLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;2;-2115.975,-232.5734;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;214;-511.6712,-2182.556;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1950.428,353.7982;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;259;-499.5088,-2545.051;Inherit;False;0;3;2;0.8584906,0.3748198,0,0;1,0.6719017,0,0.4470588;1,0.9120437,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;260;-501.7876,-2439.878;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;266;-4033.227,-1713.227;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;267;-4057.452,-1130.295;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;222;-215.0566,-2248.245;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2529.031,1032.633;Inherit;False;8;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;269;-3914.758,-995.814;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-0.26;False;2;FLOAT;-0.22;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2527.097,931.985;Inherit;False;Property;_RimOffset;RimOffset;15;0;Create;True;0;0;False;0;0;0.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1947.886,-235.9804;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;261;-229.0686,-2532.216;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;243;-68.51923,-2107.775;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;268;-3921.063,-1567.224;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-0.26;False;2;FLOAT;0.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1450.12,-371.0854;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;12;45;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1393.034,-30.51545;Inherit;False;Property;_ShadowOffset;ShadowOffset;13;0;Create;True;0;0;False;0;0;0.48;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;305;-3905.8,-1398.108;Inherit;True;Property;_LightningTexture;LightningTexture;6;0;Create;True;0;0;False;0;None;01244fb4003e60142a0571daf8673fa2;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1394.012,-214.2923;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;271;-3750.493,-1127.302;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;270;-3780.451,-1714.35;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-2311.535,937.0261;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;223;101.5055,-2276.218;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1395.034,-126.5163;Inherit;False;Property;_ShadowScale;ShadowScale;12;0;Create;True;0;0;False;0;0;1.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;-2582.671,1647.078;Inherit;False;2112.512;733.7065;;16;130;126;129;127;132;123;121;122;119;117;115;114;120;116;137;256;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;84;-2167.535,937.0261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;-2541.674,1999.924;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-1152.701,-123.1691;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;274;-3624.093,-1192.901;Inherit;True;Property;_TextureSample4;Texture Sample 4;15;0;Create;True;0;0;False;0;-1;None;01244fb4003e60142a0571daf8673fa2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;239;435.6351,-2282.416;Inherit;False;Albedo;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2546.674,1876.924;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2544.578,1698.471;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;273;-3619.832,-1707.124;Inherit;True;Property;_TextureSample3;Texture Sample 3;12;0;Create;True;0;0;False;0;-1;None;01244fb4003e60142a0571daf8673fa2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2289.676,1737.924;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;13;-925.0266,-167.1583;Inherit;True;Property;_ToonRamp;ToonRamp;11;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;46;-878.0673,-289.2449;Inherit;False;239;Albedo;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1856.649,1222.374;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;85;-2016.649,937.372;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-2188.576,1061.795;Inherit;False;Property;_RimPower;RimPower;16;0;Create;True;0;0;False;0;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;93;-1855.268,1312.04;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;275;-3304.825,-1533.76;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;117;-2314.676,2003.924;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;255;321.5093,-1311.037;Inherit;True;Property;_TextureSample1;Texture Sample 1;18;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1642.67,1259.84;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2187.06,2187.629;Inherit;False;Property;_SpecularGloss;SpecularGloss;18;0;Create;True;0;0;False;0;0.81;0.62;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-2071.675,1850.924;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-557.4346,-240.0395;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;272;-2658.451,-1763.735;Inherit;False;2292.285;932.1196;;15;304;303;158;282;301;299;295;291;289;287;286;285;283;281;306;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;86;-1817.534,937.0261;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;276;-3072.014,-1704.635;Inherit;True;111;2;1;COLOR;0,0,0,0;False;0;INT;111;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1445.273,258.5183;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1544.105,938.662;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;306;-2523.557,-1500.479;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;137;-1792.595,2149.237;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;643.7851,-1310.436;Inherit;False;FaceMask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;285;-2501.261,-1297.78;Inherit;False;Property;_LightningFresnelPower;LightningFresnelPower;9;0;Create;True;0;0;False;0;0.5;3.48;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;121;-1914.43,1850.64;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;281;-2505.185,-1406.754;Inherit;False;Property;_LightninglFresnelScale;LightninglFresnelScale;8;0;Create;True;0;0;False;0;0.5;6.59;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-311.7156,-244.8047;Inherit;False;Shadow;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1491.576,1851.079;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1274.483,324.4056;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightColorNode;90;-1346.704,1056.795;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;96;-1357.421,1214.344;Inherit;False;Property;_RimLightColor;RimLightColor;14;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;283;-1905.653,-958.407;Inherit;False;Property;_LightPower;LightPower;10;0;Create;True;0;0;False;0;0.3118221;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;289;-2111.966,-1410.261;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;98;-1332.227,936.915;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;282;-1882.876,-1161.134;Inherit;False;Property;_LightningColor;LightningColor;7;0;Create;True;0;0;False;0;0.3396226,0.3396226,0.3396226,0;1,0.8228512,0.2028302,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;47;-1277.181,423.227;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;256;-1448.521,2138.242;Inherit;False;254;FaceMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;286;-1783.933,-1483.865;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1133.436,2179.52;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;17;0;Create;True;0;0;False;0;0.2204734;0.67;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;287;-1557.837,-1153.59;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.06603771;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;129;-1147.485,1729.495;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;-1542.463,-1400.693;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;303;-1869.616,-1623.514;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-1133.303,1853.11;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-1061.435,938.002;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-899.5152,323.5102;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;304;-1833.834,-1715.91;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;-1534.07,-1651.04;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;295;-1281.139,-1297.599;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-457.5194,319.9062;Inherit;False;Lighting;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-878.4742,1831.644;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-847.2719,933.21;Inherit;False;RimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-687.66,1827.044;Inherit;False;Specularity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;453.81,498.123;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;454.0932,590.9183;Inherit;False;88;RimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;301;-886.7444,-1557.479;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-574.3336,-1575.007;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;454.4358,707.3589;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;655.3284,530.0111;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;847.6417,576.3377;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;453.3627,384.5375;Inherit;False;158;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1046.583,350.0078;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Slimes/ElectricSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.03;0.854902,0.3532894,0.0235294,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;311;0;310;0
WireConnection;311;1;309;0
WireConnection;313;0;312;0
WireConnection;313;1;311;0
WireConnection;226;0;225;0
WireConnection;314;0;313;0
WireConnection;227;0;226;0
WireConnection;229;1;315;0
WireConnection;228;0;227;0
WireConnection;250;1;249;0
WireConnection;250;2;317;0
WireConnection;230;0;229;3
WireConnection;230;1;228;0
WireConnection;34;0;250;0
WireConnection;231;0;230;0
WireConnection;232;1;316;0
WireConnection;234;0;231;0
WireConnection;4;0;37;0
WireConnection;235;0;234;0
WireConnection;235;1;232;0
WireConnection;264;0;263;2
WireConnection;264;1;263;1
WireConnection;242;0;235;0
WireConnection;265;0;264;0
WireConnection;1;0;35;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;214;0;242;0
WireConnection;8;0;6;0
WireConnection;266;0;265;0
WireConnection;267;0;265;0
WireConnection;222;0;214;0
WireConnection;222;1;214;1
WireConnection;222;2;214;2
WireConnection;222;3;214;3
WireConnection;269;0;267;1
WireConnection;7;0;2;0
WireConnection;261;0;259;0
WireConnection;261;1;260;2
WireConnection;243;0;214;3
WireConnection;268;0;266;1
WireConnection;271;0;267;0
WireConnection;271;1;269;0
WireConnection;270;0;266;0
WireConnection;270;1;268;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;223;0;261;0
WireConnection;223;1;222;0
WireConnection;223;2;243;0
WireConnection;84;0;83;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;274;0;305;0
WireConnection;274;1;271;0
WireConnection;239;0;223;0
WireConnection;273;0;305;0
WireConnection;273;1;270;0
WireConnection;115;0;114;0
WireConnection;115;1;116;1
WireConnection;13;1;19;0
WireConnection;85;0;84;0
WireConnection;275;0;273;0
WireConnection;275;1;274;0
WireConnection;117;0;120;0
WireConnection;255;0;249;0
WireConnection;255;1;317;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;276;1;275;0
WireConnection;95;0;86;0
WireConnection;95;1;94;0
WireConnection;306;0;276;0
WireConnection;254;0;255;0
WireConnection;121;0;119;0
WireConnection;121;1;122;0
WireConnection;17;0;45;0
WireConnection;123;0;121;0
WireConnection;123;1;137;1
WireConnection;123;2;137;2
WireConnection;289;2;281;0
WireConnection;289;3;285;0
WireConnection;98;0;95;0
WireConnection;286;0;306;0
WireConnection;287;0;282;0
WireConnection;287;1;283;0
WireConnection;291;0;289;0
WireConnection;291;1;286;0
WireConnection;291;2;282;0
WireConnection;303;0;228;0
WireConnection;132;0;123;0
WireConnection;132;1;256;0
WireConnection;91;0;98;0
WireConnection;91;1;90;0
WireConnection;91;2;96;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;304;0;229;0
WireConnection;299;0;304;0
WireConnection;299;1;303;0
WireConnection;299;2;282;0
WireConnection;295;0;291;0
WireConnection;295;1;287;0
WireConnection;50;0;49;0
WireConnection;126;0;129;0
WireConnection;126;1;132;0
WireConnection;126;2;127;0
WireConnection;88;0;91;0
WireConnection;130;0;126;0
WireConnection;301;0;299;0
WireConnection;301;1;295;0
WireConnection;158;0;301;0
WireConnection;100;0;18;0
WireConnection;100;1;99;0
WireConnection;134;0;100;0
WireConnection;134;1;131;0
WireConnection;0;2;159;0
WireConnection;0;13;134;0
ASEEND*/
//CHKSM=EBDED0FFAE3D2A6E71948B2834B6F03DD224F439