// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "BlobShadow"
{
	Properties
	{
		[HDR]_Color("Color", Color) = (0,0,0,0)
		_Intensity("Intensity", Float) = 1
		_Power("Power (Sharpness)", Float) = 1
		[Toggle(_ALLOWSHAPEBLENDING_ON)] _AllowShapeBlending("Allow Shape Blending", Float) = 0
		_CubeToSphereBlend("Cube to Sphere Blend", Range( 0 , 1)) = 1
		_RoundedCubeBias("RoundedCubeBias", Range( 0 , 3)) = 2.5
		_RoundedCubePower("RoundedCubePower", Range( 0 , 5)) = 3.5
	}

	SubShader
	{
		
		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
		
		Cull Front
		HLSLINCLUDE
		#pragma target 3.0
		ENDHLSL

		
		Pass
		{
			Name "Forward"
			Tags { "LightMode"="UniversalForward" }
			
			Blend SrcAlpha OneMinusSrcAlpha , One OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
			Offset 0,0
			ColorMask RGB
			

			HLSLPROGRAM
			#define _RECEIVE_SHADOWS_OFF 1
			#pragma multi_compile_instancing
			#pragma multi_compile_fog
			#define ASE_FOG 1
			#define ASE_SRP_VERSION 999999
			#define REQUIRE_DEPTH_TEXTURE 1
			#define _AlphaClip 1

			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x

			#pragma vertex vert
			#pragma fragment frag


			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#pragma shader_feature _ALLOWSHAPEBLENDING_ON


			uniform float4 _CameraDepthTexture_TexelSize;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float _CubeToSphereBlend;
			float _RoundedCubeBias;
			float _RoundedCubePower;
			float _Power;
			float _Intensity;
			CBUFFER_END

			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#ifdef ASE_FOG
				float fogFactor : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float CubeToSphereBlend89( float3 LocalPoint , half Distance , half Blend , half RoundedCubeBias , half RoundedCubePower )
			{
				#ifdef _ALLOWSHAPEBLENDING_ON
					half3 normalized = saturate(abs(LocalPoint));
					half maximized = max(max(normalized.x, normalized.y), normalized.z);
					half roundedEdges = pow((Distance - maximized) * RoundedCubeBias, RoundedCubePower);
					return lerp(roundedEdges + maximized, Distance, Blend);
				#else
					return Distance;
				#endif
			}
			

			VertexOutput vert ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord1 = screenPos;
				float3 objectToViewPos = TransformWorldToView(TransformObjectToWorld(v.vertex.xyz));
				float eyeDepth = -objectToViewPos.z;
				o.ase_texcoord2.x = eyeDepth;
				float4 appendResult2_g2 = (float4(_WorldSpaceCameraPos , 1.0));
				float4 transform3_g2 = mul(GetWorldToObjectMatrix(),appendResult2_g2);
				float3 temp_output_5_0_g2 = (transform3_g2).xyz;
				float3 vertexToFrag10_g2 = ( v.vertex.xyz - temp_output_5_0_g2 );
				o.ase_texcoord2.yzw = vertexToFrag10_g2;
				float3 vertexToFrag12_g2 = temp_output_5_0_g2;
				o.ase_texcoord3.xyz = vertexToFrag12_g2;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord3.w = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
				o.clipPos = vertexInput.positionCS;
				#ifdef ASE_FOG
				o.fogFactor = ComputeFogFactor( vertexInput.positionCS.z );
				#endif
				return o;
			}

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float4 screenPos = IN.ase_texcoord1;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float eyeDepth8_g2 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float eyeDepth = IN.ase_texcoord2.x;
				float3 vertexToFrag10_g2 = IN.ase_texcoord2.yzw;
				float3 vertexToFrag12_g2 = IN.ase_texcoord3.xyz;
				float3 temp_output_91_0 = ( ( ( eyeDepth8_g2 / eyeDepth ) * vertexToFrag10_g2 ) + vertexToFrag12_g2 );
				float3 LocalPoint89 = temp_output_91_0;
				float Distance89 = saturate( distance( temp_output_91_0 , float3( 0,0,0 ) ) );
				float Blend89 = _CubeToSphereBlend;
				float RoundedCubeBias89 = _RoundedCubeBias;
				float RoundedCubePower89 = _RoundedCubePower;
				float localCubeToSphereBlend89 = CubeToSphereBlend89( LocalPoint89 , Distance89 , Blend89 , RoundedCubeBias89 , RoundedCubePower89 );
				float smoothstepResult88 = smoothstep( 0.0 , 1.0 , ( ( 1.0 - saturate( pow( ( localCubeToSphereBlend89 * 2.0 ) , _Power ) ) ) * _Intensity ));
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = _Color.rgb;
				float Alpha = smoothstepResult88;
				float AlphaClipThreshold = 0.01;

				#if _AlphaClip
					clip( Alpha - AlphaClipThreshold );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				return half4( Color, Alpha );
			}

			ENDHLSL
		}

	
	}
	CustomEditor "UnityEditor.ShaderGraph.PBRMasterGUI"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=17101
