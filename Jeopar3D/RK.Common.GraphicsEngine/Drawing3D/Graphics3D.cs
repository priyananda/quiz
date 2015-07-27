﻿using System;
using System.Collections.Generic;
using RK.Common.GraphicsEngine.Core;
using RK.Common.GraphicsEngine.Drawing3D.Resources;

using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using System.Runtime.InteropServices;
using SharpDX;

namespace RK.Common.GraphicsEngine.Drawing3D
{
    public class Graphics3D : IDisposable
    {
        private const int MAX_LINE_VERTEX_COUNT = 100;

        private bool m_disposed;
        private ResourceDictionary m_resourceDictionary;
        private RenderState m_renderState;

        //Specific resources
        private D3D11.Buffer m_lineVertexBuffer;
        private D3D11.InputLayout m_lineInputLayout;
        //private EffectResource m_lineRenderEffect;

        /// <summary>
        /// Initializes a new instance of the <see cref="Graphics3D"/> class.
        /// </summary>
        /// <param name="renderState">Current state of the renderer.</param>
        public Graphics3D(RenderState renderState)
        {
            m_resourceDictionary = new ResourceDictionary();
            m_renderState = renderState;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            if (m_disposed) { throw new ObjectDisposedException("Graphics3D"); }

            GraphicsHelper.SafeDispose(ref m_lineVertexBuffer);
            GraphicsHelper.SafeDispose(ref m_lineInputLayout);

            m_disposed = true;
        }

        /// <summary>
        /// Draws the given line.
        /// </summary>
        /// <param name="start">Start-Point of the line.</param>
        /// <param name="destination">Destination-Point of the line.</param>
        /// <param name="lineColor">Color of the line.</param>
        public void DrawLine(Vector3 start, Vector3 destination, Color4 lineColor)
        {
            DrawLines(new Vector3[] { start, destination }, lineColor);
        }

        /// <summary>
        /// Draws lines created by the given line creator function (e. g. VertexStructure.BuildLineListForFaceNormals).
        /// </summary>
        /// <param name="lineListCreator">Delegate targeting a line creator function.</param>
        /// <param name="lineColor">Color of the result.</param>
        public void DrawLines(System.Func<List<Vector3>> lineListCreator, Color4 lineColor)
        {
            DrawLines(lineListCreator().ToArray(), lineColor);
        }

        /// <summary>
        /// Draws the given list of lines.
        /// </summary>
        /// <param name="lines">List containing all lines.</param>
        /// <param name="lineColor">Color of the line.</param>
        public void DrawLines(Vector3[] lines, Color4 lineColor)
        {
            if (m_disposed) { throw new ObjectDisposedException("Graphics3D"); }

            //D3D11.Device device = m_renderState.Device;

            ////Load all resources
            //#region -----------------------------------------------------------
            ////Create vertex buffer if needed
            //if (m_lineVertexBuffer == null)
            //{
            //    m_lineVertexBuffer = GraphicsHelper.CreateDynamicVertexBuffer<StandardVertex>(device, MAX_LINE_VERTEX_COUNT);
            //}

            ////Load resource if needed
            //if (m_lineRenderEffect == null)
            //{
            //    m_lineRenderEffect = StandardResources.AddAndLoadResource<EffectResource>(m_resourceDictionary, StandardResources.SimpleRenderingEffectName);
            //}

            ////Create input layout for line rendering
            //if (m_lineInputLayout == null)
            //{
            //    m_lineInputLayout = new D3D11.InputLayout(device, m_lineRenderEffect.GetInputLayout("Render", "P0"), StandardVertex.InputElements);
            //}

            ////Write all vertices to temporary buffer
            //StandardVertex[] vertices = StandardVertex.FromLineList(lineColor, lines);
            //DataStream outStream = null;
            //device.ImmediateContext.MapSubresource(m_lineVertexBuffer, D3D11.MapMode.WriteDiscard, D3D11.MapFlags.None, out outStream);
            //outStream.WriteRange(vertices);
            //device.ImmediateContext.UnmapSubresource(m_lineVertexBuffer, 0);
            //#endregion

            ////Draw lines
            //#region -------------------------------------------------------
            //D3D11.DeviceContext deviceContext = m_renderState.DeviceContext;

            ////Set device state
            //deviceContext.InputAssembler.InputLayout = m_lineInputLayout;
            //deviceContext.InputAssembler.PrimitiveTopology = D3D.PrimitiveTopology.LineList;
            //deviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(m_lineVertexBuffer, Marshal.SizeOf(typeof(StandardVertex)), 0));

            ////Set shader state
            //Matrix4 worldViewProj =
            //    m_renderState.World.Top *
            //    m_renderState.ViewProj;
            //m_lineRenderEffect.SetVariableValue("WorldViewProj", worldViewProj);
            //m_lineRenderEffect.SetVariableValue("Ambient", 1f);
            //m_lineRenderEffect.SetVariableValue("StrongLightFactor", 1f);
            //m_lineRenderEffect.SetVariableValue("World", Matrix4.Identity);
            //m_lineRenderEffect.SetVariableValue("ObjectTexture", null);
            //m_lineRenderEffect.SetVariableValue("LightPosition", Vector3.Empty);
            //m_lineRenderEffect.SetVariableValue("LightPower", 1f);
            //m_lineRenderEffect.ApplyPass(deviceContext, "Render", "P0");

            //D3D11.RasterizerState rasterState = null;
            //if (GraphicsCore.Current.TargetHardware == TargetHardware.SoftwareRenderer)
            //{
            //    rasterState = new D3D11.RasterizerState(device, new D3D11.RasterizerStateDescription()
            //    {
            //        CullMode = D3D11.CullMode.Back,
            //        FillMode = D3D11.FillMode.Solid,
            //        IsFrontCounterClockwise = false,
            //        DepthBias = 0,
            //        SlopeScaledDepthBias = 0f,
            //        DepthBiasClamp = 0f,
            //        IsDepthClipEnabled = true,
            //        IsAntialiasedLineEnabled = false,
            //        IsMultisampleEnabled = false,
            //        IsScissorEnabled = false
            //    });
            //}
            //else
            //{
            //    rasterState = new D3D11.RasterizerState(device, new D3D11.RasterizerStateDescription()
            //    {
            //        CullMode = D3D11.CullMode.Back,
            //        FillMode = D3D11.FillMode.Solid,
            //        IsFrontCounterClockwise = false,
            //        DepthBias = 0,
            //        SlopeScaledDepthBias = 0f,
            //        DepthBiasClamp = 0f,
            //        IsDepthClipEnabled = true,
            //        IsAntialiasedLineEnabled = true,
            //        IsMultisampleEnabled = false,
            //        IsScissorEnabled = false
            //    });
            //}
            //D3D11.RasterizerState oldOne = deviceContext.Rasterizer.State;

            //deviceContext.Rasterizer.State = rasterState;

            ////Draw vertices
            //deviceContext.Draw(vertices.Length, 0);

            //deviceContext.Rasterizer.State = oldOne;
            //GraphicsHelper.DisposeGraphicsObject(rasterState);
            //#endregion
        }
    }
}
