// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Misc/MagicAura"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Radius("Radius", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		ZWrite On
		ZTest Less
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Radius;
		uniform float _Cutoff = 0.5;


		float2 voronoihash43( float2 p )
		{
			
			p = float2( dot( p, float2( 127.1, 311.7 ) ), dot( p, float2( 269.5, 183.3 ) ) );
			return frac( sin( p ) *43758.5453);
		}


		float voronoi43( float2 v, float time, inout float2 id, float smoothness )
		{
			float2 n = floor( v );
			float2 f = frac( v );
			float F1 = 8.0;
			float F2 = 8.0; float2 mr = 0; float2 mg = 0;
			for ( int j = -1; j <= 1; j++ )
			{
				for ( int i = -1; i <= 1; i++ )
			 	{
			 		float2 g = float2( i, j );
			 		float2 o = voronoihash43( n + g );
					o = ( sin( time + o * 6.2831 ) * 0.5 + 0.5 ); float2 r = g - f + o;
					float d = 0.5 * dot( r, r );
			 		if( d<F1 ) {
			 			F2 = F1;
			 			F1 = d; mg = g; mr = r; id = o;
			 		} else if( d<F2 ) {
			 			F2 = d;
			 		}
			 	}
			}
			return F1;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = float3( 0, 1, 0 );
			float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
			float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
			float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
			v.normal = normalize( mul( float4( v.normal , 0 ), rotationCamMatrix )).xyz;
			v.vertex.x *= length( unity_ObjectToWorld._m00_m10_m20 );
			v.vertex.y *= length( unity_ObjectToWorld._m01_m11_m21 );
			v.vertex.z *= length( unity_ObjectToWorld._m02_m12_m22 );
			v.vertex = mul( v.vertex, rotationCamMatrix );
			v.vertex.xyz += unity_ObjectToWorld._m03_m13_m23;
			//Need to nullify rotation inserted by generated surface shader;
			v.vertex = mul( unity_WorldToObject, v.vertex );
			float3 ase_vertex3Pos = v.vertex.xyz;
			v.vertex.xyz += ( 0 + ( ase_vertex3Pos * 1 ) );
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float4 color53 = IsGammaSpace() ? float4(0.1745481,1,0,0) : float4(0.02572118,1,0,0);
			float time43 = _Time.y;
			float2 CenteredUV15_g9 = ( i.uv_texcoord - float2( 0.5,0.5 ) );
			float2 break17_g9 = CenteredUV15_g9;
			float2 appendResult23_g9 = (float2(( length( CenteredUV15_g9 ) * 1.0 * 2.0 ) , ( atan2( break17_g9.x , break17_g9.y ) * 1.0 * ( 1.0 / 6.28318548202515 ) )));
			float2 coords43 = appendResult23_g9 * 3.14;
			float2 id43 = 0;
			float voroi43 = voronoi43( coords43, time43,id43, 0 );
			float4 temp_cast_0 = (voroi43).xxxx;
			float div45=256.0/float(47);
			float4 posterize45 = ( floor( temp_cast_0 * div45 ) / div45 );
			float cos38 = cos( _Time.y );
			float sin38 = sin( _Time.y );
			float2 rotator38 = mul( i.uv_texcoord - float2( 0.5,0.5 ) , float2x2( cos38 , -sin38 , sin38 , cos38 )) + float2( 0.5,0.5 );
			float2 appendResult11_g10 = (float2(_Radius , _Radius));
			float temp_output_17_0_g10 = length( ( (rotator38*2.0 + -1.0) / appendResult11_g10 ) );
			float temp_output_47_0 = saturate( ( 1.0 - ( ( distance( float3( 0,0,0 ) , ( posterize45 * saturate( ( ( 1.0 - temp_output_17_0_g10 ) / fwidth( temp_output_17_0_g10 ) ) ) ).rgb ) - 0.0 ) / max( 0.0 , 1E-05 ) ) ) );
			o.Emission = ( color53 + temp_output_47_0 ).rgb;
			o.Alpha = 1;
			clip( saturate( ( 1.0 - temp_output_47_0 ) ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17900
12.8;97.6;1523;704;922.6927;704.2313;2.030716;True;False
Node;AmplifyShaderEditor.SimpleTimeNode;42;-1083.479,-705.7966;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-1071.541,-507.6017;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;44;-1015.827,-998.0574;Inherit;False;Polar Coordinates;-1;;9;7dab8e02884cf104ebefaa2e788e4162;0;4;1;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RotatorNode;38;-800.3317,-502.0988;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-807.1488,-329.0292;Inherit;False;Property;_Radius;Radius;1;0;Create;True;0;0;False;0;0;0.94;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VoronoiNode;43;-784.9294,-994.86;Inherit;True;0;0;1;0;1;False;1;False;False;4;0;FLOAT2;0.45,0;False;1;FLOAT;17.82;False;2;FLOAT;3.14;False;3;FLOAT;0;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.PosterizeNode;45;-552.5537,-977.236;Inherit;True;47;2;1;COLOR;0,0,0,0;False;0;INT;47;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;23;-585.4172,-467.7592;Inherit;True;Ellipse;-1;;10;3ba94b7b3cfd5f447befde8107c04d52;0;3;2;FLOAT2;0,0;False;7;FLOAT;0;False;9;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;27;-276.5164,-457.8899;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;47;-20.76913,-456.6039;Inherit;True;Color Mask;-1;;11;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;3;-969.2198,268.4729;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;4;-760.228,269.5591;Inherit;False;1;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;48;341.9799,-395.0059;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BillboardNode;2;-778.8654,180.3675;Inherit;False;Cylindrical;True;0;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;53;846.9409,-445.7013;Inherit;False;Constant;_Color0;Color 0;2;0;Create;True;0;0;False;0;0.1745481,1,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-542.0494,251.4389;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;49;541.9418,-400.2202;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;1098.885,-222.9517;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;935.7064,-3.353786;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Misc/MagicAura;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;1;False;-1;1;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Geometry;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;38;0;40;0
WireConnection;38;2;42;0
WireConnection;43;0;44;0
WireConnection;43;1;42;0
WireConnection;45;1;43;0
WireConnection;23;2;38;0
WireConnection;23;7;30;0
WireConnection;23;9;30;0
WireConnection;27;0;45;0
WireConnection;27;1;23;0
WireConnection;47;1;27;0
WireConnection;4;0;3;0
WireConnection;48;0;47;0
WireConnection;5;0;2;0
WireConnection;5;1;4;0
WireConnection;49;0;48;0
WireConnection;56;0;53;0
WireConnection;56;1;47;0
WireConnection;0;2;56;0
WireConnection;0;10;49;0
WireConnection;0;11;5;0
ASEEND*/
//CHKSM=D703975C5DB3A7FBB113351B5B1E2830A5D30DAF