// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BoardShader"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Tint("Tint", Color) = (0,0,0,0)
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowScale("ShadowScale", Float) = 0
		_ShadowOffset("ShadowOffset", Float) = 0
		_SpecMap("SpecMap", 2D) = "white" {}
		_SpecularIntensity("SpecularIntensity", Range( 0 , 1)) = 0.2204734
		_SpecularGloss("SpecularGloss", Range( 0 , 1)) = 0.81
		_OutlineColor("OutlineColor", Color) = (0.122241,0.3867925,0.2455354,0)
		_OutlineWidth("OutlineWidth", Range( 0 , 0.1)) = 0.1
		_OutlineTransform("OutlineTransform", Vector) = (0,0,0,0)
		_ShadowColor("ShadowColor", Color) = (0,0.05582933,0.5754717,0)
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
			float3 normalizeResult183 = normalize( _OutlineTransform );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 outlineVar = ( normalizeResult183 * _OutlineWidth * ase_vertex3Pos );
			v.vertex.xyz += outlineVar;
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _OutlineColor.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float3 worldPos;
			float3 viewDir;
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

		uniform float4 _Tint;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _ToonRamp;
		uniform float _ShadowScale;
		uniform float _ShadowOffset;
		uniform float4 _ShadowColor;
		uniform float _SpecularGloss;
		uniform sampler2D _SpecMap;
		uniform float4 _SpecMap_ST;
		uniform float _SpecularIntensity;
		uniform float3 _OutlineTransform;
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
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult2 = dot( ase_normWorldNormal , ase_worldlightDir );
			float NormalLightDir7 = dotResult2;
			float2 temp_cast_0 = ((NormalLightDir7*_ShadowScale + _ShadowOffset)).xx;
			float4 Shadow17 = ( Albedo43 * tex2D( _ToonRamp, temp_cast_0 ) * saturate( ( ase_lightAtten + _ShadowColor ) ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 Lighting50 = ( Shadow17 * ase_lightColor );
			float2 _SpecRange = float2(1.3,1.3);
			float dotResult119 = dot( ( i.viewDir + _WorldSpaceLightPos0.xyz ) , ase_worldNormal );
			float smoothstepResult123 = smoothstep( _SpecRange.x , _SpecRange.y , pow( dotResult119 , _SpecularGloss ));
			float2 uv_SpecMap = i.uv_texcoord * _SpecMap_ST.xy + _SpecMap_ST.zw;
			float4 Specularity130 = ( ase_lightAtten * ( smoothstepResult123 * tex2D( _SpecMap, uv_SpecMap ) ) * _SpecularIntensity );
			c.rgb = ( Lighting50 + Specularity130 ).rgb;
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
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
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
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
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
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.viewDir = worldViewDir;
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
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
288.8;73.6;1246;710;273.8473;-580.1809;1.331376;True;False
Node;AmplifyShaderEditor.CommentaryNode;9;-2456.968,-383.7649;Inherit;False;699.9346;373.179;;4;7;2;1;3;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2403.291,-180.9002;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;1;-2389.768,-333.7648;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;2;-2152.324,-284.5193;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;44;-2511.731,-1154.247;Inherit;False;956.4303;465.3293;;4;38;42;41;43;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1296.612,-1369.341;Inherit;False;1357.079;892.5441;;11;190;185;45;46;13;19;12;20;21;209;17;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1984.235,-287.9263;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;38;-2461.731,-918.9182;Inherit;True;Property;_Albedo;Albedo;0;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;41;-2446.219,-1104.247;Inherit;False;Property;_Tint;Tint;1;0;Create;True;0;0;False;0;0,0,0,0;0.07778571,0.7169812,0.5666291,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;136;-2377.846,168.1712;Inherit;False;2112.512;733.7065;;15;130;126;129;127;132;133;123;121;122;119;117;115;114;116;137;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1240.505,-1212.548;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;185;-1249.217,-885.3992;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1239.527,-1028.772;Inherit;False;Property;_ShadowOffset;ShadowOffset;4;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1241.527,-1124.773;Inherit;False;Property;_ShadowScale;ShadowScale;3;0;Create;True;0;0;False;0;0;0.43;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2339.753,219.5643;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;190;-1237.031,-666.0101;Inherit;False;Property;_ShadowColor;ShadowColor;11;0;Create;True;0;0;False;0;0,0.05582933,0.5754717,0;0.5058824,0.7296472,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-2069.767,-987.539;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2341.85,398.0171;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-999.1935,-1121.426;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;43;-1780.1,-929.6772;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldNormalVector;117;-2109.852,525.0171;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2084.852,259.0172;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;208;-958.5546,-819.6189;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-1866.851,372.0171;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-725.9386,-1300.127;Inherit;False;43;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-1982.236,708.7223;Inherit;False;Property;_SpecularGloss;SpecularGloss;7;0;Create;True;0;0;False;0;0.81;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-771.5193,-1165.414;Inherit;True;Property;_ToonRamp;ToonRamp;2;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;209;-697.8676,-821.399;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;121;-1709.606,371.7331;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-357.1273,-1201.895;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;137;-1587.771,670.3302;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;133;-1286.306,652.2821;Inherit;True;Property;_SpecMap;SpecMap;5;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;56;-1285.642,-352.7429;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-123.3085,-1205.497;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1286.752,372.1721;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-925.3708,374.2031;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-928.6124,700.6132;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;6;0;Create;True;0;0;False;0;0.2204734;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;129;-939.5524,250.5882;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1114.852,-286.8557;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;47;-1117.55,-188.0343;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.Vector3Node;184;421.7829,967.9167;Inherit;False;Property;_OutlineTransform;OutlineTransform;10;0;Create;True;0;0;False;0;0,0,0;8.1,20,20;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-671.5421,373.737;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-739.8842,-287.7512;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-297.8887,-291.355;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosVertexDataNode;171;435.471,1229.734;Inherit;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-476.7278,370.1372;Inherit;False;Specularity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NormalizeNode;183;634.524,1042.169;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;164;428.45,1132.466;Inherit;False;Property;_OutlineWidth;OutlineWidth;9;0;Create;True;0;0;False;0;0.1;0.044;0;0.1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;1012.388,748.55;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;181;781.5793,1113.2;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;163;418.1443,783.6019;Inherit;False;Property;_OutlineColor;OutlineColor;8;0;Create;True;0;0;False;0;0.122241,0.3867925,0.2455354,0;0.06314526,0.4056604,0.3722442,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;18;1014.763,556.3141;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;162;1026.267,954.8439;Inherit;False;2;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;1405.594,617.5289;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1604.536,384.6851;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;BoardShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.03;0,0.3396226,0.2064906,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;7;0;2;0
WireConnection;42;0;41;0
WireConnection;42;1;38;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;43;0;42;0
WireConnection;115;0;114;0
WireConnection;115;1;116;1
WireConnection;208;0;185;0
WireConnection;208;1;190;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;13;1;19;0
WireConnection;209;0;208;0
WireConnection;121;0;119;0
WireConnection;121;1;122;0
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;45;2;209;0
WireConnection;17;0;45;0
WireConnection;123;0;121;0
WireConnection;123;1;137;1
WireConnection;123;2;137;2
WireConnection;132;0;123;0
WireConnection;132;1;133;0
WireConnection;126;0;129;0
WireConnection;126;1;132;0
WireConnection;126;2;127;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;50;0;49;0
WireConnection;130;0;126;0
WireConnection;183;0;184;0
WireConnection;181;0;183;0
WireConnection;181;1;164;0
WireConnection;181;2;171;0
WireConnection;162;0;163;0
WireConnection;162;1;181;0
WireConnection;134;0;18;0
WireConnection;134;1;131;0
WireConnection;0;13;134;0
WireConnection;0;11;162;0
ASEEND*/
//CHKSM=99D896D78A8C9E9F4AC30840C6B1F5FA22BBCDDB