1840;285;1906;1035;860.181;226.3145;1;True;False
Node;AmplifyShaderEditor.FunctionNode;91;-474.055,142.8543;Inherit;False;ViewToLocalSpace;-1;;2;6ed058159163bab48947d4029c32aba2;0;0;1;FLOAT3;0
Node;AmplifyShaderEditor.DistanceOpNode;21;-238.5609,204.7698;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;94;-104.1814,202.6855;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-281.9992,311.4529;Inherit;False;Property;_CubeToSphereBlend;Cube to Sphere Blend;4;0;Create;False;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-253.9746,387.3762;Inherit;False;Property;_RoundedCubeBias;RoundedCubeBias;5;0;Create;False;0;0;False;0;2.5;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-210.9746,467.3762;Inherit;False;Property;_RoundedCubePower;RoundedCubePower;6;0;Create;False;0;0;False;0;3.5;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;89;74.50961,147.2994;Inherit;False;#ifdef _ALLOWSHAPEBLENDING_ON$	half3 normalized = saturate(abs(LocalPoint))@$	half maximized = max(max(normalized.x, normalized.y), normalized.z)@$	half roundedEdges = pow((Distance - maximized) * RoundedCubeBias, RoundedCubePower)@$	return lerp(roundedEdges + maximized, Distance, Blend)@$#else$	return Distance@$#endif;1;False;5;True;LocalPoint;FLOAT3;0,0,0;In;;Float;False;True;Distance;FLOAT;0;In;;Half;False;True;Blend;FLOAT;0;In;;Half;False;True;RoundedCubeBias;FLOAT;0;In;;Half;False;True;RoundedCubePower;FLOAT;0;In;;Half;False;CubeToSphereBlend;True;False;0;5;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;410.3417,161.4868;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;339.3417,265.4868;Inherit;False;Property;_Power;Power (Sharpness);2;0;Create;False;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;28;543.3417,171.4868;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;24;689.3417,155.4868;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;27;694.3417,237.4868;Inherit;False;Property;_Intensity;Intensity;1;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;818.3417,154.4868;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;977.3417,182.4868;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;56;53.03553,24.51889;Inherit;False;Property;_AllowShapeBlending;Allow Shape Blending;3;0;Create;False;0;0;True;0;0;0;1;True;;Toggle;2;Key0;Key1;Create;False;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;88;1135.691,175.0773;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1139.809,293.8259;Inherit;False;Constant;_Clip;Clip;5;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;23;1067.451,-15.01259;Inherit;False;Property;_Color;Color;0;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1336.179,150.3162;Float;False;True;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;BlobShadow;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;0;Forward;7;False;False;False;True;1;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;0;True;1;5;False;-1;10;False;-1;1;1;False;-1;10;False;-1;False;False;False;True;True;True;True;False;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;7;False;-1;True;False;0;False;-1;0;False;-1;True;1;LightMode=UniversalForward;False;0;Hidden/InternalErrorShader;0;0;Standard;10;Surface;1;  Blend;0;Two Sided;2;Cast Shadows;0;Receive Shadows;0;GPU Instancing;1;LOD CrossFade;0;Built-in Fog;1;Meta Pass;0;Vertex Position,InvertActionOnDeselection;1;0;4;True;False;False;False;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;2;DepthOnly;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;True;False;False;False;False;0;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;3;Meta;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;True;2;False;-1;False;False;False;False;False;True;1;LightMode=Meta;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;2;UnityEditor.ShaderGraph.PBRMasterGUI;0;3;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;1;ShadowCaster;0;False;False;False;True;0;False;-1;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;0;0;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;0;Hidden/InternalErrorShader;0;0;Standard;0;0
WireConnection;21;0;91;0
WireConnection;94;0;21;0
WireConnection;89;0;91;0
WireConnection;89;1;94;0
WireConnection;89;2;41;0
WireConnection;89;3;92;0
WireConnection;89;4;93;0
WireConnection;22;0;89;0
WireConnection;28;0;22;0
WireConnection;28;1;29;0
WireConnection;24;0;28;0
WireConnection;25;0;24;0
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;88;0;26;0
WireConnection;1;2;23;0
WireConnection;1;3;88;0
WireConnection;1;4;60;0
ASEEND*/
//CHKSM=44F7F512320BA4DF53C8BCB63FC3BC3AE3171AAF