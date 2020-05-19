// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/SolderSlime"
{
	Properties
	{
		_wave("wave", 2D) = "white" {}
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowScale("ShadowScale", Float) = 0
		_MetalLightness("MetalLightness", Float) = 1.85
		_ShadowOffset("ShadowOffset", Float) = 0
		_RimLightColor("RimLightColor", Color) = (0,0,0,0)
		_RimOffset("RimOffset", Float) = 0
		_RimPower("RimPower", Range( 0 , 4)) = 0
		_SpecularIntensity("SpecularIntensity", Range( 0 , 1)) = 0.2204734
		_SpecularGloss("SpecularGloss", Range( 0 , 1)) = 0.81
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

		uniform sampler2D _wave;
		uniform float _MetalLightness;
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
			float2 uv_TexCoord310 = i.uv_texcoord * float2( 4,1 );
			float2 panner314 = ( _Time.y * float2( 0,0 ) + uv_TexCoord310);
			float2 panner312 = ( _Time.y * float2( 0,-0.2 ) + i.uv_texcoord);
			float simplePerlin2D317 = snoise( panner312*1.51 );
			simplePerlin2D317 = simplePerlin2D317*0.5 + 0.5;
			float clampResult324 = clamp( ( 1.0 - (0.0 + (tex2D( _wave, panner314 ).r - simplePerlin2D317) * (1.0 - 0.0) / (simplePerlin2D317 - simplePerlin2D317)) ) , 0.0 , 1.0 );
			Gradient gradient328 = NewGradient( 0, 3, 2, float4( 1, 1, 1, 0.1705959 ), float4( 0.08078849, 0.08078849, 0.08078849, 0.9058824 ), float4( 0.05660379, 0.05660379, 0.05660379, 0.997055 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			Gradient gradient335 = NewGradient( 0, 2, 2, float4( 0.2358491, 0.2358491, 0.2358491, 0 ), float4( 0.7264151, 0.7264151, 0.7264151, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 lerpResult343 = lerp( ( clampResult324 * SampleGradient( gradient328, i.uv_texcoord.y ) * _MetalLightness ) , SampleGradient( gradient335, i.uv_texcoord.y ) , float4( 0.8301887,0.8301887,0.8301887,0 ));
			float4 Albedo359 = saturate( lerpResult343 );
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
			float4 Shadow17 = ( Albedo359 * tex2D( _ToonRamp, temp_cast_0 ) );
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
0;73;1408;926;111.1308;-289.1418;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;307;-3095.233,-1319.112;Inherit;False;1636.787;613.2786;;11;324;321;319;317;316;314;312;310;309;308;311;MetalGoop;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;309;-2997.466,-813.1411;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;308;-2984.782,-1081.177;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;311;-3029.74,-1214.605;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;310;-3036.424,-950.5679;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;4,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;314;-2797.6,-931.775;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;312;-2758.48,-1151.477;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;317;-2553.356,-1204.956;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;316;-2573.369,-939.5192;Inherit;True;Property;_wave;wave;0;0;Create;True;0;0;False;0;-1;6aa1ee79ccbab5241830162561a5eb5b;6aa1ee79ccbab5241830162561a5eb5b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;319;-2174.937,-1021.381;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.5377358;False;2;FLOAT;0.5849056;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;321;-1895.575,-1017.045;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;322;-1385.354,-1375.041;Inherit;False;1635.719;706.1861;;12;359;361;343;340;338;337;335;331;333;328;330;373;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;10;-2648.291,195.2222;Inherit;False;695.4052;390.7347;;4;8;6;5;4;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;4;-2560.613,250.6534;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;330;-1341.369,-1237.169;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-2556.091,422.9028;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GradientNode;328;-1338.857,-1315.017;Inherit;False;0;3;2;1,1,1,0.1705959;0.08078849,0.08078849,0.08078849,0.9058824;0.05660379,0.05660379,0.05660379,0.997055;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.ClampOpNode;324;-1708.08,-1017.32;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2654.537,-341.3416;Inherit;False;691.0286;378.7782;;4;7;2;1;3;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;373;-1015.435,-1088.005;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;1;-2596.243,-291.3414;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;333;-1054.961,-1025.017;Inherit;False;Property;_MetalLightness;MetalLightness;3;0;Create;True;0;0;False;0;1.85;4.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;6;-2364.673,342.9133;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;335;-1062.09,-924.8351;Inherit;False;0;2;2;0.2358491,0.2358491,0.2358491,0;0.7264151,0.7264151,0.7264151,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2609.766,-138.4767;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GradientSampleNode;337;-1063.471,-1317.48;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;331;-1064.289,-825.0991;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;102;-2582.645,869.0961;Inherit;False;1952.439;560.0481;;16;81;82;83;84;85;87;92;93;94;86;95;98;96;90;91;88;RimLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;338;-698.5027,-1199.505;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-2193.251,344.2757;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;2;-2358.798,-242.0958;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;340;-802.1277,-915.6769;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;82;-2527.097,931.985;Inherit;False;Property;_RimOffset;RimOffset;6;0;Create;True;0;0;False;0;0;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-2190.709,-245.5028;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1692.943,-380.6078;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;12;45;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2529.031,1032.633;Inherit;False;8;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;343;-448.3448,-936.233;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.8301887,0.8301887,0.8301887,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1637.857,-136.0387;Inherit;False;Property;_ShadowScale;ShadowScale;2;0;Create;True;0;0;False;0;0;1.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1636.835,-223.8147;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;361;-170.803,-936.2689;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1635.857,-40.0379;Inherit;False;Property;_ShadowOffset;ShadowOffset;4;0;Create;True;0;0;False;0;0;0.48;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-2311.535,937.0261;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;359;32.58574,-942.0547;Inherit;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-1395.524,-132.6915;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;-2582.671,1647.078;Inherit;False;1883.547;676.4653;;13;129;130;126;127;123;137;121;119;122;115;117;116;114;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;84;-2167.535,937.0261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2546.674,1876.924;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SamplerNode;13;-1167.849,-176.6807;Inherit;True;Property;_ToonRamp;ToonRamp;1;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1856.649,1222.374;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2544.578,1698.471;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;87;-2188.576,1061.795;Inherit;False;Property;_RimPower;RimPower;7;0;Create;True;0;0;False;0;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;93;-1855.268,1312.04;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1120.89,-298.7674;Inherit;False;359;Albedo;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;85;-2016.649,937.372;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;117;-2314.676,2003.924;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-800.2571,-249.562;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;86;-1817.534,937.0261;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1642.67,1259.84;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2289.676,1737.924;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1688.096,248.9959;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-554.5381,-254.3271;Inherit;False;Shadow;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;122;-2187.06,2187.629;Inherit;False;Property;_SpecularGloss;SpecularGloss;9;0;Create;True;0;0;False;0;0.81;0.58;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-2071.675,1850.924;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1544.105,938.662;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;98;-1332.227,936.915;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;121;-1914.43,1850.64;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;96;-1357.421,1214.344;Inherit;False;Property;_RimLightColor;RimLightColor;5;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;137;-1792.595,2149.237;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;90;-1346.704,1056.795;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightColorNode;47;-1520.004,413.7045;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1517.306,314.8831;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-1142.338,313.9877;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-1061.435,938.002;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1362.401,2179.52;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;8;0;Create;True;0;0;False;0;0.2204734;0.21;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1491.576,1851.079;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;129;-1316.347,1718.047;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-700.3419,310.3837;Inherit;False;Lighting;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-1107.439,1831.644;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-847.2719,933.21;Inherit;False;RimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;453.81,498.123;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-916.6248,1827.044;Inherit;False;Specularity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;454.0932,590.9183;Inherit;False;88;RimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;655.3284,530.0111;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;375;460.5359,1035.808;Inherit;False;Property;_OutlineWidth;OutlineWidth;11;0;Create;True;0;0;False;0;0.1;0.07;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;374;446.5359,851.8086;Inherit;False;Property;_OutlineColor;OutlineColor;10;1;[PerRendererData];Create;True;0;0;False;0;0,0,0,0;0.3098039,0.3098039,0.3098039,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;131;454.4358,707.3589;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;847.6417,576.3377;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OutlineNode;376;781.5359,815.8086;Inherit;False;0;True;None;0;0;Front;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1046.583,350.0078;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Slimes/SolderSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.08;0.3113208,0.3113208,0.3113208,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;314;0;310;0
WireConnection;314;1;309;0
WireConnection;312;0;311;0
WireConnection;312;1;308;0
WireConnection;317;0;312;0
WireConnection;316;1;314;0
WireConnection;319;0;316;1
WireConnection;319;1;317;0
WireConnection;319;2;317;0
WireConnection;321;0;319;0
WireConnection;324;0;321;0
WireConnection;373;0;324;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;337;0;328;0
WireConnection;337;1;330;2
WireConnection;338;0;373;0
WireConnection;338;1;337;0
WireConnection;338;2;333;0
WireConnection;8;0;6;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;340;0;335;0
WireConnection;340;1;331;2
WireConnection;7;0;2;0
WireConnection;343;0;338;0
WireConnection;343;1;340;0
WireConnection;361;0;343;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;359;0;361;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;84;0;83;0
WireConnection;13;1;19;0
WireConnection;85;0;84;0
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
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
WireConnection;126;0;129;0
WireConnection;126;1;123;0
WireConnection;126;2;127;0
WireConnection;88;0;91;0
WireConnection;130;0;126;0
WireConnection;100;0;18;0
WireConnection;100;1;99;0
WireConnection;134;0;100;0
WireConnection;134;1;131;0
WireConnection;376;0;374;0
WireConnection;376;1;375;0
WireConnection;0;13;134;0
WireConnection;0;11;376;0
ASEEND*/
//CHKSM=5B720D8A52E63167040EC6699C6433638D48859F