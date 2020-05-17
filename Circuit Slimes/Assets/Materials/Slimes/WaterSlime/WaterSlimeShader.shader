// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/WaterSlime"
{
	Properties
	{
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
		[PerRendererData]_OutlineColor1("OutlineColor", Color) = (0,0,0,0)
		_OutlineWidth1("OutlineWidth", Float) = 0.1
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
			float outlineVar = _OutlineWidth1;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _OutlineColor1.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
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

		uniform sampler2D _CausticsTexture;
		uniform float _WaterSpeed;
		uniform float _WaterIntensity;
		uniform sampler2D _ToonRamp;
		uniform float _ShadowScale;
		uniform float _ShadowOffset;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimLightColor;
		uniform float _SpecularGloss;
		uniform float _SpecularIntensity;
		uniform float _OutlineWidth1;
		uniform float4 _OutlineColor1;


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
			float2 temp_cast_0 = (_WaterSpeed).xx;
			float2 panner204 = ( _Time.y * temp_cast_0 + i.uv_texcoord);
			float4 tex2DNode207 = tex2D( _CausticsTexture, panner204 );
			float3 temp_cast_1 = (tex2DNode207.a).xxx;
			float4 temp_cast_3 = (saturate( ( 1.0 - ( ( distance( temp_cast_1 , tex2DNode207.rgb ) - -2.33 ) / max( 4.32 , 1E-05 ) ) ) )).xxxx;
			float div212=256.0/float(28);
			float4 posterize212 = ( floor( temp_cast_3 * div212 ) / div212 );
			Gradient gradient219 = NewGradient( 0, 2, 2, float4( 0, 0.08320529, 0.7924528, 0 ), float4( 0, 0.8330406, 0.8911465, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 appendResult222 = (float4(0.0 , 0.0 , 0.0 , 0.0));
			float4 lerpResult223 = lerp( SampleGradient( gradient219, i.uv_texcoord.y ) , appendResult222 , float4( 0,0,0,0 ));
			float4 Albedo239 = ( ( posterize212 * _WaterIntensity * ( 1.0 - 0.0 ) ) + lerpResult223 );
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
			float2 temp_cast_6 = ((NormalLightDir7*_ShadowScale + _ShadowOffset)).xx;
			float4 Shadow17 = ( Albedo239 * tex2D( _ToonRamp, temp_cast_6 ) );
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
0;73;1408;926;300.9519;599.1896;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;199;-2876.065,-2612.867;Inherit;False;1720.987;685.7514;;12;213;212;211;210;209;208;207;204;203;202;201;238;Water;0,1,0.9590418,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-2843.383,-2273.567;Inherit;False;Property;_WaterSpeed;WaterSpeed;1;0;Create;True;0;0;False;0;0.1294118;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;202;-2830.171,-2393.551;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;201;-2836.266,-2522.476;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;204;-2599.976,-2523.418;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.38,0.32;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;207;-2410.129,-2552.328;Inherit;True;Property;_CausticsTexture;CausticsTexture;0;0;Create;True;0;0;False;0;-1;d85b3c19dd309a84d9f47f9de16cbae6;d85b3c19dd309a84d9f47f9de16cbae6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;10;-2950.128,-263.3036;Inherit;False;663.8181;400.638;;4;8;6;4;5;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;208;-2040.705,-2547.998;Inherit;True;Color Mask;-1;;3;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;-2.33;False;5;FLOAT;4.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-2894.037,-207.8723;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;200;-2853.987,-1740.669;Inherit;False;1676.63;565.8932;;9;239;281;237;223;222;220;219;214;218;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-2889.515,-35.62287;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosterizeNode;212;-1736.66,-2546.654;Inherit;True;28;2;1;COLOR;0,0,0,0;False;0;INT;28;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;210;-1628.702,-2161.963;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2959.23,-801.8189;Inherit;False;665.2891;393.7782;;4;7;2;1;3;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;211;-1776.345,-2293.355;Inherit;False;Property;_WaterIntensity;WaterIntensity;2;0;Create;True;0;0;False;0;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-2909.104,-744.0189;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;214;-2705.369,-1323.528;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TextureCoordinatesNode;218;-2714.974,-1518.882;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;6;-2698.097,-115.6123;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2922.627,-591.1543;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GradientNode;219;-2712.262,-1650.841;Inherit;False;0;2;2;0,0.08320529,0.7924528,0;0,0.8330406,0.8911465,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;213;-1425.121,-2545.735;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;238;-1285.022,-2082.332;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-2526.676,-114.2499;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;2;-2671.66,-694.7734;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2946.161,399.0961;Inherit;False;1952.439;560.0481;;16;81;82;83;84;85;87;92;93;94;86;95;98;96;90;91;88;RimLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.GradientSampleNode;220;-2452.813,-1609.46;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;222;-2408.755,-1389.217;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;223;-2092.192,-1417.19;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;22;-2176.915,-809.9463;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;45;282;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2890.614,461.9851;Inherit;False;Property;_RimOffset;RimOffset;7;0;Create;True;0;0;False;0;0;0.71;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2892.548,562.6331;Inherit;False;8;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;281;-1793.79,-1556.173;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-2503.571,-698.1804;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-2121.828,-565.3772;Inherit;False;Property;_ShadowScale;ShadowScale;4;0;Create;True;0;0;False;0;0;1.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-2119.828,-469.3766;Inherit;False;Property;_ShadowOffset;ShadowOffset;5;0;Create;True;0;0;False;0;0;0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-2675.052,467.0261;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;237;-1734.83,-1419.594;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;282;-2135.272,-681.6094;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;84;-2531.052,467.0261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-1879.495,-562.03;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;239;-1417.904,-1415.912;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;136;-2946.187,1177.078;Inherit;False;1892.912;669.6566;;13;127;129;130;126;123;121;119;115;116;114;137;122;117;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.OneMinusNode;85;-2380.165,467.3721;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-2552.093,591.7951;Inherit;False;Property;_RimPower;RimPower;8;0;Create;True;0;0;False;0;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-2220.166,752.3742;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2908.094,1228.471;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2910.191,1406.924;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;93;-2218.784,842.0401;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1632.24,-732.2325;Inherit;False;239;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;13;-1651.821,-606.0192;Inherit;True;Property;_ToonRamp;ToonRamp;3;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-2006.187,789.8401;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;117;-2678.193,1533.924;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PowerNode;86;-2181.051,467.0261;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1284.229,-678.9005;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2653.193,1267.924;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2550.576,1717.629;Inherit;False;Property;_SpecularGloss;SpecularGloss;10;0;Create;True;0;0;False;0;0.81;0.62;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;-2147.202,-202.4455;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1038.511,-683.6656;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-2435.191,1380.924;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1907.622,468.6621;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1976.411,-136.5582;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;98;-1695.744,466.915;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;47;-1979.109,-37.73679;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;90;-1710.221,586.7951;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;96;-1720.938,744.3442;Inherit;False;Property;_RimLightColor;RimLightColor;6;0;Create;True;0;0;False;0;0,0,0,0;0.759434,0.9846447,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;121;-2277.947,1380.64;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;137;-2156.112,1679.237;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1601.444,-137.4536;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-1424.952,468.0021;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1855.093,1381.079;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;129;-1833.656,1252.483;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1823.607,1720.508;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;9;0;Create;True;0;0;False;0;0.2204734;0.16;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-1159.448,-141.0576;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-1210.789,463.2101;Inherit;False;RimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-1452.645,1357.632;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-1261.831,1353.032;Inherit;False;Specularity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;315.1179,-507.2196;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;315.4011,-414.4244;Inherit;False;88;RimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;283;252.7868,57.31335;Inherit;False;Property;_OutlineWidth1;OutlineWidth;12;0;Create;True;0;0;False;0;0.1;0.07;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;516.6364,-475.3315;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;308.1814,-309.3271;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;284;238.7868,-126.6864;Inherit;False;Property;_OutlineColor1;OutlineColor;11;1;[PerRendererData];Create;True;0;0;False;0;0,0,0,0;0,0.3058824,0.6901961,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;209;-2209.57,-2138.807;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleAddOpNode;134;708.9497,-429.0049;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;285;573.7868,-162.6864;Inherit;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;907.891,-661.8487;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Slimes/WaterSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.06;0,0.3053881,0.6886792,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;204;0;201;0
WireConnection;204;2;203;0
WireConnection;204;1;202;0
WireConnection;207;1;204;0
WireConnection;208;1;207;0
WireConnection;208;3;207;4
WireConnection;212;1;208;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;213;0;212;0
WireConnection;213;1;211;0
WireConnection;213;2;210;0
WireConnection;238;0;213;0
WireConnection;8;0;6;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;220;0;219;0
WireConnection;220;1;218;2
WireConnection;222;0;214;0
WireConnection;223;0;220;0
WireConnection;223;1;222;0
WireConnection;281;0;238;0
WireConnection;7;0;2;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;237;0;281;0
WireConnection;237;1;223;0
WireConnection;84;0;83;0
WireConnection;19;0;282;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;239;0;237;0
WireConnection;85;0;84;0
WireConnection;13;1;19;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;115;0;114;0
WireConnection;115;1;116;1
WireConnection;17;0;45;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;95;0;86;0
WireConnection;95;1;94;0
WireConnection;98;0;95;0
WireConnection;121;0;119;0
WireConnection;121;1;122;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;91;0;98;0
WireConnection;91;1;90;0
WireConnection;91;2;96;0
WireConnection;123;0;121;0
WireConnection;123;1;137;1
WireConnection;123;2;137;2
WireConnection;50;0;49;0
WireConnection;88;0;91;0
WireConnection;126;0;129;0
WireConnection;126;1;123;0
WireConnection;126;2;127;0
WireConnection;130;0;126;0
WireConnection;100;0;18;0
WireConnection;100;1;99;0
WireConnection;134;0;100;0
WireConnection;134;1;131;0
WireConnection;285;0;284;0
WireConnection;285;1;283;0
WireConnection;0;13;134;0
WireConnection;0;11;285;0
ASEEND*/
//CHKSM=9ECFB4FFE632D4181F6B2EBAF0E4131A0CEF04AF