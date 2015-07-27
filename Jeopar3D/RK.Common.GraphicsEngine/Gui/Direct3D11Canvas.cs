using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows.Forms;
using RK.Common.GraphicsEngine.Core;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.GraphicsEngine.Drawing3D.Resources;
using RK.Common.Util;

using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace RK.Common.GraphicsEngine.Gui
{
    public partial class Direct3D11Canvas : Control
    {
        //Resources for DXGI (DirectX Graphics Interface)
        //private DXGI.Surface m_renderTarget2DDxgi;

        private bool m_initialized;
        private bool m_isOnRendering;
        //private bool m_isWireframeEnabled;
        //private bool m_updateOnRender;
        private RenderLoop m_renderLoop;

        //Resources for Direct3D 11
        //private Exception m_initializationException;
        private DXGI.Factory m_factory;
        private DXGI.SwapChain m_swapChain;
        private D3D11.Device m_renderDevice;
        private D3D11.DeviceContext m_renderDeviceContext;
        //private BackbufferCopiedTextureSource m_backBufferSource;
        private D3D11.RenderTargetView m_renderTarget;
        private D3D11.DepthStencilView m_renderTargetDepth;
        private D3D11.Texture2D m_backBuffer;
        private D3D11.Texture2D m_depthBuffer;

        //Generic members
        //private ActivityPerformanceValueContainer m_performanceCalculator;
        //private RenderState m_renderState;
        //private DateTime m_lastRenderTime;
        private Timer m_renderTimer;
        private Brush m_backBrush;
        //private bool m_renderTimerInvokedRendering;
        //private bool m_wireframeMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct3D11Panel"/> class.
        /// </summary>
        public Direct3D11Canvas()
        {
            //Set style parameters for this control
            base.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            base.SetStyle(ControlStyles.ResizeRedraw, true);
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            base.SetStyle(ControlStyles.Opaque, true);
            base.DoubleBuffered = false;

            //Create the render loop
            m_renderLoop = new Core.RenderLoop(
                OnRenderLoopCreateViewResources,
                OnRenderLoopDisposeViewResources,
                OnRenderLoopCheckCanRender,
                OnRenderLoopPrepareRendering,
                OnRenderLoopAfterRendering,
                OnRenderLoopPresent);
            m_renderLoop.IsWireframeEnabled = this.IsWireframeEnabled;
            m_renderLoop.UpdateOnRender = this.UpdateOnRender;

            //Observe resize event and throttle them
            Observable.FromEventPattern(this, "Resize")
                .Throttle(TimeSpan.FromSeconds(0.5))
                .ObserveOn(this)
                .Subscribe((eArgs) => OnThrottledResize());

            //Initialize background brush
            m_backBrush = new SolidBrush(this.BackColor);

            ////Preparse backbuffer source for CopiedTextureResources
            //m_backBufferSource = new BackbufferCopiedTextureSource(this);

            //Prepare render time object
            m_renderTimer = new Timer();
            m_renderTimer.Interval = 25;
            m_renderTimer.Tick += OnRenderTimerTick;
        }

        /// <summary>
        /// Saves a screenshot to harddisc.
        /// </summary>
        /// <param name="targetFile">Target file path.</param>
        /// <param name="fileFormat">Target file format.</param>
        public void SaveScreenshot(string targetFile, D3D11.ImageFileFormat fileFormat)
        {
            if (m_backBuffer != null)
            {
                D3D11.Texture2D.ToFile(
                    m_renderDeviceContext,
                    m_backBuffer,
                    fileFormat,
                    targetFile);
            }
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        private Tuple<D3D11.RenderTargetView, D3D11.DepthStencilView, D3D11.Viewport> OnRenderLoopCreateViewResources()
        {
            int width = this.Width;
            int height = this.Height;

            //Get all factories
            m_factory = GraphicsCore.Current.HandlerDXGI.Factory;

            //Get all devices
            m_renderDevice = GraphicsCore.Current.HandlerD3D11.Device;
            m_renderDeviceContext = m_renderDevice.ImmediateContext;

            //Get references to current render device
            D3D11.Device renderDevice = GraphicsCore.Current.HandlerD3D11.Device;
            D3D11.DeviceContext renderDeviceContext = renderDevice.ImmediateContext;

            //Create the swap chain and the render target
            m_swapChain = GraphicsHelper.CreateDefaultSwapChain(this, m_factory, m_renderDevice);
            m_backBuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);
            m_renderTarget = new D3D11.RenderTargetView(m_renderDevice, m_backBuffer);

            //Create the depth buffer
            m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(m_renderDevice, this.Width, this.Height);
            m_renderTargetDepth = new D3D11.DepthStencilView(m_renderDevice, m_depthBuffer);

            //Define the viewport for rendering
            D3D11.Viewport viewPort = GraphicsHelper.CreateDefaultViewport(this.Width, this.Height);

            //Return all generated objects
            return Tuple.Create(m_renderTarget, m_renderTargetDepth, viewPort);
        }

        /// <summary>
        /// Disposes all loaded view resources.
        /// </summary>
        private void OnRenderLoopDisposeViewResources()
        {
            m_factory = null;
            m_renderDevice = null;
            m_renderDeviceContext = null;

            m_renderTargetDepth = GraphicsHelper.DisposeGraphicsObject(m_renderTargetDepth);
            m_depthBuffer = GraphicsHelper.DisposeGraphicsObject(m_depthBuffer);
            m_renderTarget = GraphicsHelper.DisposeGraphicsObject(m_renderTarget);
            m_backBuffer = GraphicsHelper.DisposeGraphicsObject(m_backBuffer);
            m_swapChain = GraphicsHelper.DisposeGraphicsObject(m_swapChain);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        private bool OnRenderLoopCheckCanRender()
        {
            if (!m_initialized) { return false; }
            if (this.Width <= 0) { return false; }
            if (this.Height <= 0) { return false; }
            if (m_isOnRendering) { return false; }

            return true;
        }

        private void OnRenderLoopPrepareRendering()
        {
            m_isOnRendering = true;
        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        private void OnRenderLoopPresent()
        {
            //Present all rendered stuff on screen
            m_swapChain.Present(0, DXGI.PresentFlags.None);
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        private void OnRenderLoopAfterRendering()
        {
            m_isOnRendering = false;
        }

        ///// <summary>
        ///// Recreates all view resources.
        ///// </summary>
        //private void RecreateViewResources()
        //{
        //    UnloadViewResources();

        //    if ((this.Width > 0) && (this.Height > 0))
        //    {
        //        CreateViewResources();
        //    }
        //}

        /// <summary>
        /// Initialize 3d graphics with direct3D 11.
        /// </summary>
        public void Initialize3D()
        {
            //try
            //{
                //Create the RenderLoop object
            if((this.Width > 0) && (this.Height > 0))
            {
                m_renderLoop.RefreshViewResources();
            }

                ////Creates swap chain and render target
                //if ((this.Width > 0) && (this.Height > 0))
                //{
                //    CreateViewResources();
                //}

                //Update local init-flag
                m_initialized = true;
                //m_initializationException = null;
                this.DoubleBuffered = false;

                ////Remember current time
                //m_lastRenderTime = DateTime.Now;
            //}
            //catch (Exception ex)
            //{
            //    UnloadViewResources();

            //    //Update local init-flag
            //    // -> Error message is displayed in OnPaint method
            //    m_initialized = false;
            //    m_initializationException = ex;
            //    this.DoubleBuffered = true;
            //}
        }

        /// <summary>
        /// Unload 3d graphics.
        /// </summary>
        private void Unload3D()
        {
            if (m_initialized)
            {
                ////Unload all resources of the scene
                //m_scene.UnloadResources();

                //Unload all view resources
                m_renderLoop.UnloadViewResources();

                //Update initialization flags
                m_initialized = false;
            }
        }

        ///// <summary>
        ///// Called when Direct2D rendering should be done.
        ///// </summary>
        ///// <param name="renderTarget">The render-target to use.</param>
        //protected internal virtual void OnDirect3DPaint(RenderState renderState, UpdateState updateState)
        //{
        //    //renderTarget.BeginDraw();
        //    //try
        //    //{
        //    //    renderTarget.Clear(new Color4(this.BackColor));

        //    //    //Call external painting
        //    //    RaiseDirect2DPaint(m_graphics);
        //    //}
        //    //finally
        //    //{
        //    //    renderTarget.EndDraw();
        //    //}
        //}

        /// <summary>
        /// Called when system wants to paint this panel.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_initialized)
            {
                m_renderLoop.Render();
            }

            //if (m_renderState == null) { return; }

            //if (m_initialized && (this.Width > 0) && (this.Height > 0))
            //{
            //    //Just render current clipping area
            //    m_renderDeviceContext.Rasterizer.SetScissorRectangle(e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Right, e.ClipRectangle.Bottom);

            //    //Set default rastarization state
            //    D3D11.RasterizerState rasterState = null;
            //    if (m_wireframeMode)
            //    {
            //        rasterState = new D3D11.RasterizerState(m_renderDevice, new D3D11.RasterizerStateDescription()
            //        {
            //            CullMode = D3D11.CullMode.Back,
            //            FillMode = D3D11.FillMode.Wireframe,
            //            IsFrontCounterClockwise = false,
            //            DepthBias = 0,
            //            SlopeScaledDepthBias = 0f,
            //            DepthBiasClamp = 0f,
            //            IsDepthClipEnabled = true,
            //            IsAntialiasedLineEnabled = false,
            //            IsMultisampleEnabled = false,
            //            IsScissorEnabled = false
            //        });
            //        m_renderDeviceContext.Rasterizer.State = rasterState;
            //    }

            //    //Get update time
            //    DateTime currentTime = DateTime.Now;
            //    TimeSpan updateTime = currentTime - m_lastRenderTime;
            //    if (updateTime.TotalMilliseconds > 100.0) { updateTime = TimeSpan.FromMilliseconds(100.0); }
            //    m_lastRenderTime = currentTime;
            //    UpdateState updateState = new UpdateState(updateTime);

            //    //Apply current target
            //    m_renderState.ApplyCurrentTarget();

            //    //Paint using Direct3D
            //    m_renderDeviceContext.ClearRenderTargetView(m_renderTarget, Color4.CornflowerBlue.ToDXColor());//new SharpDX.Color4(this.BackColor.ToArgb()));
            //    m_renderDeviceContext.ClearDepthStencilView(m_renderTargetDepth, D3D11.DepthStencilClearFlags.Depth, 1f, 0);

            //    //Raise events
            //    RaiseBeforeUpdating(updateState);
            //    RaiseBeforeRendering();

            //    //Call render methods of subclasses
            //    OnDirect3DPaint(m_renderState, updateState);

            //    //Raise AfterRendering event
            //    RaiseAfterRendering();

            //    //Present all rendered stuff on screen
            //    m_swapChain.Present(0, DXGI.PresentFlags.None);

            //    //Clear current state after rendering
            //    m_renderState.ClearState();

            //    //Raises the TextureChanged event
            //    m_backBufferSource.RaiseTextureChanged(m_renderState);

            //    if (m_wireframeMode)
            //    {
            //        m_renderDeviceContext.Rasterizer.State = null;
            //        rasterState.Dispose();
            //    }
            //}
            //else
            //{
            //    //Paint using System.Drawing
            //    e.Graphics.FillRectangle(m_backBrush, e.ClipRectangle);

            //    //Display initialization exception (if any)
            //    if (m_initializationException != null)
            //    {
            //        e.Graphics.DrawString("Error during initialization of 3D graphics!", this.Font, Brushes.Black, new PointF(10f, 10f));
            //        e.Graphics.DrawString(m_initializationException.Message, this.Font, Brushes.Black, new PointF(10f, 25f));
            //    }
            //}

            //m_renderTimerInvokedRendering = false;
        }

        /// <summary>
        /// Called when BackColor property has changed.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            //Update background brush
            if (m_backBrush != null) { m_backBrush.Dispose(); }
            m_backBrush = new SolidBrush(this.BackColor);
        }

        /// <summary>
        /// Called when the control has changed its size.
        /// </summary>
        private void OnThrottledResize()
        {
            if ((!this.DesignMode) && m_initialized)
            {
                if ((this.Width > 0) && (this.Height > 0))
                {
                    m_renderLoop.RefreshViewResources();
                }
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if ((this.Width > 0) && (this.Height > 0))
            {
                m_renderLoop.Camera.SetScreenSize(this.Width, this.Height);
            }
        }

        /// <summary>
        /// Called when render timer ticks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnRenderTimerTick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        /// <summary>
        /// Called when the window handle is created.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if ((!this.DesignMode) && (!m_initialized))
            {
                //Initialize 3d graphics after panel creation
                Initialize3D();
            }
        }

        /// <summary>
        /// Called when the window handle is destroyed.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if ((!this.DesignMode) && m_initialized)
            {
                //Unload 3d graphics 
                Unload3D();
            }
        }

        ///// <summary>
        ///// Raises the BeforeUpdating event.
        ///// </summary>
        //private void RaiseBeforeUpdating(UpdateState updateState)
        //{
        //    if (BeforeUpdating != null) { BeforeUpdating(this, new Updating3DArgs(updateState)); }
        //}

        ///// <summary>
        ///// Raises the BeforeRendering event.
        ///// </summary>
        //private void RaiseBeforeRendering()
        //{
        //    if (BeforeRendering != null) { BeforeRendering(this, new Rendering3DArgs(m_renderState)); }
        //}

        ///// <summary>
        ///// Raises the AfterRendering event.
        ///// </summary>
        //private void RaiseAfterRendering()
        //{
        //    if (AfterRendering != null) { AfterRendering(this, new Rendering3DArgs(m_renderState)); }
        //}

        /// <summary>
        /// Is graphics system initialized?
        /// </summary>
        [Browsable(false)]
        public bool Initialized
        {
            get { return m_initialized; }
        }

        ///// <summary>
        ///// Gets the back buffer source.
        ///// </summary>
        //public ICopiedTextureSource BackBufferSource
        //{
        //    get { return m_backBufferSource; }
        //}

        /// <summary>
        /// Is wireframe enabled?
        /// </summary>
        [Category("Rendering")]
        public bool IsWireframeEnabled
        {
            get { return m_renderLoop.IsWireframeEnabled; }
            set { m_renderLoop.IsWireframeEnabled = value; }
        }

        /// <summary>
        /// Discard rendering?
        /// </summary>
        [Category("Rendering")]
        public bool DiscardRendering
        {
            get { return m_renderLoop.DiscardRendering; }
            set { m_renderLoop.DiscardRendering = value; }
        }

        [Category("Rendering")]
        public bool UpdateOnRender
        {
            get { return m_renderLoop.UpdateOnRender; }
            set { m_renderLoop.UpdateOnRender = value; }
        }

        [Category("Rendering")]
        public Scene Scene
        {
            get { return m_renderLoop.Scene; }
            set { m_renderLoop.Scene = value; }
        }

        [Browsable(false)]
        public RK.Common.GraphicsEngine.Drawing3D.Resources.ResourceDictionary Resources3D
        {
            get { return m_renderLoop.Scene.Resources; }
        }

        [Category("Rendering")]
        public Camera Camera
        {
            get { return m_renderLoop.Camera; }
            set { m_renderLoop.Camera = value; }
        }

        public ActivityPerformanceValueContainer PerformanceCalculator
        {
            get { return m_renderLoop.PerformanceCalculator; }
        }

        /// <summary>
        /// Is cyclic rendering enabled?
        /// </summary>
        [Category("Rendering")]
        [DefaultValue(false)]
        public bool CyclicRendering
        {
            get { return m_renderTimer.Enabled; }
            set
            {
                if (value != CyclicRendering)
                {
                    if (!m_renderTimer.Enabled) { m_renderTimer.Start(); }
                    else { m_renderTimer.Stop(); }
                }
            }
        }

        //*********************************************************************
        //*********************************************************************
        //*********************************************************************
        private class BackbufferCopiedTextureSource : ICopiedTextureSource
        {
            private Direct3D11Canvas m_owner;

            public event TextureChangedHandler TextureChanged;

            /// <summary>
            /// Initializes a new instance of the <see cref="BackbufferCopiedTextureSource"/> class.
            /// </summary>
            /// <param name="owner">The owner.</param>
            public BackbufferCopiedTextureSource(Direct3D11Canvas owner)
            {
                m_owner = owner;
            }

            /// <summary>
            /// Raises the TextureChanged event.
            /// </summary>
            /// <param name="renderState">Current render state.</param>
            public void RaiseTextureChanged(RenderState renderState)
            {
                if (TextureChanged != null) { TextureChanged(this, new TextureChangedEventArgs(renderState)); }
            }

            /// <summary>
            /// Gets the texture.
            /// </summary>
            public D3D11.Texture2D Texture
            {
                get { return m_owner.m_backBuffer; }
            }

            /// <summary>
            /// Gets the width of the texture.
            /// </summary>
            public int TextureWidth
            {
                get { return m_owner.Width; }
            }

            /// <summary>
            /// Gets the height of the texture.
            /// </summary>
            public int TextureHeight
            {
                get { return m_owner.Height; }
            }
        }
    }
}
