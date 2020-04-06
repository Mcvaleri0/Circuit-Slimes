// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/oldWaterSlime"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0,0.498651,1,1)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.03
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_FresnelScale("FresnelScale", Range( 0 , 10)) = 0.5
		_FresnelPower("FresnelPower", Range( 0 , 10)) = 0.5
		_EyeMask("EyeMask", 2D) = "white" {}
		_FresnelColor("FresnelColor", Color) = (0,0.9999995,1,0)
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_FaceMask("FaceMask", 2D) = "white" {}
		_CausticsTexture("CausticsTexture", 2D) = "white" {}
		_WaterIntensity("WaterIntensity", Range( 0 , 1)) = 1
		_WaterSpeed("WaterSpeed", Range( 0 , 1)) = 0.1294118
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

		uniform sampler2D _FaceMask;
		uniform float4 _FaceMask_ST;
		uniform sampler2D _CausticsTexture;
		uniform float _WaterSpeed;
		uniform float _WaterIntensity;
		uniform sampler2D _EyeMask;
		uniform float4 _EyeMask_ST;
		uniform sampler2D _FaceTexture;
		uniform float4 _FaceTexture_ST;
		uniform float4 _FresnelColor;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _Smoothness;


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
			float2 temp_cast_0 = (_WaterSpeed).xx;
			float2 panner287 = ( _Time.y * temp_cast_0 + i.uv_texcoord);
			float4 tex2DNode293 = tex2D( _CausticsTexture, panner287 );
			float3 temp_cast_1 = (tex2DNode293.a).xxx;
			float4 temp_cast_3 = (saturate( ( 1.0 - ( ( distance( temp_cast_1 , tex2DNode293.rgb ) - -2.33 ) / max( 4.32 , 1E-05 ) ) ) )).xxxx;
			float div295=256.0/float(28);
			float4 posterize295 = ( floor( temp_cast_3 * div295 ) / div295 );
			float2 uv_EyeMask = i.uv_texcoord * _EyeMask_ST.xy + _EyeMask_ST.zw;
			float4 tex2DNode377 = tex2D( _EyeMask, uv_EyeMask );
			float temp_output_360_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float2 uv_FaceTexture = i.uv_texcoord * _FaceTexture_ST.xy + _FaceTexture_ST.zw;
			float4 temp_output_378_0 = ( ( 1.0 - ( tex2DNode377.b * temp_output_360_0 ) ) * tex2D( _FaceTexture, uv_FaceTexture ) );
			Gradient gradient23 = NewGradient( 0, 2, 2, float4( 0, 0.08320529, 0.7924528, 0 ), float4( 0, 0.8330406, 0.8911465, 1 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 break195 = temp_output_378_0;
			float4 appendResult201 = (float4(break195.r , break195.g , break195.b , break195.a));
			float4 lerpResult35 = lerp( SampleGradient( gradient23, i.uv_texcoord.y ) , appendResult201 , break195.a);
			o.Albedo = ( ( posterize295 * _WaterIntensity * ( 1.0 - temp_output_378_0.a ) ) + lerpResult35 ).rgb;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV18 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode18 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV18, _FresnelPower ) );
			o.Emission = ( ( ( tex2DNode377 * ( 1.0 - temp_output_360_0 ) ) * _FresnelColor ) + ( fresnelNode18 * _FresnelColor ) ).rgb;
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
288.8;73.6;1246;710;3122.314;724.3976;1.164122;True;False
Node;AmplifyShaderEditor.CommentaryNode;376;-4921.287,-181.1784;Inherit;False;1697.031;819.0219;;12;369;333;397;395;378;379;82;382;381;377;360;367;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;333;-4886.35,359.9868;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;369;-4703.659,358.8525;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;367;-4558.493,359.1316;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;377;-4434.095,96.64022;Inherit;True;Property;_EyeMask;EyeMask;5;0;Create;True;0;0;False;0;-1;ce02c6bba51f71349bd1782d79c9bf23;ce02c6bba51f71349bd1782d79c9bf23;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;360;-4375.989,322.4297;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.96;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;381;-4109.995,104.0037;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;382;-3899.342,147.8732;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;82;-3907.485,320.253;Inherit;True;Property;_FaceTexture;FaceTexture;7;0;Create;True;0;0;False;0;-1;45590d454fb37c444866c18ea64776b7;45590d454fb37c444866c18ea64776b7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;395;-4099.677,511.5815;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;379;-3666.805,179.878;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;378;-3510.003,164.6334;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;397;-4050.366,519.7999;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;240;-2977.449,-95.88268;Inherit;False;1731.597;741.7811;;19;104;58;45;214;18;212;211;213;19;20;215;210;48;385;386;400;401;396;402;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.RelayNode;322;-3206.963,-1003.47;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;298;-2927.902,-2283.848;Inherit;False;1720.987;685.7514;;11;296;297;294;295;293;287;288;299;292;323;327;Water;0,1,0.9590418,1;0;0
Node;AmplifyShaderEditor.WireNode;396;-2906.358,521.6454;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;244;-2831.843,-1426.131;Inherit;False;1239.633;657.631;;10;35;26;316;201;202;28;23;203;248;195;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;299;-2895.22,-1944.548;Inherit;False;Property;_WaterSpeed;WaterSpeed;11;0;Create;True;0;0;False;0;0.1294118;0.15;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;195;-2683.226,-1008.99;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleTimeNode;292;-2882.008,-2064.532;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;288;-2888.103,-2193.457;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;401;-2884.527,502.3481;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;48;-2257.717,452.1035;Inherit;False;Property;_FresnelColor;FresnelColor;6;0;Create;True;0;0;False;0;0,0.9999995,1,0;0.2235293,0.8963526,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;248;-2278.082,-908.9745;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;402;-2883.229,229.8216;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;324;-3072.405,-1777.757;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;287;-2651.813,-2194.399;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.38,0.32;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WireNode;203;-2255.776,-925.0385;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;210;-1943.362,481.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;325;-3041.492,-1794.099;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;400;-2884.154,212.4765;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;215;-2003.606,472.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;293;-2461.966,-2223.309;Inherit;True;Property;_CausticsTexture;CausticsTexture;9;0;Create;True;0;0;False;0;-1;d85b3c19dd309a84d9f47f9de16cbae6;d85b3c19dd309a84d9f47f9de16cbae6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;386;-2809.774,173.5846;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-2692.831,-1204.344;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;20;-2594.719,292.7679;Inherit;False;Property;_FresnelPower;FresnelPower;4;0;Create;True;0;0;False;0;0.5;4.6;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;294;-2092.542,-2218.979;Inherit;True;Color Mask;-1;;1;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;-2.33;False;5;FLOAT;4.32;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;323;-2205.779,-1828.91;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WireNode;202;-2253.471,-995.5525;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;23;-2687.63,-1376.131;Inherit;False;0;2;2;0,0.08320529,0.7924528,0;0,0.8330406,0.8911465,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.WireNode;211;-1997.606,446.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;239;-2701.896,766.0574;Inherit;False;971.5482;440.1462;;4;60;13;84;340;Smoothness;0.1367925,0.8419735,1,1;0;0
Node;AmplifyShaderEditor.WireNode;213;-1935.362,458.6273;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2594.803,204.268;Inherit;False;Property;_FresnelScale;FresnelScale;3;0;Create;True;0;0;False;0;0.5;1.2;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;26;-2430.67,-1294.922;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;201;-2386.612,-1074.678;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;327;-1680.539,-1832.944;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;297;-1828.182,-1964.336;Inherit;False;Property;_WaterIntensity;WaterIntensity;10;0;Create;True;0;0;False;0;1;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;340;-2641.794,851.8696;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.WireNode;316;-2240.49,-1017.073;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;18;-2309.294,204.7357;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;-2456.058,-37.57633;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;243;-2733.218,-513.0183;Inherit;False;763.95;291.7495;;2;161;392;Normals;0.7745169,0.487184,0.8679245,1;0;0
Node;AmplifyShaderEditor.WireNode;212;-1984.873,41.47329;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;214;-1932.873,339.4728;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PosterizeNode;295;-1788.497,-2217.635;Inherit;True;28;2;1;COLOR;0,0,0,0;False;0;INT;28;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;35;-2070.049,-1102.652;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;161;-2661.294,-470.3398;Inherit;True;Property;_FaceMask;FaceMask;8;0;Create;True;0;0;False;0;0c1158d9d4389144eb45ff513591a518;0c1158d9d4389144eb45ff513591a518;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2301.259,1070.956;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;False;0;0.5;0.696;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;84;-2268.662,822.9257;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1912.497,-37.21352;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1888.874,265.4731;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;296;-1478.258,-2208.916;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-1549.726,110.2616;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;300;-1126.134,-1129.793;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1963.726,822.4874;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;392;-2298.627,-464.7373;Inherit;True;NormalCreate;0;;2;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.3;False;4;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;411;-799.8347,-25.2072;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Slimes/oldWaterSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.03;0,0.498651,1,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;369;0;333;0
WireConnection;367;0;369;0
WireConnection;360;0;367;0
WireConnection;381;0;377;3
WireConnection;381;1;360;0
WireConnection;382;0;381;0
WireConnection;395;0;360;0
WireConnection;379;0;382;0
WireConnection;378;0;379;0
WireConnection;378;1;82;0
WireConnection;397;0;395;0
WireConnection;322;0;378;0
WireConnection;396;0;397;0
WireConnection;195;0;322;0
WireConnection;401;0;396;0
WireConnection;248;0;195;3
WireConnection;402;0;401;0
WireConnection;324;0;322;0
WireConnection;287;0;288;0
WireConnection;287;2;299;0
WireConnection;287;1;292;0
WireConnection;203;0;248;0
WireConnection;210;0;48;0
WireConnection;325;0;324;0
WireConnection;400;0;402;0
WireConnection;215;0;48;0
WireConnection;293;1;287;0
WireConnection;386;0;400;0
WireConnection;294;1;293;0
WireConnection;294;3;293;4
WireConnection;323;0;325;0
WireConnection;202;0;203;0
WireConnection;211;0;215;0
WireConnection;213;0;210;0
WireConnection;26;0;23;0
WireConnection;26;1;28;2
WireConnection;201;0;195;0
WireConnection;201;1;195;1
WireConnection;201;2;195;2
WireConnection;201;3;195;3
WireConnection;327;0;323;3
WireConnection;340;0;378;0
WireConnection;316;0;202;0
WireConnection;18;2;19;0
WireConnection;18;3;20;0
WireConnection;385;0;377;0
WireConnection;385;1;386;0
WireConnection;212;0;211;0
WireConnection;214;0;213;0
WireConnection;295;1;294;0
WireConnection;35;0;26;0
WireConnection;35;1;201;0
WireConnection;35;2;316;0
WireConnection;84;0;340;3
WireConnection;58;0;385;0
WireConnection;58;1;212;0
WireConnection;45;0;18;0
WireConnection;45;1;214;0
WireConnection;296;0;295;0
WireConnection;296;1;297;0
WireConnection;296;2;327;0
WireConnection;104;0;58;0
WireConnection;104;1;45;0
WireConnection;300;0;296;0
WireConnection;300;1;35;0
WireConnection;60;0;84;0
WireConnection;60;1;13;0
WireConnection;392;1;161;0
WireConnection;411;0;300;0
WireConnection;411;1;392;0
WireConnection;411;2;104;0
WireConnection;411;4;60;0
ASEEND*/
//CHKSM=8D149FFEFE9FB9DD053801D6C220B0C05EABC1B3