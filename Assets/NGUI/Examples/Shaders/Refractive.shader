Shader "Transparent/Refractive"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
        _BumpMap ("Normal Map (RGB)", 2D) = "bump" {}
        _Mask ("Specularity (R), Shininess (G), Refraction (B)", 2D) = "black" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Specular ("Specular Color", Color) = (0,0,0,0)
        _Focus ("Focus", Range(-100.0, 100.0)) = -100.0
        _Shininess ("Shininess", Range(0.01, 1.0)) = 0.2
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        GrabPass
        {
            Name "BASE"
        }

        Cull Off
        ZWrite Off
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Greater 0

        CGPROGRAM
        #pragma target 3.0
        #pragma exclude_renderers gles
        #pragma vertex vert
        #pragma surface surf BlinnPhong alpha
        #include "UnityCG.cginc"

        sampler2D _GrabTexture, _MainTex, _BumpMap, _Mask;
        fixed4 _Color, _Specular;
        half4 _GrabTexture_TexelSize;
        half _Focus, _Shininess;

        struct Input
        {
            float2 uv_MainTex : TEXCOORD0;
            float4 proj : TEXCOORD1;
        };

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.uv_MainTex = v.texcoord.xy;
            float4 pos = UnityObjectToClipPos(v.vertex);
            o.proj = float4((pos.xy / pos.w + 1.0) * 0.5, pos.z, pos.w);
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            half4 tex = tex2D(_MainTex, IN.uv_MainTex);
            half3 nm = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            half3 mask = tex2D(_Mask, IN.uv_MainTex);

            float2 offset = nm.xy * _GrabTexture_TexelSize.xy * _Focus;
            half4 ref = tex2Dproj(_GrabTexture, float4(IN.proj.xy + offset, IN.proj.z, IN.proj.w));

            half4 col;
            col.rgb = lerp(tex.rgb, _Color.rgb * ref.rgb, mask.b);
            col.a = tex.a * _Color.a;

            o.Albedo = col.rgb;
            o.Normal = nm;
            o.Specular = mask.r;
            o.Gloss = _Shininess * mask.g;
            o.Alpha = col.a;
        }

        ENDCG
    }
    Fallback Off
}