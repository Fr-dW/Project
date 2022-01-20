// UNITY_SHADER_NO_UPGRADE
Shader "Custom/SimpleColor" 
{
    Properties
    {
        _Color("Changer Couleur",Color) = (1,1,1,1)
        _MyTexture("Changer Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "LightMode" = "ForwardBase"
        }
       
        Pass
        {
            Lighting On
            CGPROGRAM
            
            #pragma vertex main_vert
            #pragma fragment main_frag
            #include "UnityLightingCommon.cginc"
            #include "UnityCG.cginc"
            

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4  pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 posW : TEXCOORD1;
                float4 dirLight : TEXCOORD2;
            };

            v2f main_vert( a2v v)
            {
                v2f vert;
                vert.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                vert.uv = v.uv;
                vert.normal = v.normal;
                vert.posW = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;

                float3 vecLight = (_WorldSpaceLightPos0).xyz;
                float3 dir = normalize(vecLight);
                vert.dirLight = _LightColor0 * max(0.0, dot(dir, normalize(vert.normal)));
                
                return vert;
            }

            uniform float4 _Color;
            sampler2D _MyTexture;

            half4 main_frag(v2f i) : COLOR
            {
                fixed4 colortex = tex2D(_MyTexture,i.uv);
                fixed4 colorambient = (_Color *UNITY_LIGHTMODEL_AMBIENT * 2 );
                return colortex;
                /*float3 vecLight = (_WorldSpaceLightPos0).xyz; // si c'est une lumière direction; alors on a la direction et non la position     
                //float dist = dot(vecLight,vecLight); seulement besoin si on est un mode position
                float3 dir = normalize(vecLight);
                float4 dirLight = _LightColor0 * max(0.0, dot(dir, normalize(i.normal)));

                return _Color * colortex * dirLight;*/
                
                /*
                fixed4 colortex = tex2D(_MyTexture,i.uv)* _LightColor0;
                return colortex;*/
               // return _Color;//float4(0.5,1.0,1.0,1.0);
            }
            ENDCG
        }




    }
}
