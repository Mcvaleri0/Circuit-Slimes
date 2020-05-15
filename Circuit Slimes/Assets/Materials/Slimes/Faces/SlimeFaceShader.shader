// Upgrade NOTE: upgraded instancing buffer 'SlimesFaceShader' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/FaceShader"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_FaceOffset("FaceOffset", Vector) = (0,0,0,0)
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_FaceTextureBlink("FaceTextureBlink", 2D) = "white" {}
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowScale("ShadowScale", Float) = 0
		_ShadowOffset("ShadowOffset", Float) = 0
		_EmissionColor("EmissionColor", Color) = (1,1,1,0)
		_EmissionStrength("EmissionStrength", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
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

		uniform float4 _EmissionColor;
		uniform float _EmissionStrength;
		uniform sampler2D _FaceTexture;
		uniform sampler2D _FaceTextureBlink;
		uniform sampler2D _ToonRamp;
		uniform float _ShadowScale;
		uniform float _ShadowOffset;
		uniform float _Cutoff = 0.5;

		UNITY_INSTANCING_BUFFER_START(SlimesFaceShader)
			UNITY_DEFINE_INSTANCED_PROP(float2, _FaceOffset)
#define _FaceOffset_arr SlimesFaceShader
		UNITY_INSTANCING_BUFFER_END(SlimesFaceShader)

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 _FaceOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(_FaceOffset_arr, _FaceOffset);
			float2 uv_TexCoord261 = i.uv_texcoord * float2( 2.5,1 ) + ( float2( -1.37,-0.11 ) + _FaceOffset_Instance );
			float2 FaceUV278 = uv_TexCoord261;
			float4 lerpResult290 = lerp( tex2D( _FaceTexture, FaceUV278 ) , tex2D( _FaceTextureBlink, FaceUV278 ) , (( sin( ( _Time.y * 3.0 ) ) > 0.9 ) ? 1.0 :  0.0 ));
			float4 Albedo281 = lerpResult290;
			float AlphaMask283 = Albedo281.a;
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
			float2 temp_cast_4 = ((NormalLightDir7*_ShadowScale + _ShadowOffset)).xx;
			float4 Shadow17 = ( Albedo281 * tex2D( _ToonRamp, temp_cast_4 ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 Lighting50 = ( Shadow17 * ase_lightColor );
			c.rgb = Lighting50.rgb;
			c.a = 1;
			clip( AlphaMask283 - _Cutoff );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float4 color314 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
			float4 color308 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			float2 _FaceOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(_FaceOffset_arr, _FaceOffset);
			float2 uv_TexCoord261 = i.uv_texcoord * float2( 2.5,1 ) + ( float2( -1.37,-0.11 ) + _FaceOffset_Instance );
			float2 FaceUV278 = uv_TexCoord261;
			float4 lerpResult290 = lerp( tex2D( _FaceTexture, FaceUV278 ) , tex2D( _FaceTextureBlink, FaceUV278 ) , (( sin( ( _Time.y * 3.0 ) ) > 0.9 ) ? 1.0 :  0.0 ));
			float4 Albedo281 = lerpResult290;
			float4 break305 = Albedo281;
			float4 appendResult306 = (float4(break305.r , break305.g , break305.b , 0.0));
			float4 lerpResult303 = lerp( color308 , appendResult306 , break305.a);
			float4 Emission316 = saturate( ( _EmissionColor * _EmissionStrength * saturate( ( 1.0 - ( ( distance( color314.rgb , lerpResult303.xyz ) - 0.0 ) / max( 0.2 , 1E-05 ) ) ) ) ) );
			o.Emission = Emission316.rgb;
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
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
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
717;206;1055;546;3053.896;2036.148;1.3;True;True
Node;AmplifyShaderEditor.CommentaryNode;275;-2617.338,-2337.764;Inherit;False;896.6765;579.775;Adjusted UV mapping to be able to control face posiiton;6;261;277;274;264;276;278;Face UVs;0.4575472,0.5343004,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;264;-2541.142,-2043.334;Inherit;False;Constant;_BaseFaceOffset;BaseFaceOffset;18;0;Create;True;0;0;False;0;-1.37,-0.11;-2.38,-0.26;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;276;-2527.83,-1896.651;Inherit;False;InstancedProperty;_FaceOffset;FaceOffset;1;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;274;-2540.155,-2234.334;Inherit;False;Constant;_BaseFaceTilling;BaseFaceTilling;19;0;Create;True;0;0;False;0;2.5,1;3.86,1.43;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;224;-3049.864,-1544.545;Inherit;False;1985.198;772.0947;;11;283;281;282;228;227;226;225;229;232;290;279;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;277;-2315.83,-1952.651;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;261;-2177.611,-2240.697;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;225;-2978.268,-1033.941;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;278;-1930.344,-2231.431;Inherit;False;FaceUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-2795.578,-1033.416;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;279;-2863.564,-1394.912;Inherit;False;278;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;227;-2650.411,-1033.137;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;9;-3396.364,563.8545;Inherit;False;875.0286;393.7782;;4;7;2;1;3;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-3154.07,613.8547;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;232;-2580.189,-1473.214;Inherit;True;Property;_FaceTexture;FaceTexture;2;0;Create;True;0;0;False;0;-1;None;48a2e7fe52b22db4e861499774340e5a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;228;-2510.553,-1032.011;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;229;-2576.536,-1277.709;Inherit;True;Property;_FaceTextureBlink;FaceTextureBlink;3;0;Create;True;0;0;False;0;-1;None;2cc9f034f4219e74a9e6d10d51300ec1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-3167.593,766.7192;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;290;-2180.856,-1328.756;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;2;-2916.625,663.1001;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-2313.639,241.6781;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;12;45;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;281;-1831.909,-1316.291;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;317;-3014.762,-648.3414;Inherit;False;1912.093;601.0745;;13;292;305;314;312;321;320;308;319;316;315;303;306;309;Emission;1,0.8874891,0,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-2748.536,659.6931;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2256.553,582.248;Inherit;False;Property;_ShadowOffset;ShadowOffset;6;0;Create;True;0;0;False;0;0;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;292;-2968.762,-283.2363;Inherit;True;281;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-2257.531,398.4712;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-2258.553,486.2473;Inherit;False;Property;_ShadowScale;ShadowScale;5;0;Create;True;0;0;False;0;0;1.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;305;-2751.37,-278.2405;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-2016.22,489.5944;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-1788.546,445.6052;Inherit;True;Property;_ToonRamp;ToonRamp;4;0;Create;True;0;0;False;0;-1;a01b538848797dc4ab5c717ee3b99fc0;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1781.965,304.392;Inherit;False;281;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;309;-2349.201,-185.3004;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;306;-2477.986,-316.0915;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;308;-2520.682,-525.3414;Inherit;False;Constant;_Black;Black;7;0;Create;True;0;0;False;0;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1420.953,372.724;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;303;-2285.371,-305.2405;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;314;-2234.769,-517.8305;Inherit;False;Constant;_White;White;7;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1175.234,367.9589;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;56;-2303.553,886.999;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;312;-1971.794,-305.0155;Inherit;True;Color Mask;-1;;4;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;320;-1895.973,-590.5639;Inherit;False;Property;_EmissionColor;EmissionColor;7;0;Create;True;0;0;False;0;1,1,1,0;0.6839622,1,0.959169,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;321;-1899.035,-412.4373;Inherit;False;Property;_EmissionStrength;EmissionStrength;8;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;319;-1613.08,-298.2951;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-2132.764,952.8864;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;47;-2135.461,1051.708;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1757.795,951.991;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;282;-1566.964,-1310.194;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SaturateNode;315;-1460.795,-298.2955;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;316;-1303.795,-303.2955;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-1315.799,948.387;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;283;-1296.089,-1247.534;Inherit;False;AlphaMask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-309.9398,-73.2321;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;318;-313.4725,-275.3754;Inherit;False;316;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;284;-316.0511,-153.7038;Inherit;False;283;AlphaMask;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-59.16679,-318.8611;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Slimes/FaceShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;277;0;264;0
WireConnection;277;1;276;0
WireConnection;261;0;274;0
WireConnection;261;1;277;0
WireConnection;278;0;261;0
WireConnection;226;0;225;0
WireConnection;227;0;226;0
WireConnection;232;1;279;0
WireConnection;228;0;227;0
WireConnection;229;1;279;0
WireConnection;290;0;232;0
WireConnection;290;1;229;0
WireConnection;290;2;228;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;281;0;290;0
WireConnection;7;0;2;0
WireConnection;305;0;292;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;13;1;19;0
WireConnection;309;0;305;3
WireConnection;306;0;305;0
WireConnection;306;1;305;1
WireConnection;306;2;305;2
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;303;0;308;0
WireConnection;303;1;306;0
WireConnection;303;2;309;0
WireConnection;17;0;45;0
WireConnection;312;1;303;0
WireConnection;312;3;314;0
WireConnection;319;0;320;0
WireConnection;319;1;321;0
WireConnection;319;2;312;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;282;0;281;0
WireConnection;315;0;319;0
WireConnection;316;0;315;0
WireConnection;50;0;49;0
WireConnection;283;0;282;3
WireConnection;0;2;318;0
WireConnection;0;10;284;0
WireConnection;0;13;18;0
ASEEND*/
//CHKSM=908E209A07BC7551AA4B958475553C8B87FB6048