Shader "Custom/CustomLitShader"
{
    Properties
    {
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseColor ("Example Colour", Color) = (0, 0.66, 0.73, 1)
        _DitherLevel("DitherLevel", Range(0, 16)) = 0
        _Smoothness ("Smoothness", Float) = 0.5

        [Toggle(_ALPHATEST_ON)] _EnableAlphaTest("Enable Alpha Cutoff", Float) = 0.0
        _Cutoff ("Alpha Cutoff", Float) = 0.5

        [Toggle(_NORMALMAP)] _EnableBumpMap("Enable Normal/Bump Map", Float) = 0.0
        _BumpMap ("Normal/Bump Texture", 2D) = "bump" {}
        _BumpScale ("Bump Scale", Float) = 1

        [Toggle(_EMISSION)] _EnableEmission("Enable Emission", Float) = 0.0
        _EmissionMap ("Emission Texture", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Colour", Color) = (0, 0, 0, 0)

        [Toggle(_OutLineColor_ON)] _EnableOutLine("Enable OutLine", Float) = 0.0
        _OutLineColor ("OutLineColor", Color) = (0, 0, 0, 1)
        _OutlineWidth ("OutlineWidth", Range(0, 100)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _BaseColor;
            float _BumpScale;
            float4 _EmissionColor;
            float _Smoothness;
            float _Cutoff;
            half _DitherLevel;
        CBUFFER_END
        ENDHLSL

        Pass
        {
            Name "Example"
            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            // 標準 SRP ライブラリを使用して gles 2.0 をコンパイルするために必要です
            // すべてのシェーダーは HLSLcc でコンパイルする必要があり、現在デフォルトで HLSLcc を使用していないのは gles だけです
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles

            //#pragma target 4.5 // https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html

            #pragma vertex vert
            #pragma fragment frag

            // Materialのキーワード
            #pragma shader_feature _NORMALMAP
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON
            #pragma shader_feature _EMISSION
            //#pragma shader_feature _METALLICSPECGLOSSMAP
            //#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            //#pragma shader_feature _OCCLUSIONMAP
            //#pragma shader_feature _ _CLEARCOAT _CLEARCOATMAP // URP v10+

            //#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
            //#pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
            //#pragma shader_feature _SPECULAR_SETUP
            #pragma shader_feature _RECEIVE_SHADOWS_OFF

            // URPのキーワード
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            // Unityで定義されたキーワード
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile_fog

            // Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

                #ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
                float3 positionWS : TEXCOORD2;
                #endif

                float3 normalWS : TEXCOORD3;
                #ifdef _NORMALMAP
					float4 tangentWS 			: TEXCOORD4;
                #endif

                float3 viewDirWS : TEXCOORD5;
                half4 fogFactorAndVertexLight : TEXCOORD6; // x: fogFactor, yzw: vertex light

                #ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
					float4 shadowCoord			: TEXCOORD7;
                #endif

                float4 positionSS : TEXCOORD8;
            };

            // SurfaceInput.hlslで自動的に定義される。
            //TEXTURE2D(_BaseMap);
            //SAMPLER(sampler_BaseMap);

            #if SHADER_LIBRARY_VERSION_MAJOR < 9
			// この関数は URP v9.xx バージョンで追加されました。以前のバージョンの URP をサポートしたい場合は、代わりにそれを処理する必要があります。
			// ワールド空間のビューの方向 (ビューアの方向を指す) を計算します。
			float3 GetWorldSpaceViewDir(float3 positionWS) {
				if (unity_OrthoParams.w == 0) {
					// 視点
					return _WorldSpaceCameraPos - positionWS;
				} else {
					// 正投影法
					float4x4 viewMat = GetWorldToViewMatrix();
					return viewMat[2].xyz;
				}
			}
            #endif

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                VertexPositionInputs positionInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                OUT.color = IN.color;

                #ifdef REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
                OUT.positionWS = positionInputs.positionWS;
                #endif

                OUT.positionSS = ComputeScreenPos(OUT.positionCS);

                OUT.viewDirWS = GetWorldSpaceViewDir(positionInputs.positionWS);

                VertexNormalInputs normalInputs = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                OUT.normalWS = normalInputs.normalWS;
                #ifdef _NORMALMAP
					real sign = IN.tangentOS.w * GetOddNegativeScale();
					OUT.tangentWS = half4(normalInputs.tangentWS.xyz, sign);
                #endif

                half3 vertexLight = VertexLighting(positionInputs.positionWS, normalInputs.normalWS);
                half fogFactor = ComputeFogFactor(positionInputs.positionCS.z);

                OUT.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                OUTPUT_LIGHTMAP_UV(IN.lightmapUV, unity_LightmapST, OUT.lightmapUV);
                OUTPUT_SH(OUT.normalWS.xyz, OUT.vertexSH);

                #ifdef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
					OUT.shadowCoord = GetShadowCoord(positionInputs);
                #endif

                return OUT;
            }

            InputData InitializeInputData(Varyings IN, half3 normalTS)
            {
                InputData inputData = (InputData)0;

                #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                inputData.positionWS = IN.positionWS;
                #endif

                half3 viewDirWS = SafeNormalize(IN.viewDirWS);
                #ifdef _NORMALMAP
					float sgn = IN.tangentWS.w; // +1 または -1 のいずれかでなければなりません。
					float3 bitangent = sgn * cross(IN.normalWS.xyz, IN.tangentWS.xyz);
					inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(IN.tangentWS.xyz, bitangent.xyz, IN.normalWS.xyz));
                #else
                inputData.normalWS = IN.normalWS;
                #endif

                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                inputData.viewDirectionWS = viewDirWS;

                #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
					inputData.shadowCoord = IN.shadowCoord;
                #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
					inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                #else
                inputData.shadowCoord = float4(0, 0, 0, 0);
                #endif

                inputData.fogCoord = IN.fogFactorAndVertexLight.x;
                inputData.vertexLighting = IN.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SAMPLE_GI(IN.lightmapUV, IN.vertexSH, inputData.normalWS);
                return inputData;
            }

            SurfaceData InitializeSurfaceData(Varyings IN)
            {
                SurfaceData surfaceData = (SurfaceData)0;
                // 注意: SurfaceData surfaceData を使用するだけです。ここでは設定しません。
                // ただし、戻る前に構造体のすべての値が設定されていることを確認する必要があります。
                // SurfaceData に 0 をキャストすることで、すべての内容が自動的に 0 に設定されます。

                half4 albedoAlpha = SampleAlbedoAlpha(IN.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
                surfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
                surfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb * IN.color.rgb;

                // 簡単にするために、メタリック/スペキュラー マップまたはオクルージョン マップはサポートしていません。
                // その例については、https://github.com/Unity-Technologies/Graphics/blob/master/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl を参照してください。

                surfaceData.smoothness = 0.5;
                surfaceData.normalTS = SampleNormal(IN.uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
                surfaceData.emission = SampleEmission(IN.uv, _EmissionColor.rgb,
                                                      TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));

                surfaceData.occlusion = 1;

                return surfaceData;
            }

            // 閾値マップ
            static const float4x4 pattern =
            {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 565
            };
            static const int PATTERN_ROW_SIZE = 4;

            half4 frag(Varyings IN) : SV_Target
            {
                SurfaceData surfaceData = InitializeSurfaceData(IN);
                InputData inputData = InitializeInputData(IN, surfaceData.normalTS);

                // URP v10+ バージョンでは、これを使用できます。
                // half4 color = UniversalFragmentPBR(inputData, surfaceData);

                // ただし、他のバージョンでは、代わりにこれを使用する必要があります。
                // SurfaceData 構造体の使用を完全に避けることもできますが、整理するのに役立ちます。
                half4 color = UniversalFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic,
                                                   surfaceData.specular,
                                                   surfaceData.smoothness,
                                                   surfaceData.occlusion,
                                                   surfaceData.emission, surfaceData.alpha);

                color.rgb = MixFog(color.rgb, inputData.fogCoord);

                // color.a = OutputAlpha(color.a);
                // これが本当に重要かどうかはわかりません。それは次のように実装されます。
                // saturate(outputAlpha + _DrawObjectPassData.a);
                // ここで、_DrawObjectPassData.a は、不透明オブジェクトの場合は 1、アルファ ブレンドの場合は 0 です。
                // しかし、これは URP v8 で追加されたもので、それより前のバージョンにはありませんでした。
                // ただし、アルファが 0 ～ 1 の範囲を超えないようにするために、アルファを飽和させることもできます。
                color.a = saturate(color.a);

                // スクリーン座標
                float2 screenPos = IN.positionSS.xy / IN.positionSS.w;
                // 画面サイズを乗算して、ピクセル単位に
                float2 screenPosInPixel = screenPos.xy * _ScreenParams.xy;

                // ディザリングテクスチャ用のUVを作成
                int ditherUV_x = (int)fmod(screenPosInPixel.x, PATTERN_ROW_SIZE);
                int ditherUV_y = (int)fmod(screenPosInPixel.y, PATTERN_ROW_SIZE);
                float dither = pattern[ditherUV_x, ditherUV_y];

                // 閾値が0以下なら描画しない
                clip(dither - _DitherLevel);

                return color; // float4(inputData.bakedGI,1);
            }
            ENDHLSL
        }

        // UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        // これを行うことはできますが、CBUFFER が同じではないため、現在 SRP Batcher でのバッチ処理が中断されることに注意してください。
        // したがって、代わりにパスを手動で定義します。
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode"="ShadowCaster"
            }

            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            // 標準 srp ライブラリを使用して gles 2.0 をコンパイルするために必要です
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles
            //#pragma target 4.5

            // Materialのキーワード
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // GPUのインスタンス化
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"

            // 頂点の移動を行いたい場合は、頂点関数を変更する必要があることに注意してください。
            /*
            // 例: 
            #pragma vertex vert
 
            Varyings vert(Attributes input) {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
 
                // 変位の例
                input.positionOS += float4(0, _SinTime.y, 0, 0);
 
                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = GetShadowPositionHClip(input);
                return output;
            }*/

            // ShadowCasterPass を使用するということは、_BaseMap、_BaseColor、_Cutoff シェーダ プロパティも必要であることを意味します。
            // テクスチャである _BaseMap を除いて、それらも cbuffer に含まれます。
            ENDHLSL
        }

        // 同様に、DepthOnly パスが必要です。
        // UsePass "Universal Render Pipeline/Lit/DepthOnly"
        // 繰り返しますが、cbuffer が異なるため、SRP Batcher によるバッチ処理が中断されます。

        // DepthOnly パスは ShadowCaster に非常に似ていますが、シャドウ バイアス オフセットは含まれません。
        // Unity はシーンビューでオブジェクトの深度をレンダリングするときにこのパスを使用すると思います。
        // ただし、ゲームビュー/実際のカメラの深度テクスチャの場合は、それなしでも問題なくレンダリングされます。
        // ただし、Forward Renderer 機能で使用できる可能性があるため、おそらく引き続き含める必要があります。
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode"="DepthOnly"
            }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            // 標準 srp ライブラリを使用して gles 2.0 をコンパイルするために必要です
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x gles
            //#pragma target 4.5

            // Materialのキーワード
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            // GPUのインスタンス化
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            // URP が提供する Lit シェーダはこれを使用しますが、すでにある cbuffer も処理することに注意してください。
            // cbuffer を使用するようにシェーダーを変更することもできますが、単にこれを行うこともできます。
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"

            // 繰り返しますが、DepthOnlyPass を使用するということは、_BaseMap、_BaseColor、_Cutoff シェーダープロパティも必要であることを意味します。
            // テクスチャである _BaseMap を除いて、それらも cbuffer に含まれます。
            ENDHLSL
        }

        // URP には、ライトマップをベイクするときに使用される「メタ」パスもあります。
        // UsePass "Universal Render Pipeline/Lit/Meta"
        // これはまだ SRP Batcher を壊しますが、それが重要なのかどうか興味があります。
        // メタ パスはライトマップのベイク処理にのみ使用されるため、エディターでのみ使用されるのでしょうか?
        // とにかく、独自のメタパスを作成したい場合は、URP がサンプルとして提供するシェーダを見てください。

        Pass
        {
            Name "OutLine"
            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature _OutLineColor_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            float _OutlineWidth;
            half4 _OutLineColor;

            struct Attributes
            {
                float4 positionOS: POSITION;
                float4 normalOS: NORMAL;
                float4 tangentOS: TANGENT;
            };

            struct Varyings
            {
                float4 positionCS: SV_POSITION;
                float4 positionSS : TEXCOORD0;
            };

            // 閾値マップ
            static const float4x4 pattern =
            {
                0, 8, 2, 10,
                12, 4, 14, 6,
                3, 11, 1, 9,
                15, 7, 13, 565
            };
            static const int PATTERN_ROW_SIZE = 4;

            Varyings vert(Attributes v)
            {
                Varyings OUT;

                VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);

                float3 normalWS = vertexNormalInput.normalWS;
                float3 normalCS = TransformWorldToHClipDir(normalWS);

                VertexPositionInputs positionInputs = GetVertexPositionInputs(v.positionOS.xyz);
                OUT.positionCS = positionInputs.positionCS + float4(normalCS.xy * 0.001 * _OutlineWidth, 0, 0);

                OUT.positionSS = ComputeScreenPos(OUT.positionCS);

                return OUT;
            }

            half4 frag(Varyings IN): SV_Target
            {
                //#ifdef _EnableOutLine_ON
                float4 col = _OutLineColor;

                // スクリーン座標
                float2 screenPos = IN.positionSS.xy / IN.positionSS.w;
                // 画面サイズを乗算して、ピクセル単位に
                float2 screenPosInPixel = screenPos.xy * _ScreenParams.xy;

                // ディザリングテクスチャ用のUVを作成
                int ditherUV_x = (int)fmod(screenPosInPixel.x, PATTERN_ROW_SIZE);
                int ditherUV_y = (int)fmod(screenPosInPixel.y, PATTERN_ROW_SIZE);
                float dither = pattern[ditherUV_x, ditherUV_y];

                // 閾値が0以下なら描画しない
                clip(dither - _DitherLevel);
                return col;
                // #else
                // const float r = (_BaseMap_ST.r * _BaseColor).x;
                // clip(r - 0.5);
                // return 1;
                // #endif
            }
            ENDHLSL
        }
    }


}