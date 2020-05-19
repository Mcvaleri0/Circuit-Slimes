// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/ElectricSlime"
{
	Properties
	{
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
		[PerRendererData]_OutlineColor("OutlineColor", Color) = (0,0,0,0)
		_SpecularIntensity("SpecularIntensity", Range( 0 , 1)) = 0.2204734
		_OutlineWidth("OutlineWidth", Float) = 0.1
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
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float2 uv_texcoord;
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

		uniform float _LightninglFresnelScale;
		uniform float _LightningFresnelPower;
		uniform sampler2D _LightningTexture;
		uniform float4 _LightningColor;
		uniform float _LightPower;
		uniform sampler2D _ToonRamp;
		uniform float _ShadowScale;
		uniform float _ShadowOffset;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimLightColor;
		uniform float _SpecularGloss;
		uniform float _SpecularIntensity;
		uniform float _OutlineWidth;
		uniform float4 _OutlineColor;


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
			Gradient gradient259 = NewGradient( 0, 3, 2, float4( 0.8584906, 0.3748198, 0, 0 ), float4( 1, 0.6719017, 0, 0.4470588 ), float4( 1, 0.9120437, 0, 1 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 Albedo239 = SampleGradient( gradient259, i.uv_texcoord.y );
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
			float4 Shadow17 = ( Albedo239 * tex2D( _ToonRamp, temp_cast_4 ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 Lighting50 = ( Shadow17 * ase_lightColor );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult6 = dot( ase_normWorldNormal , ase_worldViewDir );
			float NormalViewDir8 = dotResult6;
			float4 RimLight88 = ( saturate( ( pow( ( 1.0 - saturate( ( _RimOffset + NormalViewDir8 ) ) ) , _RimPower ) * ( NormalLightDir7 * ase_lightAtten ) ) ) * ase_lightColor * _RimLightColor );
			float2 _SpecRange = float2(1.3,1.3);
			float dotResult119 = dot( ( i.viewDir + _WorldSpaceLightPos0.xyz ) , ase_worldNormal );
			float smoothstepResult123 = smoothstep( _SpecRange.x , _SpecRange.y , pow( dotResult119 , _SpecularGloss ));
			float Specularity130 = ( ase_lightAtten * smoothstepResult123 * _SpecularIntensity );
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
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
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
			float4 Emission158 = ( float4( 0,0,0,0 ) + ( ( fresnelNode289 * posterize276 * _LightningColor ) + ( _LightningColor * _LightPower ) ) );
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
927;73;480;926;-257.1452;-503.7928;1.3;False;False
Node;AmplifyShaderEditor.CommentaryNode;262;-3442.704,-1627.969;Inherit;False;1834.013;905.8936;;14;305;274;271;269;267;276;275;273;270;268;266;265;264;263;Lightning;1,0.901886,0,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;263;-3392.704,-1577.969;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;10;-2390.865,204.7446;Inherit;False;680.8019;385.5609;;4;8;6;5;4;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2395.974,-331.8191;Inherit;False;675.2889;378.0411;;4;7;2;1;3;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-2317.79,260.1758;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-2313.268,432.4253;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;264;-3131.797,-1409.131;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2366.943,-128.9543;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;6;-2121.85,352.4358;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;265;-3028.876,-1577.855;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.8,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-2353.42,-281.8189;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1950.428,353.7982;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;319;-2444.887,-2199.379;Inherit;False;930.5007;312.373;;4;259;260;261;239;Albedo;0.4065059,0.8207547,0.4934061,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2426.734,798.6848;Inherit;False;1952.439;560.0481;;16;81;82;83;84;85;87;92;93;94;86;95;98;96;90;91;88;RimLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.DotProductOpNode;2;-2115.975,-232.5734;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;266;-2840.014,-1575.066;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;267;-2864.239,-992.1335;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2373.12,962.2217;Inherit;False;8;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;260;-2394.887,-2044.206;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;-2371.186,861.5737;Inherit;False;Property;_RimOffset;RimOffset;9;0;Create;True;0;0;False;0;0;0.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;259;-2392.608,-2149.379;Inherit;False;0;3;2;0.8584906,0.3748198,0,0;1,0.6719017,0,0.4470588;1,0.9120437,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1450.12,-371.0854;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;12;45;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;269;-2721.545,-857.6524;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-0.26;False;2;FLOAT;-0.22;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;268;-2727.85,-1429.063;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-0.26;False;2;FLOAT;0.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1947.886,-235.9804;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-2155.624,866.6148;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;261;-2122.168,-2136.544;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-1393.034,-30.51545;Inherit;False;Property;_ShadowOffset;ShadowOffset;7;0;Create;True;0;0;False;0;0;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1395.034,-126.5163;Inherit;False;Property;_ShadowScale;ShadowScale;6;0;Create;True;0;0;False;0;0;1.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1394.012,-214.2923;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;305;-2712.587,-1259.947;Inherit;True;Property;_LightningTexture;LightningTexture;0;0;Create;True;0;0;False;0;None;01244fb4003e60142a0571daf8673fa2;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.DynamicAppendNode;271;-2557.28,-989.1404;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;270;-2587.238,-1576.189;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;239;-1739.187,-2140.722;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;84;-2011.624,866.6148;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;-2452.521,1508.792;Inherit;False;2112.512;733.7065;;13;130;126;129;127;123;121;122;119;117;115;114;116;137;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-1152.701,-123.1691;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;274;-2430.88,-1054.74;Inherit;True;Property;_TextureSample4;Texture Sample 4;15;0;Create;True;0;0;False;0;-1;None;01244fb4003e60142a0571daf8673fa2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;273;-2426.619,-1568.963;Inherit;True;Property;_TextureSample3;Texture Sample 3;12;0;Create;True;0;0;False;0;-1;None;01244fb4003e60142a0571daf8673fa2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;87;-2032.665,991.3837;Inherit;False;Property;_RimPower;RimPower;10;0;Create;True;0;0;False;0;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;275;-2111.612,-1395.599;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;13;-925.0266,-167.1583;Inherit;True;Property;_ToonRamp;ToonRamp;5;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;85;-1860.738,866.9607;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1700.738,1151.963;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-878.0673,-289.2449;Inherit;False;239;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2416.524,1738.638;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;93;-1699.357,1241.629;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2414.427,1560.185;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosterizeNode;276;-1878.801,-1566.474;Inherit;True;111;2;1;COLOR;0,0,0,0;False;0;INT;111;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2159.526,1599.638;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;86;-1661.623,866.6148;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;272;-1465.237,-1625.574;Inherit;False;2208.071;932.1196;;12;158;301;295;291;287;286;282;283;289;281;285;306;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;117;-2184.526,1865.638;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1486.759,1189.429;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-557.4346,-240.0395;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-1941.524,1712.638;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1445.273,258.5183;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1388.194,868.2507;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-311.7156,-244.8047;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;306;-1330.343,-1362.318;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;281;-1311.971,-1268.593;Inherit;False;Property;_LightninglFresnelScale;LightninglFresnelScale;2;0;Create;True;0;0;False;0;0.5;6.59;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;285;-1308.047,-1159.619;Inherit;False;Property;_LightningFresnelPower;LightningFresnelPower;3;0;Create;True;0;0;False;0;0.5;3.48;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2056.909,2049.344;Inherit;False;Property;_SpecularGloss;SpecularGloss;14;0;Create;True;0;0;False;0;0.81;0.62;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;96;-1201.51,1143.933;Inherit;False;Property;_RimLightColor;RimLightColor;8;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;289;-918.7526,-1272.1;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;137;-1662.444,2010.951;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PowerNode;121;-1784.279,1712.354;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;98;-1176.316,866.5037;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;47;-1277.181,423.227;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;282;-689.6627,-1022.973;Inherit;False;Property;_LightningColor;LightningColor;1;0;Create;True;0;0;False;0;0.3396226,0.3396226,0.3396226,0;1,0.5686275,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;286;-590.7196,-1345.704;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;283;-712.4395,-820.2454;Inherit;False;Property;_LightPower;LightPower;4;0;Create;True;0;0;False;0;0.3118221;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1274.483,324.4056;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;90;-1190.793,986.3837;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-905.5242,867.5907;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;129;-1141.233,1588.986;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1361.425,1712.793;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;287;-364.6236,-1015.428;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.06603771;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-899.5152,323.5102;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;291;-349.2495,-1262.532;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1312.875,2038.34;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;12;0;Create;True;0;0;False;0;0.2204734;0.67;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-937.3608,1689.858;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-691.3611,862.7987;Inherit;False;RimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-457.5194,319.9062;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;295;-87.92558,-1159.438;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-746.5466,1685.258;Inherit;False;Specularity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;454.0932,590.9183;Inherit;False;88;RimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;453.81,498.123;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;301;198.194,-1383.226;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;321;466.4855,855.1619;Inherit;False;Property;_OutlineColor;OutlineColor;11;1;[PerRendererData];Create;True;0;0;False;0;0,0,0,0;0.6698113,0.3442094,0.009478444,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;504.5896,-1391.731;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;655.3284,530.0111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;454.4358,707.3589;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;320;480.4855,1039.161;Inherit;False;Property;_OutlineWidth;OutlineWidth;13;0;Create;True;0;0;False;0;0.1;0.07;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;322;801.4856,819.1619;Inherit;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;847.6417,576.3377;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;453.3627,384.5375;Inherit;False;158;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1046.583,350.0078;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Slimes/ElectricSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.06;0.8396226,0.3769274,0,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;264;0;263;2
WireConnection;264;1;263;1
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;265;0;264;0
WireConnection;8;0;6;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;266;0;265;0
WireConnection;267;0;265;0
WireConnection;269;0;267;1
WireConnection;268;0;266;1
WireConnection;7;0;2;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;261;0;259;0
WireConnection;261;1;260;2
WireConnection;271;0;267;0
WireConnection;271;1;269;0
WireConnection;270;0;266;0
WireConnection;270;1;268;0
WireConnection;239;0;261;0
WireConnection;84;0;83;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;274;0;305;0
WireConnection;274;1;271;0
WireConnection;273;0;305;0
WireConnection;273;1;270;0
WireConnection;275;0;273;0
WireConnection;275;1;274;0
WireConnection;13;1;19;0
WireConnection;85;0;84;0
WireConnection;276;1;275;0
WireConnection;115;0;114;0
WireConnection;115;1;116;1
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;95;0;86;0
WireConnection;95;1;94;0
WireConnection;17;0;45;0
WireConnection;306;0;276;0
WireConnection;289;2;281;0
WireConnection;289;3;285;0
WireConnection;121;0;119;0
WireConnection;121;1;122;0
WireConnection;98;0;95;0
WireConnection;286;0;306;0
WireConnection;91;0;98;0
WireConnection;91;1;90;0
WireConnection;91;2;96;0
WireConnection;123;0;121;0
WireConnection;123;1;137;1
WireConnection;123;2;137;2
WireConnection;287;0;282;0
WireConnection;287;1;283;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;291;0;289;0
WireConnection;291;1;286;0
WireConnection;291;2;282;0
WireConnection;126;0;129;0
WireConnection;126;1;123;0
WireConnection;126;2;127;0
WireConnection;88;0;91;0
WireConnection;50;0;49;0
WireConnection;295;0;291;0
WireConnection;295;1;287;0
WireConnection;130;0;126;0
WireConnection;301;1;295;0
WireConnection;158;0;301;0
WireConnection;100;0;18;0
WireConnection;100;1;99;0
WireConnection;322;0;321;0
WireConnection;322;1;320;0
WireConnection;134;0;100;0
WireConnection;134;1;131;0
WireConnection;0;2;159;0
WireConnection;0;13;134;0
WireConnection;0;11;322;0
ASEEND*/
//CHKSM=914C8338F576AFBA5726F2445E3F476D99408E8D