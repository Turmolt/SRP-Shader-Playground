Shader "Unlit/Raymarch"
{
    Properties
    {
        _ObjectPosition ("Position", Vector) = (0, 0, 0, 0)
        _YRot ("Y Rotation", float) = 0.0
        _Speech ("Speech", float) = 0.0
        _Offset("Offset", Vector) = (0.05, 0.01, 0.065)
        _InnerColor ("Inner Color", Color) = (0.2,0.5,1,1)
        _OuterColor ("Outer Color", Color) = (1, 0, 1, 1)
        _ExternalTime("Time", float)=0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 wPos : TEXCOORD1;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            float3 _ObjectPosition;
            float _YRot;
            float _Speech;

            #include "Assets/Shaders/RayFunctions.cginc"
            
            float _ExternalTime;
            fixed4 _InnerColor;
            fixed4 _OuterColor;
            float3 _Offset;
            
            fixed4 frag (v2f i) : SV_Target
            {
                sphere=1;
                yrot = _YRot;
                time=_ExternalTime;
                float3 worldPos = i.wPos + _ObjectPosition;
                float3 viewDir = normalize(i.wPos - _WorldSpaceCameraPos);
                float2 scene = raymarch(worldPos, viewDir);
                float3 sun = _WorldSpaceLightPos0.xyz;
                
                float3 p = worldPos + viewDir * scene.y;
                float3 norm = normal(p);
                
                float dif = clamp(dot(norm, normalize(sun)), 0.0, 1.0);
                dif+=0.5;
                dif=saturate(dif);
                fixed4 col =  scene.x >0 ?fixed4(0.2,0.5,0.9,scene.x):.5/(scene.y*6.0);

                float shadowInfluence = 0.8;
                col.a = scene.x;
                col.rgb*=((1-dif)*shadowInfluence + (1-shadowInfluence));
                col = clamp(col+ (0.65 + _Speech/2.0)*pow(glow/100.,1.2)*lerp(_InnerColor,_OuterColor,saturate(scene.y/30.)), 0., 2.);
                col.a = saturate(col.a);
                return col;
            }
            ENDCG
        }
    }
}
