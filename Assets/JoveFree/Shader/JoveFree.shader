// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_LightmapInd', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D
// Upgrade NOTE: replaced tex2D unity_LightmapInd with UNITY_SAMPLE_TEX2D_SAMPLER


Shader "Jove/Free" 
{
	Properties 
	{
		_Color   ("Diffuse Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse(RGB) Roughness(A)", 2D) = "white" {}
		_BumpMap ("Normal(RGB)", 2D) = "white" {}
		_MaskMap ("Metallic(R), Reflective(G), Occlusion(B)", 2D) = "white" {}
		_DiffMap ("Diffuse Environment Map", Cube) = "black" {}
		_SpecMap ("Specular Environment Map", Cube) = "black" {}
		_nMips ("Specular cube mips", float) = 1
	}
	
	SubShader 
	{
		Pass
		{
			Tags 
			{
				"Queue"="Geometry"
				"RenderType"="Opaque"
				"LightMode" = "ForwardBase"
			}
			LOD 400
			Blend Off
				
			CGPROGRAM
			#include "UnityCG.cginc" 
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			#pragma multi_compile_fwdbase
			#pragma glsl
			#pragma target 3.0
 			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			#define Pi 3.14159265359
			
			uniform sampler2D _MainTex;
			uniform sampler2D _BumpMap;
			uniform sampler2D _MaskMap;
			uniform samplerCUBE _SpecMap;
			uniform fixed _nMips;
			uniform samplerCUBE _DiffMap;

			uniform fixed4 _Color;
			uniform float4 _MainTex_ST;

			struct a2v
			{
			    float4 vertex : POSITION;
			    float3 normal : NORMAL;
			    float4 texcoord : TEXCOORD0; 
			    float4 texcoord1 : TEXCOORD1;   
			    float4 tangent : TANGENT;
			};

			struct v2f
			{
			    float4 pos : SV_POSITION;
			    float2 tex : TEXCOORD0;
			    float4 posWS : TEXCOORD1;		        
			    float3 normalDir : TEXCOORD2;
			    float3 tangentDir : TEXCOORD3;
			    float3 binormalDir : TEXCOORD4;
			    LIGHTING_COORDS(5,6)
			    #ifndef LIGHTMAP_OFF
			    	float2 lightmapUV : TEXCOORD7;	
				#endif
			};	

			#ifndef LIGHTMAP_OFF
				// sampler2D unity_Lightmap;
				// float4 unity_LightmapST;
				#ifndef DIRLIGHTMAP_OFF
					// sampler2D unity_LightmapInd;
				#endif
			#endif

			v2f vert(a2v v)
			{
			    v2f output;

			    output.normalDir = normalize(mul(float4(v.normal, 0.0), _World2Object).xyz);
			    output.tangentDir = normalize(mul(_Object2World, v.tangent).xyz );
			    output.binormalDir = normalize(cross(output.normalDir, output.tangentDir) * v.tangent.w );  
			    output.posWS = mul(_Object2World, v.vertex);
			    output.tex = TRANSFORM_TEX(v.texcoord, _MainTex);
			    output.pos = mul(UNITY_MATRIX_MVP, v.vertex);         
			    TRANSFER_VERTEX_TO_FRAGMENT(output);
				#ifndef LIGHTMAP_OFF
					output.lightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
				#endif           
			    return output;                                       
			}

			inline fixed3 FresnelRoughness(fixed3 reflectance, fixed XdotY, fixed roughness)
			{
			    return reflectance + (max(roughness, reflectance) - reflectance) * pow(1.0 - XdotY, 5.0);
			}

			inline fixed3 Fresnel(fixed3 reflectance, fixed XdotY)
			{
			    return reflectance + (fixed3(1.0, 1.0, 1.0) - reflectance) * pow(1.0 - XdotY, 5.0);
			}

			inline fixed CTGeo (fixed k, fixed NDotX)
			{
				return NDotX / (NDotX * (1 - k) + k);
				//return 1.0f / (NDotX + sqrt(k + (1 - k) * NDotX * NDotX));
			}

			inline fixed CTSpecular(fixed3 cSpec, fixed NdotL, fixed NdotV, fixed3 n, fixed3 h, fixed3 v, fixed3 l, fixed roughness)
			{
				if (NdotL <= 0.0f)
				{
					return 0.0f;
				}
				fixed NdotH = saturate(dot(n, h));
				fixed NdotH2 = NdotH * NdotH;
			    
			    fixed alpha = roughness*roughness;
			    fixed alpha2 = alpha*alpha;
			 	fixed distribution = alpha2 / (Pi * pow(NdotH2*(alpha2 - 1.0f) + 1.0f, 2.0f)); 
			   	
				fixed k = pow(roughness+1, 2) / 8;
				k = alpha / 2;
				fixed G1 = CTGeo(k, NdotL);
				fixed G2 = CTGeo(k, NdotV);
				fixed visibility = G1 * G2;
			   	
			   	return distribution * visibility;
			}

			fixed4 frag(v2f input) : COLOR
			{    
			    fixed4 albedoMap = tex2D(_MainTex, input.tex.xy);
			    fixed3 NormalMap = UnpackNormal(tex2D(_BumpMap, input.tex.xy));
			    fixed4 maskMap = tex2D(_MaskMap, input.tex.xy); //Metal map (R), Reflective (G), Cavity (B), Glow (A)
			    
			 
			    maskMap.g = -maskMap.g*maskMap.r + maskMap.g;
			    albedoMap.xyz *= _Color.xyz; 
			    
			    fixed3 cSpec = lerp(fixed3(0.04,0.04,0.04), albedoMap.rgb, maskMap.r);
			    float3x3 local2World = float3x3(input.tangentDir, input.binormalDir, input.normalDir);   
			    
				#ifndef LIGHTMAP_OFF
				   	fixed3 lightMap = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, input.lightmapUV));
					#ifndef DIRLIGHTMAP_OFF
						half3 scalePerBasis = DecodeLightmap(UNITY_SAMPLE_TEX2D_SAMPLER(unity_LightmapInd,unity_Lightmap, input.lightmapUV));
						UNITY_DIRBASIS
						half3 normalRNM = saturate(mul(unity_DirBasis, NormalMap));
						lightMap *= dot(normalRNM, scalePerBasis);
						half3 l = normalize (scalePerBasis.x * unity_DirBasis[0] + scalePerBasis.y * unity_DirBasis[1] + scalePerBasis.z * unity_DirBasis[2]);
						l = mul(l, local2World);
					#else
			   			fixed3 l = normalize(_WorldSpaceLightPos0.xyz);
					#endif
				#else
			   		fixed3 l = normalize(_WorldSpaceLightPos0.xyz);
				#endif
			 
			    fixed3 n = normalize(mul(NormalMap, local2World));	
			    fixed3 v =  normalize(_WorldSpaceCameraPos - input.posWS.xyz);
			    fixed3 h = normalize(l + v);
			    half3 reflection = reflect(half3(-v.x, -v.y, -v.z), n);
			    
				fixed roughness = -_Color.a * albedoMap.a + _Color.a;
				fixed HdotL = saturate(dot(h, l));
				fixed NdotL = saturate(dot(n, l));
				fixed NdotV = max(dot(n, v), 0.0001f);
				fixed3 fresnel = Fresnel(cSpec, HdotL);
			    fixed3 spec = CTSpecular(cSpec, NdotL, NdotV, n, h, v, l, roughness) * fresnel;  
			                                              
				fixed3 diff = albedoMap.xyz * (-fresnel*NdotL + fixed3(1.0f, 1.0f, 1.0f)); 
				
				fixed4 tempDiff = texCUBE(_DiffMap, n);
				fixed3 diffIBL = albedoMap.xyz * tempDiff.xyz * 6.0f * tempDiff.a;
				
				fixed4 tempSpec = texCUBElod(_SpecMap, half4(reflection, roughness*_nMips));
				fixed3 specIBL = tempSpec.rgb * 6.0f * tempSpec.a;

				fixed3 reflectiveFresnel = FresnelRoughness(cSpec, NdotV, roughness);
				fixed3 metallicIBL = specIBL*reflectiveFresnel*maskMap.r;
				fixed3 dielectricIBL = -maskMap.r*diffIBL + diffIBL;
				
				fixed3 colorOutput = (0.0, 0.0, 0.0);

				colorOutput = -diff * maskMap.r + diff;
				colorOutput = lerp(colorOutput, diff*specIBL*reflectiveFresnel, maskMap.g);
				colorOutput = spec + colorOutput;
				
				#ifdef DIRLIGHTMAP_OFF
					#ifdef LIGHTMAP_OFF
						colorOutput *= _LightColor0.xyz;
					#endif
					colorOutput *= _LightColor0.w;
				#endif
				
				colorOutput *= NdotL;
				
				#ifdef LIGHTMAP_OFF
					colorOutput *= LIGHT_ATTENUATION(input);
				#else
					colorOutput *= lightMap;
				#endif
				
				colorOutput = dielectricIBL + colorOutput;
				colorOutput = metallicIBL + colorOutput;
				colorOutput *= maskMap.b;
				
			   	return fixed4(colorOutput, 1.0); 	
			}
	
			ENDCG
		}
	}
	FallBack "Diffuse"
}
