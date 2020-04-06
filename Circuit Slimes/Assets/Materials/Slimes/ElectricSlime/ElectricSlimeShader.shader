// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Slimes/ElectricSlime"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0.8962264,0.5688522,0,1)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.03
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.5
		_FresnelScale("FresnelScale", Range( 0 , 10)) = 0.5
		_FresnelPower("FresnelPower", Range( 0 , 10)) = 0.5
		_EyeMask("EyeMask", 2D) = "white" {}
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_FaceMask("FaceMask", 2D) = "white" {}
		_EmissionColor("EmissionColor", Color) = (0.3396226,0.3396226,0.3396226,0)
		_TextureSample3("Texture Sample 3", 2D) = "white" {}
		_TextureSample4("Texture Sample 4", 2D) = "white" {}
		_LightPower("LightPower", Range( 0 , 1)) = 0.3118221
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
		uniform half4 _ASEOutlineColor;
		uniform half _ASEOutlineWidth;
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
		uniform sampler2D _EyeMask;
		uniform float4 _EyeMask_ST;
		uniform sampler2D _FaceTexture;
		uniform float4 _FaceTexture_ST;
		uniform float4 _EmissionColor;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform sampler2D _TextureSample3;
		uniform sampler2D _TextureSample4;
		uniform float _LightPower;
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
			Gradient gradient23 = NewGradient( 0, 3, 2, float4( 0.8584906, 0.3748198, 0, 0 ), float4( 1, 0.6719017, 0, 0.4470588 ), float4( 1, 0.9120437, 0, 1 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 uv_EyeMask = i.uv_texcoord * _EyeMask_ST.xy + _EyeMask_ST.zw;
			float4 tex2DNode377 = tex2D( _EyeMask, uv_EyeMask );
			float temp_output_360_0 = (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 );
			float2 uv_FaceTexture = i.uv_texcoord * _FaceTexture_ST.xy + _FaceTexture_ST.zw;
			float4 temp_output_378_0 = ( ( 1.0 - ( tex2DNode377.b * temp_output_360_0 ) ) * tex2D( _FaceTexture, uv_FaceTexture ) );
			float4 break195 = temp_output_378_0;
			float4 appendResult201 = (float4(break195.r , break195.g , break195.b , break195.a));
			float4 lerpResult35 = lerp( SampleGradient( gradient23, i.uv_texcoord.y ) , appendResult201 , break195.a);
			o.Albedo = lerpResult35.xyz;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float fresnelNdotV18 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode18 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV18, _FresnelPower ) );
			float4 appendResult508 = (float4(i.uv_texcoord.y , i.uv_texcoord.x , 0.0 , 0.0));
			float2 panner484 = ( 1.0 * _Time.y * float2( -0.8,0 ) + appendResult508.xy);
			float2 break497 = panner484;
			float4 appendResult500 = (float4(break497.x , (break497.y*-0.26 + 0.51) , 0.0 , 0.0));
			float2 break513 = panner484;
			float4 appendResult515 = (float4(break513.x , (break513.y*-0.26 + -0.22) , 0.0 , 0.0));
			float div509=256.0/float(113);
			float4 posterize509 = ( floor( ( tex2D( _TextureSample3, appendResult500.xy ) + tex2D( _TextureSample4, appendResult515.xy ) ) * div509 ) / div509 );
			float temp_output_60_0 = ( ( 1.0 - temp_output_378_0.a ) * _Smoothness );
			o.Emission = ( ( ( tex2DNode377 * ( 1.0 - temp_output_360_0 ) ) * _EmissionColor ) + ( ( ( fresnelNode18 * posterize509 * _EmissionColor ) + ( _EmissionColor * _LightPower ) ) * temp_output_60_0 ) ).rgb;
			o.Smoothness = temp_output_60_0;
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
Version=17800
0;73.6;1027;710;3008.657;108.5289;1.631922;True;False
Node;AmplifyShaderEditor.CommentaryNode;376;-5075.182,-202.1905;Inherit;False;1690.71;727.3736;;14;397;378;82;395;379;382;381;360;377;367;369;333;412;413;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;333;-5040.244,269.4484;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;369;-4857.553,268.3141;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;367;-4712.387,268.5932;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;517;-3141.328,877.8792;Inherit;False;1807.586;723.3087;;13;486;508;484;497;513;514;503;500;515;483;512;516;509;Lightning;1,0.901886,0,1;0;0
Node;AmplifyShaderEditor.SamplerNode;377;-4650.883,6.101793;Inherit;True;Property;_EyeMask;EyeMask;5;0;Create;True;0;0;False;0;-1;None;ce02c6bba51f71349bd1782d79c9bf23;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;360;-4529.882,231.8913;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.96;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;381;-4263.89,13.46529;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;486;-3091.328,927.8793;Inherit;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;382;-4053.237,57.33489;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;508;-2830.42,1096.718;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;379;-3820.7,89.33975;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;82;-4061.38,229.7146;Inherit;True;Property;_FaceTexture;FaceTexture;6;0;Create;True;0;0;False;0;-1;None;64c1e1a0b32d115469a69023fe41d68e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;484;-2727.5,927.9933;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.8,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;378;-3663.898,74.09512;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;497;-2538.637,930.7823;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;513;-2562.863,1307.105;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.ScaleAndOffsetNode;503;-2426.473,1076.785;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-0.26;False;2;FLOAT;0.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;514;-2400.949,1443.989;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-0.26;False;2;FLOAT;-0.22;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;428;-3453.68,1918.964;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;395;-4253.572,421.0432;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;239;-2732.016,1826.609;Inherit;False;971.5482;440.1462;;4;60;13;84;340;Smoothness;0.1367925,0.8419735,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;515;-2255.904,1310.098;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;511;-3429.056,1944.274;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;500;-2285.861,929.6593;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;397;-4204.261,429.2615;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;240;-3095.868,-188.9359;Inherit;False;2011.52;933.6337;;24;528;527;529;519;487;469;524;104;526;58;385;18;471;386;19;400;402;401;396;531;533;534;535;537;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;483;-2125.242,936.8854;Inherit;True;Property;_TextureSample3;Texture Sample 3;9;0;Create;True;0;0;False;0;-1;None;01244fb4003e60142a0571daf8673fa2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;512;-2129.503,1244.499;Inherit;True;Property;_TextureSample4;Texture Sample 4;10;0;Create;True;0;0;False;0;-1;None;01244fb4003e60142a0571daf8673fa2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;244;-2987.06,-1317.087;Inherit;False;1337.732;657.631;;10;35;201;26;316;28;202;23;203;248;195;Albedo;1,0.6153061,0,1;0;0
Node;AmplifyShaderEditor.RelayNode;322;-3441.71,-904.875;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BreakToComponentsNode;340;-2671.914,1912.422;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;195;-2740.344,-899.9461;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.OneMinusNode;84;-2298.781,1883.478;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-2331.379,2131.507;Inherit;False;Property;_Smoothness;Smoothness;2;0;Create;True;0;0;False;0;0.5;0.696;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;516;-1810.235,1110.25;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;396;-3022.042,434.0615;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-1993.847,1883.039;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;248;-2335.2,-799.9306;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;401;-3000.211,414.7641;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;509;-1577.424,939.3737;Inherit;True;113;2;1;COLOR;0,0,0,0;False;0;INT;113;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;402;-2999.755,15.62179;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;520;-1331.783,816.3735;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;471;-2924.691,313.5222;Inherit;False;Property;_FresnelPower;FresnelPower;4;0;Create;True;0;0;False;0;0.5;2.601973;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;532;-1181.228,1593.237;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;203;-2312.894,-815.9946;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-2933.974,112.2513;Inherit;False;Property;_FresnelScale;FresnelScale;3;0;Create;True;0;0;False;0;0.5;2.698469;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;537;-2334.672,607.8852;Inherit;False;Property;_LightPower;LightPower;11;0;Create;True;0;0;False;0;0.3118221;0.3118221;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;469;-2279.119,419.28;Inherit;False;Property;_EmissionColor;EmissionColor;8;0;Create;True;0;0;False;0;0.3396226,0.3396226,0.3396226,0;1,0.7870027,0.0518868,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;533;-1174.874,633.6372;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;18;-2379.199,172.6426;Inherit;True;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-2749.949,-1095.3;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;202;-2310.589,-886.5086;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;412;-4310.317,-98.63137;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;243;-2698.413,-556.916;Inherit;False;763.95;291.7495;;2;161;392;Normals;0.7745169,0.487184,0.8679245,1;0;0
Node;AmplifyShaderEditor.GradientNode;23;-2758.228,-1233.903;Inherit;False;0;3;2;0.8584906,0.3748198,0,0;1,0.6719017,0,0.4470588;1,0.9120437,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.WireNode;400;-3000.68,-1.723315;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;529;-2019.189,584.9402;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.06603771;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;519;-2025.324,384.4705;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;386;-2926.301,-40.61529;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;161;-2626.489,-514.2375;Inherit;True;Property;_FaceMask;FaceMask;7;0;Create;True;0;0;False;0;None;0c1158d9d4389144eb45ff513591a518;False;white;Auto;Texture2D;-1;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.WireNode;316;-2297.608,-908.0291;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;201;-2443.73,-965.634;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;534;-1492.622,551.0227;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;528;-1782.194,574.7371;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;487;-1979.88,174.1063;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;413;-4275.551,-104.9519;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;26;-2487.788,-1185.878;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;35;-2127.167,-993.6078;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;527;-1718.556,277.201;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;392;-2263.822,-508.635;Inherit;True;NormalCreate;0;;2;e12f7ae19d416b942820e3932b56220f;0;4;1;SAMPLER2D;;False;2;FLOAT2;0,0;False;3;FLOAT;0.3;False;4;FLOAT;0.5;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;535;-1522.19,395.7754;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;385;-2574.478,-130.6296;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;531;-1473.888,280.0921;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;523;-1538.775,-431.8577;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-2030.917,-130.2668;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;522;-894.6204,1907.117;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;525;-1494.238,-884.0727;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;526;-1131.096,-130.3809;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;104;-1324.162,17.32075;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;524;-1172.208,-58.43774;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;518;-863.62,1880.375;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;411;-799.8347,-25.2072;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;Slimes/ElectricSlime;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.03;0.8962264,0.5688522,0,1;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;369;0;333;0
WireConnection;367;0;369;0
WireConnection;360;0;367;0
WireConnection;381;0;377;3
WireConnection;381;1;360;0
WireConnection;382;0;381;0
WireConnection;508;0;486;2
WireConnection;508;1;486;1
WireConnection;379;0;382;0
WireConnection;484;0;508;0
WireConnection;378;0;379;0
WireConnection;378;1;82;0
WireConnection;497;0;484;0
WireConnection;513;0;484;0
WireConnection;503;0;497;1
WireConnection;514;0;513;1
WireConnection;428;0;378;0
WireConnection;395;0;360;0
WireConnection;515;0;513;0
WireConnection;515;1;514;0
WireConnection;511;0;428;0
WireConnection;500;0;497;0
WireConnection;500;1;503;0
WireConnection;397;0;395;0
WireConnection;483;1;500;0
WireConnection;512;1;515;0
WireConnection;322;0;378;0
WireConnection;340;0;511;0
WireConnection;195;0;322;0
WireConnection;84;0;340;3
WireConnection;516;0;483;0
WireConnection;516;1;512;0
WireConnection;396;0;397;0
WireConnection;60;0;84;0
WireConnection;60;1;13;0
WireConnection;248;0;195;3
WireConnection;401;0;396;0
WireConnection;509;1;516;0
WireConnection;402;0;401;0
WireConnection;520;0;509;0
WireConnection;532;0;60;0
WireConnection;203;0;248;0
WireConnection;533;0;532;0
WireConnection;18;2;19;0
WireConnection;18;3;471;0
WireConnection;202;0;203;0
WireConnection;412;0;377;0
WireConnection;400;0;402;0
WireConnection;529;0;469;0
WireConnection;529;1;537;0
WireConnection;519;0;520;0
WireConnection;386;0;400;0
WireConnection;316;0;202;0
WireConnection;201;0;195;0
WireConnection;201;1;195;1
WireConnection;201;2;195;2
WireConnection;201;3;195;3
WireConnection;534;0;533;0
WireConnection;528;0;529;0
WireConnection;487;0;18;0
WireConnection;487;1;519;0
WireConnection;487;2;469;0
WireConnection;413;0;412;0
WireConnection;26;0;23;0
WireConnection;26;1;28;2
WireConnection;35;0;26;0
WireConnection;35;1;201;0
WireConnection;35;2;316;0
WireConnection;527;0;487;0
WireConnection;527;1;528;0
WireConnection;392;1;161;0
WireConnection;535;0;534;0
WireConnection;385;0;413;0
WireConnection;385;1;386;0
WireConnection;531;0;527;0
WireConnection;531;1;535;0
WireConnection;523;0;392;0
WireConnection;58;0;385;0
WireConnection;58;1;469;0
WireConnection;522;0;60;0
WireConnection;525;0;35;0
WireConnection;526;0;525;0
WireConnection;104;0;58;0
WireConnection;104;1;531;0
WireConnection;524;0;523;0
WireConnection;518;0;522;0
WireConnection;411;0;526;0
WireConnection;411;1;524;0
WireConnection;411;2;104;0
WireConnection;411;4;518;0
ASEEND*/
//CHKSM=BAA5E8ADAB592DCE12C4110ECA4E84994E69E86F