using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Drawing3D.Resources;

using D3D11 = SharpDX.Direct3D11;

namespace RK.Common.GraphicsEngine.Core
{
    public class RenderPassDefaultTransparent : RenderPassBase
    {
        private D3D11.BlendState m_blendState;
        private D3D11.DepthStencilState m_depthStencilState;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPassDefaultTransparent" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public RenderPassDefaultTransparent(string name)
            : base(name)
        {

        }

        /// <summary>
        /// Applies this RenderPass (called before starting rendering first objects with it).
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        public override void Apply(RenderState renderState)
        {
            D3D11.DeviceContext deviceContext = renderState.DeviceContext;

            deviceContext.OutputMerger.BlendState = m_blendState;
            deviceContext.OutputMerger.DepthStencilState = m_depthStencilState;
        }

        /// <summary>
        /// Discards this RenderPass (called after rendering all objects of this pass).
        /// </summary>
        /// <param name="renderState">The current render state.</param>
        public override void Discard(RenderState renderState)
        {
            D3D11.DeviceContext deviceContext = renderState.DeviceContext;

            deviceContext.OutputMerger.BlendState = null;
            deviceContext.OutputMerger.DepthStencilState = null;
        }

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void LoadResourceInternal(ResourceDictionary resources)
        {
            D3D11.Device device = GraphicsCore.Current.HandlerD3D11.Device;

            //Define the blend state (based on http://www.rastertek.com/dx11tut26.html)
            D3D11.BlendStateDescription blendDesc = D3D11.BlendStateDescription.Default();
            blendDesc.RenderTarget[0].IsBlendEnabled = true;
            blendDesc.RenderTarget[0].SourceBlend = D3D11.BlendOption.SourceAlpha;
            blendDesc.RenderTarget[0].DestinationBlend = D3D11.BlendOption.InverseSourceAlpha;
            blendDesc.RenderTarget[0].BlendOperation = D3D11.BlendOperation.Add;
            blendDesc.RenderTarget[0].SourceAlphaBlend = D3D11.BlendOption.One;
            blendDesc.RenderTarget[0].DestinationAlphaBlend = D3D11.BlendOption.Zero;
            blendDesc.RenderTarget[0].AlphaBlendOperation = D3D11.BlendOperation.Add;
            blendDesc.RenderTarget[0].RenderTargetWriteMask = D3D11.ColorWriteMaskFlags.All;

            //Create the blendstate object
            m_blendState = new D3D11.BlendState(device, blendDesc);

            //Create depth stencil state (
            m_depthStencilState = new D3D11.DepthStencilState(device, new D3D11.DepthStencilStateDescription()
            {
                BackFace = new D3D11.DepthStencilOperationDescription()
                {
                    Comparison = D3D11.Comparison.Never,
                    DepthFailOperation = D3D11.StencilOperation.Keep,
                    FailOperation = D3D11.StencilOperation.Keep,
                    PassOperation = D3D11.StencilOperation.Keep
                },
                FrontFace = new D3D11.DepthStencilOperationDescription()
                {
                    Comparison = D3D11.Comparison.Never,
                    DepthFailOperation = D3D11.StencilOperation.Keep,
                    FailOperation = D3D11.StencilOperation.Keep,
                    PassOperation = D3D11.StencilOperation.Keep
                },
                DepthComparison = D3D11.Comparison.Less,
                IsDepthEnabled = true,
                DepthWriteMask = D3D11.DepthWriteMask.Zero,
                IsStencilEnabled = false,
                StencilReadMask = 0,
                StencilWriteMask = 0,
            });
        }

        /// <summary>
        /// Unloads the resource.
        /// </summary>
        /// <param name="resources">Parent ResourceDictionary.</param>
        protected override void UnloadResourceInternal(ResourceDictionary resources)
        {
            m_blendState = GraphicsHelper.DisposeGraphicsObject(m_blendState);
            m_depthStencilState = GraphicsHelper.DisposeGraphicsObject(m_depthStencilState);
        }

        /// <summary>
        /// Is the resource loaded?
        /// </summary>
        public override bool IsLoaded
        {
            get { return m_blendState != null; }
        }
    }
}
