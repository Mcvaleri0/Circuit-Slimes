// Upgrade NOTE: upgraded instancing buffer 'FaceShader' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FaceShader"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0
		_FaceOffset("FaceOffset", Vector) = (0,0,0,0)
		_FaceTexture("FaceTexture", 2D) = "white" {}
		_EyeMask("EyeMask", 2D) = "white" {}
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
		
		
		
		struct Input {
			half filler;
		};
		UNITY_INSTANCING_BUFFER_START(FaceShader)
		UNITY_DEFINE_INSTANCED_PROP( float4, _ASEOutlineColor )
#define _ASEOutlineColor_arr FaceShader
		UNITY_DEFINE_INSTANCED_PROP( float, _ASEOutlineWidth )
#define _ASEOutlineWidth_arr FaceShader
		UNITY_INSTANCING_BUFFER_END(FaceShader)
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineWidth_arr, _ASEOutlineWidth) );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = UNITY_ACCESS_INSTANCED_PROP(_ASEOutlineColor_arr, _ASEOutlineColor).rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _EyeMask;
		uniform sampler2D _FaceTexture;

		UNITY_INSTANCING_BUFFER_START(FaceShader)
			UNITY_DEFINE_INSTANCED_PROP(float2, _FaceOffset)
#define _FaceOffset_arr FaceShader
		UNITY_INSTANCING_BUFFER_END(FaceShader)

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 _FaceOffset_Instance = UNITY_ACCESS_INSTANCED_PROP(_FaceOffset_arr, _FaceOffset);
			float2 uv_TexCoord8 = i.uv_texcoord * float2( 3.86,1.43 ) + ( float2( -2.38,-0.26 ) + _FaceOffset_Instance );
			float2 FaceUV9 = uv_TexCoord8;
			o.Albedo = saturate( ( ( 1.0 - ( tex2D( _EyeMask, FaceUV9 ).b * (( sin( ( _Time.y * 3.0 ) ) > 0.96 ) ? 1.0 :  0.0 ) ) ) * tex2D( _FaceTexture, FaceUV9 ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=17900
288.8;73.6;1246;710;2532.056;1797.115;4.247692;True;False
Node;AmplifyShaderEditor.CommentaryNode;1;-782.4167,-1003.398;Inherit;False;896.6765;579.775;Adjusted UV mapping to be able to control face posiiton;6;9;8;6;5;3;2;Face UVs;0.4575472,0.5343004,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;2;-692.9088,-562.2853;Inherit;False;InstancedProperty;_FaceOffset;FaceOffset;0;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;3;-706.2208,-708.9685;Inherit;False;Constant;_BaseFaceOffset;BaseFaceOffset;18;0;Create;True;0;0;False;0;-2.38,-0.26;-2.38,-0.26;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;4;-815.981,-190.2575;Inherit;False;1860.597;582.574;;13;21;20;19;18;17;16;15;14;13;12;11;10;7;Blinking;1,0.7216981,0.9518253,1;0;0
Node;AmplifyShaderEditor.Vector2Node;5;-705.2347,-899.9685;Inherit;False;Constant;_BaseFaceTilling;BaseFaceTilling;19;0;Create;True;0;0;False;0;3.86,1.43;3.86,1.43;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleAddOpNode;6;-480.9087,-618.2853;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;7;-782.179,165.2755;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-342.6897,-906.3315;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;9;-87.41978,-911.5295;Inherit;False;FaceUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-599.488,165.8005;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;12;-454.322,166.0794;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;11;-574.9121,-48.13761;Inherit;False;9;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;13;-369.749,-101.3915;Inherit;True;Property;_EyeMask;EyeMask;2;0;Create;True;0;0;False;0;-1;ce02c6bba51f71349bd1782d79c9bf23;ce02c6bba51f71349bd1782d79c9bf23;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareGreater;14;-309.9839,134.3565;Inherit;True;4;0;FLOAT;0;False;1;FLOAT;0.96;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;20.72603,-95.68741;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;16;71.8149,162.5744;Inherit;False;9;FaceUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;17;231.3798,-51.81852;Inherit;False;FLOAT;1;0;FLOAT;0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SamplerNode;18;368.4169,100.3755;Inherit;True;Property;_FaceTexture;FaceTexture;1;0;Create;True;0;0;False;0;-1;45590d454fb37c444866c18ea64776b7;45590d454fb37c444866c18ea64776b7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;19;463.9169,-19.81363;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;620.7198,-35.05851;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;21;843.1021,-35.14151;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1258.102,-824.1573;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;FaceShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;3;0
WireConnection;6;1;2;0
WireConnection;8;0;5;0
WireConnection;8;1;6;0
WireConnection;9;0;8;0
WireConnection;10;0;7;0
WireConnection;12;0;10;0
WireConnection;13;1;11;0
WireConnection;14;0;12;0
WireConnection;15;0;13;3
WireConnection;15;1;14;0
WireConnection;17;0;15;0
WireConnection;18;1;16;0
WireConnection;19;0;17;0
WireConnection;20;0;19;0
WireConnection;20;1;18;0
WireConnection;21;0;20;0
WireConnection;0;0;21;0
ASEEND*/
//CHKSM=02D0F7060FAC5C88A0399BE605E7FB0E36AF80AB