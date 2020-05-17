// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ToonShader"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Tint("Tint", Color) = (0,0,0,0)
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowScale("ShadowScale", Float) = 0
		_ShadowOffset("ShadowOffset", Float) = 0
		_NormalMap("NormalMap", 2D) = "bump" {}
		_NormalScale("NormalScale", Float) = 1
		_RimLightColor("RimLightColor", Color) = (0,0,0,0)
		_RimOffset("RimOffset", Float) = 0
		_RimPower("RimPower", Range( 0 , 4)) = 0
		_SpecMap("SpecMap", 2D) = "white" {}
		_SpecularIntensity("SpecularIntensity", Range( 0 , 1)) = 0.2204734
		_SpecularGloss("SpecularGloss", Range( 0 , 1)) = 0.81
		_EmissionTexture("EmissionTexture", 2D) = "black" {}
		_EmissionStrength("EmissionStrength", Range( 0 , 9)) = 1
		_EmissionColor("EmissionColor", Color) = (0.9952283,1,0,1)
		[PerRendererData]_OutlineColor("OutlineColor", Color) = (0,0,0,0)
		_OutlineWidth("OutlineWidth", Float) = 0.1
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
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _OutlineWidth;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _OutlineColor.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
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

		uniform float _EmissionStrength;
		uniform sampler2D _EmissionTexture;
		uniform float4 _EmissionTexture_ST;
		uniform float4 _EmissionColor;
		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _ToonRamp;
		uniform float _NormalScale;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _ShadowScale;
		uniform float _ShadowOffset;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimLightColor;
		uniform float _SpecularGloss;
		uniform sampler2D _SpecMap;
		uniform float4 _SpecMap_ST;
		uniform float _SpecularIntensity;
		uniform float _OutlineWidth;
		uniform float4 _OutlineColor;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += 0;
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
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float4 Albedo43 = ( _Tint * tex2D( _Albedo, uv_Albedo ) );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 Normal34 = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult2 = dot( normalize( (WorldNormalVector( i , Normal34 )) ) , ase_worldlightDir );
			float NormalLightDir7 = dotResult2;
			float2 temp_cast_1 = ((NormalLightDir7*_ShadowScale + _ShadowOffset)).xx;
			float4 Shadow17 = ( Albedo43 * tex2D( _ToonRamp, temp_cast_1 ) );
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
			float2 uv_SpecMap = i.uv_texcoord * _SpecMap_ST.xy + _SpecMap_ST.zw;
			float4 Specularity130 = ( ase_lightAtten * ( smoothstepResult123 * tex2D( _SpecMap, uv_SpecMap ) ) * _SpecularIntensity );
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
			float2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			float4 Emission158 = ( _EmissionStrength * tex2D( _EmissionTexture, uv_EmissionTexture ) * _EmissionColor );
			o.Emission = Emission158.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
				vertexDataFunc( v, customInputData );
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
Version=18000
927;73;480;926;-240.9172;-166.5183;1.3;False;False
Node;AmplifyShaderEditor.CommentaryNode;36;-3592.706,-512.8768;Inherit;False;951.218;327.4774;;3;34;33;160;NormalMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;160;-3561.592,-392.106;Inherit;False;Property;_NormalScale;NormalScale;6;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;33;-3380.557,-446.6935;Inherit;True;Property;_NormalMap;NormalMap;5;0;Create;True;0;0;False;0;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;10;-2461.341,5.495974;Inherit;False;882.4052;396.7347;;5;8;6;4;5;37;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-2877.071,-445.2506;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2464.587,-531.0677;Inherit;False;875.0286;393.7782;;5;7;2;1;3;35;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-2418.206,71.99416;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;35;-2446.785,-440.8095;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-2182.141,233.1766;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;4;-2186.663,60.92724;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;6;-1990.724,153.1872;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2235.816,-328.203;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;1;-2222.293,-481.0676;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;2;-1984.849,-431.8221;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1819.301,154.5496;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;44;-2507.424,-1205.928;Inherit;False;956.4303;465.3293;;4;38;42;41;43;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2451.518,669.847;Inherit;False;1952.439;560.0481;;16;81;82;83;84;85;87;92;93;94;86;95;98;96;90;91;88;RimLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;38;-2457.424,-970.5991;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;-1;None;9dd57f4f906cc1041ad7160a9a308064;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;41;-2441.912,-1155.928;Inherit;False;Property;_Tint;Tint;1;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;-2395.97,732.7359;Inherit;False;Property;_RimOffset;RimOffset;8;0;Create;True;0;0;False;0;0;0.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1816.759,-435.2291;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1250.081,-1212.886;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;12;45;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2397.904,833.3839;Inherit;False;8;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1194.996,-968.3171;Inherit;False;Property;_ShadowScale;ShadowScale;3;0;Create;True;0;0;False;0;0;0.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1192.996,-872.3162;Inherit;False;Property;_ShadowOffset;ShadowOffset;4;0;Create;True;0;0;False;0;0;0.21;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1193.974,-1056.093;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-2065.46,-1039.22;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-2180.408,737.7769;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;-2451.544,1447.829;Inherit;False;2112.512;733.7065;;16;130;126;129;127;132;133;123;121;122;119;117;115;114;120;116;137;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;84;-2036.408,737.7769;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2413.451,1499.222;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2415.547,1677.675;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-1775.793,-981.3581;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-952.6622,-964.97;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;-2410.547,1800.675;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightAttenuation;93;-1724.142,1112.791;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-679.4073,-1143.672;Inherit;False;43;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-2057.449,862.5459;Inherit;False;Property;_RimPower;RimPower;9;0;Create;True;0;0;False;0;0;1.29;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-724.988,-1008.959;Inherit;True;Property;_ToonRamp;ToonRamp;2;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;117;-2183.549,1804.675;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;85;-1885.523,738.123;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1725.523,1023.125;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2158.549,1538.675;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1511.544,1060.591;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;86;-1686.408,737.7769;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-310.596,-1045.44;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-1940.549,1651.675;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2055.933,1988.38;Inherit;False;Property;_SpecularGloss;SpecularGloss;12;0;Create;True;0;0;False;0;0.81;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1236.994,-533.726;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;121;-1783.304,1651.391;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-105.6773,-1048.042;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;137;-1661.468,1949.988;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1412.978,739.4129;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1066.204,-467.8387;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;47;-1068.902,-369.0174;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;133;-1360.003,1931.94;Inherit;True;Property;_SpecMap;SpecMap;10;0;Create;True;0;0;False;0;-1;None;802766653923aed4b9316c723f49a46d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;98;-1201.101,737.666;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1360.45,1651.83;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;90;-1215.577,857.5459;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;96;-1240.595,1017.695;Inherit;False;Property;_RimLightColor;RimLightColor;7;0;Create;True;0;0;False;0;0,0,0,0;1,0.6201182,0.07843132,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-930.3085,738.753;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-691.236,-468.7342;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;157;-1255.611,28.75435;Inherit;False;873.3601;525.9373;Comment;5;161;158;155;143;156;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-999.0684,1653.861;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;129;-1013.25,1530.246;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1002.31,1980.271;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;11;0;Create;True;0;0;False;0;0.2204734;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;143;-1205.612,182.9208;Inherit;True;Property;_EmissionTexture;EmissionTexture;13;0;Create;True;0;0;False;0;-1;None;24fb97131d3fc424287384429e2e6d2b;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-716.1453,733.961;Inherit;False;RimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;161;-1181.2,387.6937;Inherit;False;Property;_EmissionColor;EmissionColor;15;0;Create;True;0;0;False;0;0.9952283,1,0,1;0.9952283,1,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-745.2396,1653.395;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-1195.159,78.7544;Inherit;False;Property;_EmissionStrength;EmissionStrength;14;0;Create;True;0;0;False;0;1;1;0;9;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-249.2404,-472.3381;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;453.81,498.123;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-550.4254,1649.795;Inherit;False;Specularity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;155;-855.0594,163.5159;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;454.0932,590.9183;Inherit;False;88;RimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;163;443.6414,819.1522;Inherit;False;Property;_OutlineColor;OutlineColor;16;1;[PerRendererData];Create;True;0;0;False;0;0,0,0,0;0.5176471,0.2784314,0.1294118,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-612.0848,158.1266;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;454.4358,707.3589;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;655.3284,530.0111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;164;457.6414,1003.152;Inherit;False;Property;_OutlineWidth;OutlineWidth;17;0;Create;True;0;0;False;0;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;453.3627,384.5375;Inherit;False;158;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;847.6417,576.3377;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;162;778.6414,783.1522;Inherit;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1046.583,343.494;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;ToonShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.03;0,0.3396226,0.2064906,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;33;5;160;0
WireConnection;34;0;33;0
WireConnection;4;0;37;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;1;0;35;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;8;0;6;0
WireConnection;7;0;2;0
WireConnection;42;0;41;0
WireConnection;42;1;38;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;84;0;83;0
WireConnection;43;0;42;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;13;1;19;0
WireConnection;117;0;120;0
WireConnection;85;0;84;0
WireConnection;115;0;114;0
WireConnection;115;1;116;1
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;121;0;119;0
WireConnection;121;1;122;0
WireConnection;17;0;45;0
WireConnection;95;0;86;0
WireConnection;95;1;94;0
WireConnection;98;0;95;0
WireConnection;123;0;121;0
WireConnection;123;1;137;1
WireConnection;123;2;137;2
WireConnection;91;0;98;0
WireConnection;91;1;90;0
WireConnection;91;2;96;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;132;0;123;0
WireConnection;132;1;133;0
WireConnection;88;0;91;0
WireConnection;126;0;129;0
WireConnection;126;1;132;0
WireConnection;126;2;127;0
WireConnection;50;0;49;0
WireConnection;130;0;126;0
WireConnection;155;0;156;0
WireConnection;155;1;143;0
WireConnection;155;2;161;0
WireConnection;158;0;155;0
WireConnection;100;0;18;0
WireConnection;100;1;99;0
WireConnection;134;0;100;0
WireConnection;134;1;131;0
WireConnection;162;0;163;0
WireConnection;162;1;164;0
WireConnection;0;2;159;0
WireConnection;0;13;134;0
WireConnection;0;11;162;0
ASEEND*/
//CHKSM=DB7916712FAE35B1177C70A2B5971B058E0151F5