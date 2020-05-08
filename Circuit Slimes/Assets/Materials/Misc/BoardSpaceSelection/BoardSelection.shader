// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BoardSelection"
{
	Properties
	{
		[PerRendererData]_Color("Color", Color) = (0,1,1,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_GridOpacity("GridOpacity", Float) = 0
		_Bias("Bias", Float) = 0
		_Scale("Scale", Float) = 1.44
		_GlobalOpacity("GlobalOpacity", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Color;
		uniform sampler2D _TextureSample0;
		uniform float4 _TextureSample0_ST;
		uniform float _Bias;
		uniform float _Scale;
		uniform float _GridOpacity;
		uniform float _GlobalOpacity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Emission = _Color.rgb;
			float2 uv_TextureSample0 = i.uv_texcoord * _TextureSample0_ST.xy + _TextureSample0_ST.zw;
			float temp_output_2_0_g14 = 0.8;
			float2 appendResult10_g15 = (float2(temp_output_2_0_g14 , temp_output_2_0_g14));
			float2 temp_output_11_0_g15 = ( abs( (frac( (i.uv_texcoord*float2( 11,11 ) + float2( 0,0 )) )*2.0 + -1.0) ) - appendResult10_g15 );
			float2 break16_g15 = ( 1.0 - ( temp_output_11_0_g15 / fwidth( temp_output_11_0_g15 ) ) );
			float2 appendResult10_g17 = (float2(0.48 , 0.48));
			float2 temp_output_11_0_g17 = ( abs( (i.uv_texcoord*2.0 + -1.0) ) - appendResult10_g17 );
			float2 break16_g17 = ( 1.0 - ( temp_output_11_0_g17 / fwidth( temp_output_11_0_g17 ) ) );
			float temp_output_37_0 = ( 0.48 - 0.04 );
			float2 appendResult10_g18 = (float2(temp_output_37_0 , temp_output_37_0));
			float2 temp_output_11_0_g18 = ( abs( (i.uv_texcoord*2.0 + -1.0) ) - appendResult10_g18 );
			float2 break16_g18 = ( 1.0 - ( temp_output_11_0_g18 / fwidth( temp_output_11_0_g18 ) ) );
			o.Alpha = ( ( ( saturate( ( ( tex2D( _TextureSample0, uv_TextureSample0 ) + _Bias ) * _Scale ) ) * ( 1.0 - saturate( min( break16_g15.x , break16_g15.y ) ) ) * _GridOpacity ) + ( saturate( min( break16_g17.x , break16_g17.y ) ) - saturate( min( break16_g18.x , break16_g18.y ) ) ) ) * _GlobalOpacity ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18000
0;73.6;1027;710;844.0636;445.5187;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;8;-1375.341,489.5776;Inherit;False;Constant;_SquareSide;SquareSide;1;0;Create;True;0;0;False;0;0.48;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-1486.204,-487.8004;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;-1;None;4c9dfb1f59cbe824c824c7af19361f6e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;57;-1442.843,-203.2516;Inherit;False;Property;_Scale;Scale;4;0;Create;True;0;0;False;0;1.44;9.06;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1384.284,651.0499;Inherit;False;Constant;_SideThickness;SideThickness;4;0;Create;True;0;0;False;0;0.04;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-1444.157,-276.8343;Inherit;False;Property;_Bias;Bias;3;0;Create;True;0;0;False;0;0;-0.45;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-1399.833,295.0449;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;37;-1159.109,588.783;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;53;-1065.731,-409.5461;Inherit;True;ConstantBiasScale;-1;;13;63208df05c83e8e49a48ffbdce2e43a0;0;3;3;COLOR;0,0,0,0;False;1;FLOAT;-0.05;False;2;FLOAT;1.84;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;1;-1457.704,-76.64881;Inherit;True;Grid;-1;;14;a9240ca2be7e49e4f9fa3de380c0dbe9;0;3;5;FLOAT2;11,11;False;6;FLOAT2;0,0;False;2;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-859.2968,59.9677;Inherit;False;Property;_GridOpacity;GridOpacity;2;0;Create;True;0;0;False;0;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;35;-975.6279,297.3253;Inherit;False;Rectangle;-1;;17;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0.5;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-779.2842,-309.6837;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;2;-1113.468,-75.97295;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;36;-983.2033,548.5385;Inherit;False;Rectangle;-1;;18;6b23e0c975270fb4084c354b2c83366a;0;3;1;FLOAT2;0,0;False;2;FLOAT;0.5;False;3;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-580.5505,-95.62939;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;33;-747.2529,492.277;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-352.2664,196.7393;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-310.869,469.119;Inherit;False;Property;_GlobalOpacity;GlobalOpacity;5;0;Create;True;0;0;False;0;0;0.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;14;-224.6421,-75.50772;Inherit;False;Property;_Color;Color;0;1;[PerRendererData];Create;True;0;0;False;0;0,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-87.47141,239.0528;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;87.64148,-49.17528;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;BoardSelection;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.38;True;False;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;5;False;2;5;False;-1;10;False;-1;0;5;False;-1;10;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;37;0;8;0
WireConnection;37;1;34;0
WireConnection;53;3;45;0
WireConnection;53;1;56;0
WireConnection;53;2;57;0
WireConnection;35;1;28;0
WireConnection;35;2;8;0
WireConnection;35;3;8;0
WireConnection;55;0;53;0
WireConnection;2;0;1;0
WireConnection;36;1;28;0
WireConnection;36;2;37;0
WireConnection;36;3;37;0
WireConnection;47;0;55;0
WireConnection;47;1;2;0
WireConnection;47;2;48;0
WireConnection;33;0;35;0
WireConnection;33;1;36;0
WireConnection;46;0;47;0
WireConnection;46;1;33;0
WireConnection;59;0;46;0
WireConnection;59;1;58;0
WireConnection;0;2;14;0
WireConnection;0;9;59;0
ASEEND*/
//CHKSM=E526ED0551064ABF873A66E2BEF0ACF15CDFF4B8