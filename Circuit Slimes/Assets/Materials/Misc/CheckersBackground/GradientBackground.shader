// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GradientBackground"
{
	Properties
	{
		_GridSize("GridSize", Range( 0 , 0.01)) = 4.94
		_transparency("transparency", Float) = 0.2
		_Speed("Speed", Vector) = (1,1,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float4 screenPos;
		};

		uniform float2 _Speed;
		uniform float _GridSize;
		uniform float _transparency;


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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			Gradient gradient1 = NewGradient( 0, 3, 2, float4( 0.260591, 0.2660971, 0.3018868, 0 ), float4( 0.6365796, 0.6700602, 0.7374583, 0.6441138 ), float4( 0.6433339, 0.677317, 0.745283, 0.9882353 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float4 break45 = ase_screenPosNorm;
			float4 appendResult46 = (float4(break45.y , break45.x , 0.0 , 0.0));
			float4 color7_g1 = IsGammaSpace() ? float4(0.2,0.2,0.2,0) : float4(0.03310476,0.03310476,0.03310476,0);
			float4 color8_g1 = IsGammaSpace() ? float4(0.6980392,0.6980392,0.6980392,0) : float4(0.4452012,0.4452012,0.4452012,0);
			float2 temp_cast_1 = (0.46).xx;
			float2 panner35 = ( _Time.y * _Speed + (( ase_screenPosNorm * _ScreenParams * _GridSize )).xy);
			float2 FinalUV13_g1 = ( temp_cast_1 * ( 0.5 + panner35 ) );
			float2 temp_cast_2 = (0.5).xx;
			float2 temp_cast_3 = (1.0).xx;
			float4 appendResult16_g1 = (float4(ddx( FinalUV13_g1 ) , ddy( FinalUV13_g1 )));
			float4 UVDerivatives17_g1 = appendResult16_g1;
			float4 break28_g1 = UVDerivatives17_g1;
			float2 appendResult19_g1 = (float2(break28_g1.x , break28_g1.z));
			float2 appendResult20_g1 = (float2(break28_g1.x , break28_g1.z));
			float dotResult24_g1 = dot( appendResult19_g1 , appendResult20_g1 );
			float2 appendResult21_g1 = (float2(break28_g1.y , break28_g1.w));
			float2 appendResult22_g1 = (float2(break28_g1.y , break28_g1.w));
			float dotResult23_g1 = dot( appendResult21_g1 , appendResult22_g1 );
			float2 appendResult25_g1 = (float2(dotResult24_g1 , dotResult23_g1));
			float2 derivativesLength29_g1 = sqrt( appendResult25_g1 );
			float2 temp_cast_4 = (-1.0).xx;
			float2 temp_cast_5 = (1.0).xx;
			float2 clampResult57_g1 = clamp( ( ( ( abs( ( frac( ( FinalUV13_g1 + 0.25 ) ) - temp_cast_2 ) ) * 4.0 ) - temp_cast_3 ) * ( 0.35 / derivativesLength29_g1 ) ) , temp_cast_4 , temp_cast_5 );
			float2 break71_g1 = clampResult57_g1;
			float2 break55_g1 = derivativesLength29_g1;
			float4 lerpResult73_g1 = lerp( color7_g1 , color8_g1 , saturate( ( 0.5 + ( 0.5 * break71_g1.x * break71_g1.y * sqrt( saturate( ( 1.1 - max( break55_g1.x , break55_g1.y ) ) ) ) ) ) ));
			o.Emission = ( SampleGradient( gradient1, appendResult46.x ) + ( lerpResult73_g1 * _transparency ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
0;161.6;1523;640;11686.21;4175.684;3.882509;True;False
Node;AmplifyShaderEditor.ScreenPosInputsNode;6;-8572.091,-2994.83;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-8584.277,-2629.202;Inherit;False;Property;_GridSize;GridSize;0;0;Create;True;0;0;False;0;4.94;0.01;0;0.01;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenParams;20;-8577.479,-2823.591;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-8324.48,-2945.591;Inherit;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;42;-7985.779,-3234.189;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;41;-7918.745,-2780.427;Inherit;False;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;36;-7921.937,-2651.784;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;48;-8027.603,-2925.362;Inherit;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;35;-7692.205,-2759.948;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-7458.505,-2690.92;Inherit;False;Constant;_frequency;frequency;1;0;Create;True;0;0;False;0;0.46;0.46;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;45;-7766.966,-3216.934;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;17;-7224.895,-2489.896;Inherit;False;Property;_transparency;transparency;1;0;Create;True;0;0;False;0;0.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;1;-7580.752,-3415.761;Inherit;False;0;3;2;0.260591,0.2660971,0.3018868,0;0.6365796,0.6700602,0.7374583,0.6441138;0.6433339,0.677317,0.745283,0.9882353;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.DynamicAppendNode;46;-7505.624,-3214.128;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;14;-7288.758,-2757.019;Inherit;True;Checkerboard;-1;;1;43dad715d66e03a4c8ad5f9564018081;0;4;1;FLOAT2;0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT2;0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-6937.252,-2663.751;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;2;-7272.822,-3320.855;Inherit;True;2;0;OBJECT;;False;1;FLOAT;-2;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-6722.604,-3039.171;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-6369.478,-3115.495;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;GradientBackground;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;21;0;6;0
WireConnection;21;1;20;0
WireConnection;21;2;18;0
WireConnection;48;0;21;0
WireConnection;35;0;48;0
WireConnection;35;2;41;0
WireConnection;35;1;36;0
WireConnection;45;0;42;0
WireConnection;46;0;45;1
WireConnection;46;1;45;0
WireConnection;14;1;35;0
WireConnection;14;4;23;0
WireConnection;38;0;14;0
WireConnection;38;1;17;0
WireConnection;2;0;1;0
WireConnection;2;1;46;0
WireConnection;39;0;2;0
WireConnection;39;1;38;0
WireConnection;0;2;39;0
ASEEND*/
//CHKSM=2B1D7CC6B0AE0AA12A2CC6E379773C69708FDA7E