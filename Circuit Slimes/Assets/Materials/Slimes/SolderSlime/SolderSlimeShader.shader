// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/SolderSlime"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 0.03
		_ASEOutlineColor( "Outline Color", Color ) = (0.3113208,0.3113208,0.3113208,1)
		_FaceOffset1("FaceOffset", Vector) = (0,0,0,0)
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_FaceMask("FaceMask", 2D) = "white" {}
		_EyeMask("EyeMask", 2D) = "white" {}
		_wave("wave", 2D) = "white" {}
		_ToonRamp("ToonRamp", 2D) = "white" {}
		_ShadowScale("ShadowScale", Float) = 0
		_MetalLightness("MetalLightness", Float) = 1.85
		_ShadowOffset("ShadowOffset", Float) = 0
		_RimLightColor("RimLightColor", Color) = (0,0,0,0)
		_RimOffset("RimOffset", Float) = 0
		_RimPower("RimPower", Range( 0 , 4)) = 0
		_SpecularIntensity("SpecularIntensity", Range( 0 , 1)) = 0.2204734
		_EmissionColor("EmissionColor", Color) = (0.5235849,1,0.9553949,0)
		_SpecularGloss("SpecularGloss", Range( 0 , 1)) = 0.81
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
		uniform float2 _FaceOffset1;
		uniform float4 _EmissionColor;
		uniform float _EmissionStrength;
		uniform sampler2D _wave;
		uniform sampler2D _FaceTexture;
		uniform float _MetalLightness;
		uniform sampler2D _ToonRamp;
		uniform sampler2D _FaceMask;
		uniform float _ShadowScale;
		uniform float _ShadowOffset;
		uniform float _RimOffset;
		uniform float _RimPower;
		uniform float4 _RimLightColor;
		uniform float _SpecularGloss;
		uniform float _SpecularIntensity;


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
			float2 uv_TexCoord367 = i.uv_texcoord * float2( 3.86,1.43 ) + ( float2( -2.38,-0.26 ) + _FaceOffset1 );
			float2 FaceUV368 = uv_TexCoord367;
			float4 tex2DNode229 = tex2D( _EyeMask, FaceUV368 );
			float temp_output_228_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			Gradient gradient328 = NewGradient( 0, 3, 2, float4( 1, 1, 1, 0.1705959 ), float4( 0.08078849, 0.08078849, 0.08078849, 0.9058824 ), float4( 0.05660379, 0.05660379, 0.05660379, 0.997055 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			Gradient gradient335 = NewGradient( 0, 2, 2, float4( 0.2358491, 0.2358491, 0.2358491, 0 ), float4( 0.7264151, 0.7264151, 0.7264151, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 lerpResult343 = lerp( ( ( clampResult324 - saturate( ( ( 1.0 - ( tex2DNode229.b * temp_output_228_0 ) ) * tex2D( _FaceTexture, FaceUV368 ) ) ).a ) * SampleGradient( gradient328, i.uv_texcoord.y ) * _MetalLightness ) , SampleGradient( gradient335, i.uv_texcoord.y ) , float4( 0.8301887,0.8301887,0.8301887,0 ));
			float4 break326 = saturate( ( ( 1.0 - ( tex2DNode229.b * temp_output_228_0 ) ) * tex2D( _FaceTexture, FaceUV368 ) ) );
			float4 appendResult344 = (float4(break326.r , break326.g , break326.b , break326.a));
			float4 lerpResult347 = lerp( lerpResult343 , appendResult344 , break326.a);
			float4 Albedo359 = saturate( lerpResult347 );
			float2 temp_output_2_0_g2 = FaceUV368;
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
			float2 temp_cast_2 = ((NormalLightDir7*_ShadowScale + _ShadowOffset)).xx;
			float4 Shadow17 = ( Albedo359 * tex2D( _ToonRamp, temp_cast_2 ) );
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
			float4 FaceMask254 = tex2D( _FaceMask, FaceUV368 );
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
			float2 uv_TexCoord367 = i.uv_texcoord * float2( 3.86,1.43 ) + ( float2( -2.38,-0.26 ) + _FaceOffset1 );
			float2 FaceUV368 = uv_TexCoord367;
			float4 tex2DNode229 = tex2D( _EyeMask, FaceUV368 );
			float temp_output_228_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float4 Emission357 = ( ( tex2DNode229 * ( 1.0 - temp_output_228_0 ) ) * _EmissionColor * _EmissionStrength );
			o.Emission = Emission357.rgb;
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
288.8;73.6;1246;710;5238.9;4016.466;5.540428;True;False
Node;AmplifyShaderEditor.CommentaryNode;362;-3125.102,-3466.564;Inherit;False;896.6765;579.775;Adjusted UV mapping to be able to control face posiiton;6;368;367;366;365;364;363;Face UVs;0.4575472,0.5343004,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;364;-3048.906,-3172.134;Inherit;False;Constant;_BaseFaceOffset1;BaseFaceOffset;18;0;Create;True;0;0;False;0;-2.38,-0.26;-2.38,-0.26;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;363;-3035.594,-3025.451;Inherit;False;Property;_FaceOffset1;FaceOffset;2;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;224;-3262.507,-2105.521;Inherit;False;1860.597;582.574;;13;235;232;234;231;230;229;228;227;226;225;242;369;370;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.Vector2Node;366;-3047.92,-3363.134;Inherit;False;Constant;_BaseFaceTilling1;BaseFaceTilling;19;0;Create;True;0;0;False;0;3.86,1.43;3.86,1.43;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;365;-2823.594,-3081.451;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;225;-3228.705,-1749.988;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;367;-2685.375,-3369.497;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;368;-2430.105,-3374.695;Inherit;False;FaceUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;226;-3046.014,-1749.463;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;369;-3113.861,-1978.371;Inherit;False;368;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SinOpNode;227;-2900.848,-1749.184;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCCompareGreater;228;-2756.51,-1780.907;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.96;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;229;-2816.275,-2016.655;Inherit;True;Property;_EyeMask;EyeMask;5;0;Create;True;0;0;False;0;-1;ce02c6bba51f71349bd1782d79c9bf23;a8f49a97d39b7b242baffb284e632f4e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;230;-2425.8,-2010.951;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;370;-2411.135,-1746.433;Inherit;False;368;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;231;-2215.146,-1967.082;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;232;-2223.289,-1794.7;Inherit;True;Property;_FaceTexture;FaceTexture;3;0;Create;True;0;0;False;0;-1;45590d454fb37c444866c18ea64776b7;a3b7ef34dcb18ac449363241fa2e114c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;234;-1982.609,-1935.077;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;307;-1434.357,-3680.911;Inherit;False;1993.255;838.2368;;15;317;316;314;312;311;310;308;325;324;323;321;320;319;318;309;MetalGoop;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;235;-1825.806,-1950.322;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;311;-1368.864,-3576.404;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;308;-1323.906,-3442.976;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;242;-1603.424,-1950.405;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleTimeNode;309;-1336.59,-3174.94;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;310;-1375.548,-3312.367;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;4,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;312;-1097.605,-3513.276;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RelayNode;313;-1305.687,-1949.373;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;314;-1136.724,-3293.574;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;316;-912.4937,-3301.318;Inherit;True;Property;_wave;wave;6;0;Create;True;0;0;False;0;-1;6aa1ee79ccbab5241830162561a5eb5b;6aa1ee79ccbab5241830162561a5eb5b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;315;-1156.74,-2742.749;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;317;-892.4808,-3566.755;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;318;-260.8786,-3127.395;Inherit;True;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TFHCRemapNode;319;-514.0607,-3383.18;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0.5377358;False;2;FLOAT;0.5849056;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;36;-1232.458,-1400.256;Inherit;False;1118.405;568.2388;;6;371;254;255;34;250;249;NormalMap;0.9137255,0.8822335,0.4117648,1;0;0
Node;AmplifyShaderEditor.WireNode;320;153.3891,-3035.104;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;321;-234.6985,-3378.844;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;249;-1174.181,-1334.682;Inherit;True;Property;_FaceMask;FaceMask;4;0;Create;True;0;0;False;0;0c1158d9d4389144eb45ff513591a518;b75eb0031eaad3a4791653eb28b13d60;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;371;-1133.41,-1021.603;Inherit;False;368;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;250;-743.9057,-1327.231;Inherit;True;NormalCreate;0;;2;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.5;False;4;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;322;-1080.844,-2699.66;Inherit;False;1822.114;946.8055;;19;359;328;347;344;343;342;340;339;338;337;336;335;333;332;331;330;327;326;361;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.ClampOpNode;324;-47.20383,-3379.119;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;323;181.4301,-3062.766;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-450.5294,-1332.63;Inherit;False;Normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BreakToComponentsNode;326;-750.1736,-1954.364;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleSubtractOpNode;325;239.0169,-3378.141;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;10;-2592.468,204.7446;Inherit;False;882.4052;396.7347;;5;8;6;4;5;37;Normal.ViewDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;9;-2595.714,-331.8191;Inherit;False;875.0286;393.7782;;5;7;2;1;3;35;Normal.LightDir;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;330;-1036.859,-2546.288;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;349;297.4484,-2786.886;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;327;-345.0295,-1854.348;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-2549.333,271.2427;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GradientNode;328;-1034.347,-2639.636;Inherit;False;0;3;2;1,1,1,0.1705959;0.08078849,0.08078849,0.08078849,0.9058824;0.05660379,0.05660379,0.05660379,0.997055;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.RangedFloatNode;333;-1021.559,-2379.385;Inherit;False;Property;_MetalLightness;MetalLightness;9;0;Create;True;0;0;False;0;1.85;4.68;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;331;-759.7786,-2149.718;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;332;-413.8166,-2560.767;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;336;-322.7238,-1870.412;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;335;-757.5796,-2249.454;Inherit;False;0;2;2;0.2358491,0.2358491,0.2358491,0;0.7264151,0.7264151,0.7264151,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.GradientSampleNode;337;-758.9607,-2642.099;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;35;-2577.912,-241.5608;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;5;-2313.268,432.4253;Inherit;False;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;4;-2317.79,260.1758;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;1;-2353.42,-281.8189;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;338;-393.9926,-2524.124;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;340;-497.6176,-2240.296;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;339;-320.4189,-1940.926;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;6;-2121.85,352.4358;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-2366.943,-128.9543;Inherit;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;2;-2115.975,-232.5734;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;343;-143.8348,-2260.852;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.8301887,0.8301887,0.8301887,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;102;-2582.645,869.0961;Inherit;False;1952.439;560.0481;;16;81;82;83;84;85;87;92;93;94;86;95;98;96;90;91;88;RimLight;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;8;-1950.428,353.7982;Inherit;False;NormalViewDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;342;-307.4376,-1962.447;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;344;-453.5595,-2020.052;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2529.031,1032.633;Inherit;False;8;NormalViewDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;82;-2527.097,931.985;Inherit;False;Property;_RimOffset;RimOffset;12;0;Create;True;0;0;False;0;0;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;347;195.5395,-2044.597;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-1947.886,-235.9804;Inherit;False;NormalLightDir;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;22;-1450.12,-371.0854;Inherit;False;1352.696;462.6083;;8;17;46;13;19;20;21;12;45;Shadow;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-2311.535,937.0261;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;361;444.7071,-2151.888;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-1395.034,-126.5163;Inherit;False;Property;_ShadowScale;ShadowScale;8;0;Create;True;0;0;False;0;0;1.04;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-1393.034,-30.51545;Inherit;False;Property;_ShadowOffset;ShadowOffset;10;0;Create;True;0;0;False;0;0;0.48;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1394.012,-214.2923;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;136;-2582.671,1647.078;Inherit;False;2112.512;733.7065;;16;130;126;129;127;132;123;121;122;119;117;115;114;120;116;137;256;Specularity;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;359;558.0958,-1951.674;Inherit;False;Albedo;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;120;-2541.674,1999.924;Inherit;False;34;Normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;19;-1152.701,-123.1691;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;84;-2167.535,937.0261;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;114;-2544.578,1698.471;Inherit;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightPos;116;-2546.674,1876.924;Inherit;False;0;3;FLOAT4;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;87;-2188.576,1061.795;Inherit;False;Property;_RimPower;RimPower;13;0;Create;True;0;0;False;0;0;0;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-925.0266,-167.1583;Inherit;True;Property;_ToonRamp;ToonRamp;7;0;Create;True;0;0;False;0;-1;None;a01b538848797dc4ab5c717ee3b99fc0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;93;-1855.268,1312.04;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;92;-1856.649,1222.374;Inherit;False;7;NormalLightDir;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;85;-2016.649,937.372;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;117;-2314.676,2003.924;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;46;-878.0673,-289.2449;Inherit;False;359;Albedo;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;115;-2289.676,1737.924;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-557.4346,-240.0395;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;86;-1817.534,937.0261;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;255;-770.6455,-1086.383;Inherit;True;Property;_TextureSample1;Texture Sample 1;18;0;Create;True;0;0;False;0;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;122;-2187.06,2187.629;Inherit;False;Property;_SpecularGloss;SpecularGloss;16;0;Create;True;0;0;False;0;0.81;0.58;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;-1642.67,1259.84;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;119;-2071.675,1850.924;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;254;-448.3698,-1085.782;Inherit;False;FaceMask;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-311.7156,-244.8047;Inherit;False;Shadow;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PowerNode;121;-1914.43,1850.64;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;56;-1445.273,258.5183;Inherit;False;1230.724;338.2227;;4;50;49;48;47;Lighting;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;137;-1792.595,2149.237;Inherit;False;Constant;_SpecRange;SpecRange;13;0;Create;True;0;0;False;0;1.3,1.3;1.29,1.87;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-1544.105,938.662;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;350;-2496.859,-1400.572;Inherit;False;1071.578;568.9196;;7;357;356;358;355;354;353;351;Emission;0.6917149,1,0.1273585,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;123;-1491.576,1851.079;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;90;-1346.704,1056.795;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;96;-1357.421,1214.344;Inherit;False;Property;_RimLightColor;RimLightColor;11;0;Create;True;0;0;False;0;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;256;-1448.521,2138.242;Inherit;False;254;FaceMask;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;98;-1332.227,936.915;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1274.483,324.4056;Inherit;False;17;Shadow;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightColorNode;47;-1277.181,423.227;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;351;-2425.923,-1277.484;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;358;-2403.99,-1356.729;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-1061.435,938.002;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;132;-1133.303,1853.11;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-1133.436,2179.52;Inherit;False;Property;_SpecularIntensity;SpecularIntensity;14;0;Create;True;0;0;False;0;0.2204734;0.21;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-899.5152,323.5102;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightAttenuation;129;-1147.485,1729.495;Inherit;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;88;-847.2719,933.21;Inherit;False;RimLight;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;-457.5194,319.9062;Inherit;False;Lighting;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;353;-2174.398,-1327.395;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;355;-2170.804,-1100.912;Inherit;False;Property;_EmissionColor;EmissionColor;15;0;Create;True;0;0;False;0;0.5235849,1,0.9553949,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;354;-2171.276,-917.6855;Inherit;False;Property;_EmissionStrength;EmissionStrength;17;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;-878.4742,1831.644;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;453.81,498.123;Inherit;False;50;Lighting;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;99;454.0932,590.9183;Inherit;False;88;RimLight;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-687.66,1827.044;Inherit;False;Specularity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;356;-1892.934,-1243.064;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;131;454.4358,707.3589;Inherit;False;130;Specularity;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;100;655.3284,530.0111;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;357;-1668.863,-1247.305;Inherit;False;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;847.6417,576.3377;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;453.3627,384.5375;Inherit;False;357;Emission;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1046.583,350.0078;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Slimes/SolderSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.03;0.3113208,0.3113208,0.3113208,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;365;0;364;0
WireConnection;365;1;363;0
WireConnection;367;0;366;0
WireConnection;367;1;365;0
WireConnection;368;0;367;0
WireConnection;226;0;225;0
WireConnection;227;0;226;0
WireConnection;228;0;227;0
WireConnection;229;1;369;0
WireConnection;230;0;229;3
WireConnection;230;1;228;0
WireConnection;231;0;230;0
WireConnection;232;1;370;0
WireConnection;234;0;231;0
WireConnection;235;0;234;0
WireConnection;235;1;232;0
WireConnection;242;0;235;0
WireConnection;312;0;311;0
WireConnection;312;1;308;0
WireConnection;313;0;242;0
WireConnection;314;0;310;0
WireConnection;314;1;309;0
WireConnection;316;1;314;0
WireConnection;315;0;313;0
WireConnection;317;0;312;0
WireConnection;318;0;315;0
WireConnection;319;0;316;1
WireConnection;319;1;317;0
WireConnection;319;2;317;0
WireConnection;320;0;318;3
WireConnection;321;0;319;0
WireConnection;250;1;249;0
WireConnection;250;2;371;0
WireConnection;324;0;321;0
WireConnection;323;0;320;0
WireConnection;34;0;250;0
WireConnection;326;0;313;0
WireConnection;325;0;324;0
WireConnection;325;1;323;0
WireConnection;349;0;325;0
WireConnection;327;0;326;3
WireConnection;332;0;349;0
WireConnection;336;0;327;0
WireConnection;337;0;328;0
WireConnection;337;1;330;2
WireConnection;4;0;37;0
WireConnection;1;0;35;0
WireConnection;338;0;332;0
WireConnection;338;1;337;0
WireConnection;338;2;333;0
WireConnection;340;0;335;0
WireConnection;340;1;331;2
WireConnection;339;0;336;0
WireConnection;6;0;4;0
WireConnection;6;1;5;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;343;0;338;0
WireConnection;343;1;340;0
WireConnection;8;0;6;0
WireConnection;342;0;339;0
WireConnection;344;0;326;0
WireConnection;344;1;326;1
WireConnection;344;2;326;2
WireConnection;344;3;326;3
WireConnection;347;0;343;0
WireConnection;347;1;344;0
WireConnection;347;2;342;0
WireConnection;7;0;2;0
WireConnection;83;0;82;0
WireConnection;83;1;81;0
WireConnection;361;0;347;0
WireConnection;359;0;361;0
WireConnection;19;0;12;0
WireConnection;19;1;20;0
WireConnection;19;2;21;0
WireConnection;84;0;83;0
WireConnection;13;1;19;0
WireConnection;85;0;84;0
WireConnection;117;0;120;0
WireConnection;115;0;114;0
WireConnection;115;1;116;1
WireConnection;45;0;46;0
WireConnection;45;1;13;0
WireConnection;86;0;85;0
WireConnection;86;1;87;0
WireConnection;255;0;249;0
WireConnection;255;1;371;0
WireConnection;94;0;92;0
WireConnection;94;1;93;0
WireConnection;119;0;115;0
WireConnection;119;1;117;0
WireConnection;254;0;255;0
WireConnection;17;0;45;0
WireConnection;121;0;119;0
WireConnection;121;1;122;0
WireConnection;95;0;86;0
WireConnection;95;1;94;0
WireConnection;123;0;121;0
WireConnection;123;1;137;1
WireConnection;123;2;137;2
WireConnection;98;0;95;0
WireConnection;351;0;228;0
WireConnection;358;0;229;0
WireConnection;91;0;98;0
WireConnection;91;1;90;0
WireConnection;91;2;96;0
WireConnection;132;0;123;0
WireConnection;132;1;256;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;88;0;91;0
WireConnection;50;0;49;0
WireConnection;353;0;358;0
WireConnection;353;1;351;0
WireConnection;126;0;129;0
WireConnection;126;1;132;0
WireConnection;126;2;127;0
WireConnection;130;0;126;0
WireConnection;356;0;353;0
WireConnection;356;1;355;0
WireConnection;356;2;354;0
WireConnection;100;0;18;0
WireConnection;100;1;99;0
WireConnection;357;0;356;0
WireConnection;134;0;100;0
WireConnection;134;1;131;0
WireConnection;0;2;159;0
WireConnection;0;13;134;0
ASEEND*/
//CHKSM=29C3D14B60DF6D62564F57F162B62272F65D5B21