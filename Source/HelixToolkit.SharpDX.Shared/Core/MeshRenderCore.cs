﻿/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;

    public class MeshRenderCore : MaterialGeometryRenderCore, IInvertNormal
    {
        public bool InvertNormal { set; get; } = false;

        public string ShaderShadowMapTextureName { set; get; } = DefaultBufferNames.ShadowMapTB;

        private int shadowMapSlot;

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            model.InvertNormal = InvertNormal ? 1 : 0;
        }

        protected override void OnDefaultPassChanged(IShaderPass pass)
        {
            base.OnDefaultPassChanged(pass);
            pass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(ShaderShadowMapTextureName);
        }

        protected override void OnRender(IRenderContext context)
        {                  
            DefaultShaderPass.BindShader(context.DeviceContext);
            DefaultShaderPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            if(!BindMaterialTextures(context.DeviceContext, DefaultShaderPass))
            {
                return;
            }
            if (context.RenderHost.IsShadowMapEnabled)
            {
                DefaultShaderPass.GetShader(ShaderStage.Pixel).BindTexture(context.DeviceContext, ShaderShadowMapTextureName, context.SharedResource.ShadowView);
            }
            OnDraw(context.DeviceContext, InstanceBuffer);
        }

        protected override void OnRenderShadow(IRenderContext context)
        {
            if (!IsThrowingShadow)
            {
                return;
            }
            ShadowPass.BindShader(context.DeviceContext);
            ShadowPass.BindStates(context.DeviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(context.DeviceContext, InstanceBuffer);
        }
    }
}
