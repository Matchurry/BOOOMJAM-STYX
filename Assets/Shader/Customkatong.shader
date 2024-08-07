// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/katong"
{
	Properties
	{
		[Header(Light Setting)]
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Color("Color物体本身颜色(自发光)",Color) = (1,1,1,1)
		_Ambiel_K("Ambiel_K(环境光系数)",Float) = 1
		_Diff_K("Diff_K(漫反射系数)", Float) = 1
		_Diff_Color("Diff_Color(漫反射颜色)",Color) = (1,1,1,1)
		_HLight_K("HLight_K(高光系数)", Float) = 1
		_Specular_Color("Specular_Color(镜面反射颜色)", Color) = (1, 1, 1, 1)
		_ShadowThreshold("ShadowThreshold(漫反射光影分界阈值)", Range(-1.0, 1.0)) = 0.2
		_ShadowColor("ShadowColor(漫反射暗处颜色)", Color) = (1.0, 1.0, 1.0, 1.0)
		[Header(Line Setting)]
		[Space(5)]
		_LineColor("LineColor:轮廓颜色",Color) = (1,1,1,1)
		_Line("Line:轮廓宽度",Range(0,0.1)) = 0.002

	}
	SubShader
	{	
		CGINCLUDE
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
		#pragma vertex vert
		ENDCG
		
		pass {
			
			//使用光照模型
			Tags{"LightingMode" = "ForwardBase"}
			//剔除背面
			Cull Back
			CGPROGRAM
			#pragma fragment fragLig
			#pragma multi_compile_fwdbase
			#pragma	multi_compile_shadowcaster

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Ambiel_K;
			float _Diff_K;
			float4 _Diff_Color;
			float _HLight_K;
			float4 _Specular_Color;
			float _ShadowThreshold;
			float4 _ShadowColor;

			struct a2v {
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
				float3 normal:NORMAL;
				float4 color:COLOR0;
			};
			struct v2f {
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
				float3 color:TEXCOORD3;
				//阴影
				SHADOW_COORDS(4) //声明阴影坐标，参数2表示下一个可用的插值寄存器的索引值
			};
			v2f vert(a2v v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				//坐标转换到裁剪空间
				o.pos = UnityObjectToClipPos(v.vertex);
				//贴图
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				//阴影计算
				TRANSFER_SHADOW(o);//计算上一步声明发阴影纹理坐标
				return o;
			}
			fixed4 fragLig(v2f i) :SV_Target
			{
				float3 L = normalize(UnityWorldSpaceLightDir(i.worldPos));
				float3 N = normalize(i.worldNormal);
				float3 V = normalize(UnityWorldSpaceViewDir(i.worldPos));

				//纹理采样
				float3 color = tex2D(_MainTex, i.uv).rgb*_Color.rgb;

				fixed spec = dot(N, normalize(L + V)) + (i.color.g - 0.5) * 2;
				fixed w = fwidth(spec)*2.0;
				//环境光
				float3 Ambiel = UNITY_LIGHTMODEL_AMBIENT.xyz*_Ambiel_K*color;
				//阴影计算atten
				UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
				//半兰伯特光照的漫反射
				fixed halfdiff = dot(L, N) + (i.color.r - 0.5 * 4);
				fixed diffStep = smoothstep(-w + _ShadowThreshold, w + _ShadowThreshold, halfdiff);
				fixed4 light = _LightColor0 * 0.5 + 0.5;
				//漫反射
				fixed3 Diff = light * color*(diffStep + (1 - diffStep)*_ShadowColor)*_Diff_Color*_Diff_K;
			

				//镜面反射-高光
				float3 Specul = _Specular_Color.rgb * lerp(0, 1, smoothstep(-w, w, dot(N, normalize(L + V)) + _HLight_K - 1))*step(0.0001, _HLight_K);

				return fixed4(Ambiel + Diff + Specul, 1.0);
			}
			ENDCG
		}
		pass
		{
			Tags{ "LightMode" = "ForwardAdd" } //ForwardAdd ：多灯混合//
			Blend One One//混合模式，表示该Pass计算的光照结果可以在帧缓存中与之前的光照结果进行叠加，否则会覆盖之前的光照结果
			CGPROGRAM
			#pragma fragment fragLight
			#pragma multi_compile_fwdadd//

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			float _Ambiel_K;
			float _Diff_K;
			float4 _Diff_Color;
			float _HLight_K;
			float4 _Specular_Color;
			float _ShadowThreshold;
			float4 _ShadowColor;

			struct a2v {
				float4 vertex:POSITION;
				float2 uv:TEXCOORD0;
				float3 normal:NORMAL;
				float4 color:COLOR;
			};
			struct v2f {
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
				float3 color:TEXCOORD3;
				//点光源需要的衰减
				LIGHTING_COORDS(6, 7)
			};
			v2f vert(a2v v)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				//坐标转换到裁剪空间
				o.pos = UnityObjectToClipPos(v.vertex);
				//贴图
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.color = v.color;
				return o;
			}
			fixed4 fragLight(v2f i) :SV_Target
			{
				fixed3 N = normalize(i.worldNormal);
				#ifdef USING_DIRECTIONAL_LIGHT  //平行光下可以直接获取世界空间下的光照方向
							fixed3 L = normalize(_WorldSpaceLightPos0.xyz);
				#else  //其他光源下_WorldSpaceLightPos0代表光源的世界坐标，与顶点的世界坐标的向量相减可得到世界空间下的光照方向
							fixed3 L = normalize(_WorldSpaceLightPos0.xyz - i.worldPos.xyz);
				#endif
				//相机坐标减去顶点坐标
				fixed3 viewdir = normalize(UnityWorldSpaceViewDir(i.worldPos));


				fixed spec = dot(N, normalize(L + viewdir)) + (i.color.g - 0.5) * 2;
				fixed w = fwidth(spec)*2.0;
				//半兰伯特光照的漫反射
				fixed halfdiff = dot(L, N) + (i.color.r - 0.5 * 4);
				fixed diffStep = smoothstep(-w + _ShadowThreshold, w + _ShadowThreshold, halfdiff);
				fixed4 light = _LightColor0 * 0.5 + 0.5;


				//漫反射
				fixed3 Diff = light *(diffStep + (1 - diffStep)*_ShadowColor)*_Diff_Color*_Diff_K;
				//镜面反射-高光
				float3 Specul = _Specular_Color.rgb * lerp(0, 1, smoothstep(-w, w, dot(N, normalize(L + viewdir)) + _HLight_K - 1))*step(0.0001, _HLight_K);

				//点光源需要的衰减
				fixed atten;
				#ifdef USING_DIRECTIONAL_LIGHT//平行光下不存在光照衰减，恒值为1
					atten = 1.0;
				#else
					#if defined(POINT)//点光源的光照衰减计算
						//unity_WorldToLight内置矩阵，世界空间到光源空间变换矩阵。与顶点的世界坐标相乘可得到光源空间下的顶点坐标
						float3 lightcoord = mul(unity_WorldToLight, float4(i.worldPos, 1)).xyz;
						/*利用Unity内置函数tex2D对Unity内置纹理_LightTexture0进行纹理采样计算光源衰减，获取其衰减纹理，
						再通过UNITY_ATTEN_CHANNEL得到衰减纹理中衰减值所在的分量，以得到最终的衰减值*/
						atten = tex2D(_LightTexture0, dot(lightcoord, lightcoord).rr).UNITY_ATTEN_CHANNEL;
					#elif defined(SPOT)//聚光灯的光照衰减
						float4 lightcoord = mul(unity_WorldToLight, float4(i.worldPos, 1));
						/*(lightCoord.z > 0)：聚光灯的深度值小于等于0时，则光照衰减为0
						_LightTextureB0：如果该光源使用了cookie，则衰减查找纹理则为_LightTextureB0*/
						atten = (lightcoord.z > 0) * tex2D(_LightTexture0, lightcoord.xy / lightcoord.w + 0.5).w * tex2D(_LightTextureB0, dot(lightcoord, lightcoord).rr).UNITY_ATTEN_CHANNEL;
					#else
						atten = 1;
					#endif
				#endif

				fixed4 col = fixed4((Diff + Specul)*atten, 1);
				return col;
			}
			ENDCG
		}
		pass
		{
			//剔除前面 （朝向摄像机的面）保留内部渲染
			Cull Front
			CGPROGRAM
			#pragma fragment frag

			float4 _LineColor;
			float _Line;
			struct a2v {
				float4 pos:POSITION;
				float3 normal:NORMAL;
			};
			struct v2f {
				float4 pos:POSITION;
			};
			v2f vert(a2v i)
			{
				v2f v;
				UNITY_INITIALIZE_OUTPUT(v2f, v);
				//UNITY_MATRIX_MV模型视图矩阵转置
				float4 pos = float4(UnityObjectToViewPos(i.pos), 1);
				//UNITY_MATRIX_IT_MV中,模型视图逆转置,加IT是专门用来变换法线用的，将模型空间转换到相机空间
				float3 normal = mul((float3x3)UNITY_MATRIX_IT_MV, i.normal);
				normal.z = -0.5f;
				pos = pos + float4(normalize(normal), 0)*_Line;//法线向外扩展向量*线宽，得到轮廓的宽度
				//在将其转换到裁剪空间
				v.pos = mul(UNITY_MATRIX_P, pos);
				return v;
			}
			fixed4 frag(v2f i) :SV_Target
			{
				//返回纯色
				return fixed4(_LineColor.rgb, 1);
			}
			ENDCG
		}
	}
	FallBack "Specular"
}

