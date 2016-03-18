// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:1,cusa:False,bamd:0,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:7396,x:33006,y:32869,varname:node_7396,prsc:2|emission-4009-RGB;n:type:ShaderForge.SFN_Tex2d,id:4009,x:32766,y:32973,ptovrint:False,ptlb:Render Texture,ptin:_RenderTexture,varname:node_4009,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:05e6340869ed40747a14cf93bcacabd1,ntxv:0,isnm:False|UVIN-8708-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4024,x:31326,y:32666,ptovrint:False,ptlb:Pixel Width,ptin:_PixelWidth,varname:node_4024,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_ValueProperty,id:9484,x:31351,y:33320,ptovrint:False,ptlb:Pixel Height,ptin:_PixelHeight,varname:node_9484,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Floor,id:3920,x:32071,y:32914,varname:node_3920,prsc:2|IN-388-OUT;n:type:ShaderForge.SFN_Multiply,id:2341,x:32260,y:32860,varname:node_2341,prsc:2|A-1521-OUT,B-3920-OUT;n:type:ShaderForge.SFN_Divide,id:388,x:31872,y:32938,varname:node_388,prsc:2|A-4647-U,B-1521-OUT;n:type:ShaderForge.SFN_TexCoord,id:4647,x:31605,y:32934,varname:node_4647,prsc:2,uv:0;n:type:ShaderForge.SFN_Append,id:8708,x:32510,y:33004,varname:node_8708,prsc:2|A-2341-OUT,B-9318-OUT;n:type:ShaderForge.SFN_Floor,id:3698,x:32039,y:33194,varname:node_3698,prsc:2|IN-5531-OUT;n:type:ShaderForge.SFN_Multiply,id:9318,x:32238,y:33151,varname:node_9318,prsc:2|A-5039-OUT,B-3698-OUT;n:type:ShaderForge.SFN_Divide,id:5531,x:31880,y:33240,varname:node_5531,prsc:2|A-4647-V,B-5039-OUT;n:type:ShaderForge.SFN_ScreenParameters,id:7606,x:31019,y:33000,varname:node_7606,prsc:2;n:type:ShaderForge.SFN_Divide,id:922,x:31455,y:33122,varname:node_922,prsc:2|A-8011-OUT,B-7606-PXH;n:type:ShaderForge.SFN_Multiply,id:5039,x:31645,y:33206,varname:node_5039,prsc:2|A-9484-OUT,B-922-OUT;n:type:ShaderForge.SFN_Vector1,id:8011,x:31246,y:33013,varname:node_8011,prsc:2,v1:1;n:type:ShaderForge.SFN_Divide,id:1129,x:31392,y:32879,varname:node_1129,prsc:2|A-8450-OUT,B-7606-PXW;n:type:ShaderForge.SFN_Vector1,id:8450,x:31165,y:32719,varname:node_8450,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1521,x:31554,y:32734,varname:node_1521,prsc:2|A-4024-OUT,B-1129-OUT;proporder:4009-4024-9484;pass:END;sub:END;*/

Shader "ImageEffect/Pixelate" {
    Properties {
        _RenderTexture ("Render Texture", 2D) = "white" {}
        _PixelWidth ("Pixel Width", Float ) = 1
        _PixelHeight ("Pixel Height", Float ) = 1
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform sampler2D _RenderTexture; uniform float4 _RenderTexture_ST;
            uniform float _PixelWidth;
            uniform float _PixelHeight;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_1521 = (_PixelWidth*(1.0/_ScreenParams.r));
                float node_5039 = (_PixelHeight*(1.0/_ScreenParams.g));
                float2 node_8708 = float2((node_1521*floor((i.uv0.r/node_1521))),(node_5039*floor((i.uv0.g/node_5039))));
                float4 _RenderTexture_var = tex2D(_RenderTexture,TRANSFORM_TEX(node_8708, _RenderTexture));
                float3 emissive = _RenderTexture_var.rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
