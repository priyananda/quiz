using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using RK.Common.GraphicsEngine.Core;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.Util;
using D3D11 = SharpDX.Direct3D11;
using DXGI = SharpDX.DXGI;

namespace RK.Common.GraphicsEngine.Gui
{
    public partial class Direct3D11Image : Image
    {
        //public static readonly DependencyProperty IsWireframeEnabledProperty =
        //    DependencyProperty.Register("IsWireframeEnabled", typeof(bool), typeof(Direct3D11Image), new PropertyMetadata(false));
        //public static readonly DependencyProperty UpdateOnRenderProperty =
        //    DependencyProperty.Register("UpdateOnRender", typeof(bool), typeof(Direct3D11Image), new PropertyMetadata(false));

        //Some members..
        private bool m_initialized;
        private RenderLoop m_renderLoop;
        private HigherD3DImageSource m_d3dImageSource;

        //All needed direct3d resources
        private D3D11.Device m_device;
        private D3D11.DeviceContext m_deviceContext;
        private D3D11.Texture2D m_backBufferForWpf;
        private D3D11.Texture2D m_backBufferD3D11;
        private D3D11.Texture2D m_depthBuffer;
        private D3D11.RenderTargetView m_renderTarget;
        private D3D11.DepthStencilView m_renderTargetDepth;
        private DXGI.Surface m_renderTarget2DDxgi;

        //Some size related properties
        private int m_renderTargetHeight;
        private int m_renderTargetWidth;
        private int m_viewportHeight;
        private int m_viewportWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Direct3D11Image"/> class.
        /// </summary>
        public Direct3D11Image()
        {
            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;

            //Create the RenderLoop object
            m_renderLoop = new Core.RenderLoop(
                OnRenderLoopCreateViewResources,
                OnRenderLoopDisposeViewResources,
                OnRenderLoopCheckCanRender,
                OnRenderLoopPrepareRendering,
                OnRenderLoopAfterRendering,
                OnRenderLoopPresent);

            //Register all events needed for mouse camera dragging
            this.MouseWheel += OnMouseWheel;
            this.MouseDown += OnViewportGridMouseDown;
            this.MouseUp += OnViewportGridMouseUp;
            this.MouseMove += OnViewportGridMouseMove;
            this.MouseLeave += OnViewportGridMouseLeave;
            this.LostFocus += OnViewportGridLostFocus;
            this.PreviewMouseUp += OnViewportGridPreviewMouseUp;

            //Attach to SizeChanged event (refresh view resources only after a specific time)
            Observable.FromEventPattern<EventArgs>(this, "SizeChanged")
                .Throttle(TimeSpan.FromSeconds(0.5))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe((eArgs) => OnThrottledSizeChanged());

            //Configure render interval
            SynchronizationContext.Current.InvokeDelayedWhile(
                () => true,
                () => OnRenderTrigger(),
                TimeSpan.FromMilliseconds(18.0),
                () => { },
                InvokeDelayedMode.EnsuredTimerInterval);
        }

        /// <summary>
        /// Discard rendering?
        /// </summary>
        public bool DiscardRendering
        {
            get { return m_renderLoop.DiscardRendering; }
            set { m_renderLoop.DiscardRendering = value; }
        }

        public Scene Scene
        {
            get { return m_renderLoop.Scene; }
            set { m_renderLoop.Scene = value; }
        }

        public RK.Common.GraphicsEngine.Drawing3D.Resources.ResourceDictionary Resources3D
        {
            get { return m_renderLoop.Scene.Resources; }
        }

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
        /// Is wireframe enabled?
        /// </summary>
        public bool IsWireframeEnabled
        {
            get { return m_renderLoop.IsWireframeEnabled; }
            set { m_renderLoop.IsWireframeEnabled = value; }
        }

        public bool UpdateOnRender
        {
            get { return m_renderLoop.UpdateOnRender; }
            set { m_renderLoop.UpdateOnRender = value; }
        }

        public RenderLoop RenderLoop
        {
            get { return m_renderLoop; }
        }

        /// <summary>
        /// Initialization method (called when this control gets loaded).
        /// </summary>
        private void Initialize()
        {
            if (!GraphicsCore.IsInitialized) { return; }

            //Create the d3d image
            m_d3dImageSource = new HigherD3DImageSource();

            //Get device references
            m_device = GraphicsCore.Current.HandlerD3D11.Device;
            m_deviceContext = m_device.ImmediateContext;

            //Create all view resources
            m_renderLoop.RefreshViewResources();

            //Enable 3d rendering
            m_initialized = true;
            this.Source = m_d3dImageSource;
        }

        /// <summary>
        /// Is this object in design mode?
        /// </summary>
        /// <param name="dependencyObject">The object to check.</param>
        private bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(this);
        }

        /// <summary>
        /// Called when the render size has changed.
        /// </summary>
        /// <param name="sizeInfo">New size information.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (!GraphicsCore.IsInitialized) { return; }

            //Update render size
            m_renderLoop.Camera.SetScreenSize((int)this.RenderSize.Width, (int)this.RenderSize.Height);

            //Resize render target only on greater size changes
            if (m_initialized)
            {
                double resizeFactorWidth = sizeInfo.NewSize.Width > m_renderTargetWidth ? sizeInfo.NewSize.Width / m_renderTargetWidth : m_renderTargetWidth / sizeInfo.NewSize.Width;
                double resizeFactorHeight = sizeInfo.NewSize.Height > m_renderTargetHeight ? sizeInfo.NewSize.Height / m_renderTargetHeight : m_renderTargetHeight / sizeInfo.NewSize.Height;
                if ((resizeFactorWidth > 1.3) || (resizeFactorHeight > 1.3))
                {
                    m_renderLoop.RefreshViewResources();
                }
            }
        }

        /// <summary>
        /// Disposes all loaded view resources.
        /// </summary>
        private void OnRenderLoopDisposeViewResources()
        {
            m_d3dImageSource.SetRenderTarget(null);

            m_renderTarget2DDxgi = GraphicsHelper.DisposeGraphicsObject(m_renderTarget2DDxgi);
            m_renderTargetDepth = GraphicsHelper.DisposeGraphicsObject(m_renderTargetDepth);
            m_depthBuffer = GraphicsHelper.DisposeGraphicsObject(m_depthBuffer);
            m_renderTarget = GraphicsHelper.DisposeGraphicsObject(m_renderTarget);
            m_backBufferForWpf = GraphicsHelper.DisposeGraphicsObject(m_backBufferForWpf);
            m_backBufferD3D11 = GraphicsHelper.DisposeGraphicsObject(m_backBufferD3D11);
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        private Tuple<D3D11.RenderTargetView, D3D11.DepthStencilView, D3D11.Viewport> OnRenderLoopCreateViewResources()
        {
            int width = Math.Max((int)base.ActualWidth, 100);
            int height = Math.Max((int)base.ActualHeight, 100);

            //Get references to current render device
            D3D11.Device renderDevice = GraphicsCore.Current.HandlerD3D11.Device;
            D3D11.DeviceContext renderDeviceContext = renderDevice.ImmediateContext;

            //Create the swap chain and the render target
            m_backBufferD3D11 = GraphicsHelper.CreateRenderTargetTexture(renderDevice, width, height);
            m_backBufferForWpf = GraphicsHelper.CreateSharedTexture(renderDevice, width, height);
            m_renderTarget = new D3D11.RenderTargetView(renderDevice, m_backBufferD3D11);

            //Create the depth buffer
            m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(renderDevice, width, height);
            m_renderTargetDepth = new D3D11.DepthStencilView(renderDevice, m_depthBuffer);

            //Apply render target size values
            m_renderTargetWidth = width;
            m_renderTargetHeight = height;

            //Define the viewport for rendering
            D3D11.Viewport viewPort = GraphicsHelper.CreateDefaultViewport(width, height);

            //Apply new width and height values of the viewport
            m_viewportWidth = width;
            m_viewportHeight = height;

            //Apply render target on D3DImage object
            m_d3dImageSource.SetRenderTarget(m_backBufferForWpf);

            //Return all generated objects
            return Tuple.Create(m_renderTarget, m_renderTargetDepth, viewPort);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        private bool OnRenderLoopCheckCanRender()
        {
            if (m_d3dImageSource == null) { return false; }
            if (!m_d3dImageSource.IsFrontBufferAvailable) { return false; }
            if (!m_d3dImageSource.HasRenderTarget) { return false; }
            if (this.Width <= 0) { return false; }
            if (this.Height <= 0) { return false; }

            return true;
        }

        private void OnRenderLoopPrepareRendering()
        {
            m_renderLoop.PerformanceCalculator.MeasureActivityDuration("Render.Lock", () => m_d3dImageSource.Lock());
        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        private void OnRenderLoopPresent()
        {
            m_deviceContext.ResolveSubresource(m_backBufferD3D11, 0, m_backBufferForWpf, 0, DXGI.Format.B8G8R8A8_UNorm);
            m_deviceContext.Flush();
            m_deviceContext.ClearState();

            m_d3dImageSource.AddDirtyRect(new Int32Rect(0, 0, m_d3dImageSource.PixelWidth, m_d3dImageSource.PixelHeight));
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        private void OnRenderLoopAfterRendering()
        {
            m_d3dImageSource.Unlock();
        }

        //if (m_d3dImageSource == null) { Initialize(); }
        //if (!m_d3dImageSource.IsFrontBufferAvailable) { return; }
        //if (!m_d3dImageSource.HasRenderTarget) { return; }
        //if (this.Width <= 0) { return; }
        //if (this.Height <= 0) { return; }
        //if (!m_d3dImageSource.TryLock(new Duration(TimeSpan.Zero))) 
        //{
        //    m_d3dImageSource.Unlock();
        //    return; 
        //}
        ////m_performanceCalculator.MeasureActivityDuration("Render.Lock", () => m_d3dImageSource.Lock());

        //var renderTimeMeasurenment = m_performanceCalculator.BeginMeasureActivityDuration("Render.Complete");
        //try
        //{
        //    //Set default rastarization state
        //    D3D11.RasterizerState rasterState = null;
        //    bool wireframeEnabled = this.IsWireframeEnabled;
        //    if (wireframeEnabled)
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
        //    DateTime currentTime = DateTime.UtcNow;
        //    TimeSpan updateTime = currentTime - m_lastRenderTime;
        //    if (updateTime.TotalMilliseconds > 100.0) { updateTime = TimeSpan.FromMilliseconds(100.0); }
        //    m_lastRenderTime = currentTime;
        //    UpdateState updateState = new UpdateState(updateTime);

        //    //Apply current target
        //    m_renderState.ApplyCurrentTarget();
        //    m_renderState.ApplyMaterial(null);

        //    //Paint using Direct3D
        //    m_renderDeviceContext.ClearRenderTargetView(m_renderTarget, new SharpDX.Color(0, 0, 0, 0));
        //    m_renderDeviceContext.ClearDepthStencilView(m_renderTargetDepth, D3D11.DepthStencilClearFlags.Depth | D3D11.DepthStencilClearFlags.Stencil, 1f, 0);

        //    //Raise events
        //    RaiseBeforeUpdating(updateState);
        //    RaiseBeforeRendering();

        //    //Call render methods of subclasses
        //    OnDirect3DPaint(m_renderState, updateState);

        //    //Raise AfterRendering event
        //    RaiseAfterRendering();

        //    //Clear current state after rendering
        //    m_renderState.ClearState();

        //    if (wireframeEnabled)
        //    {
        //        m_renderDeviceContext.Rasterizer.State = null;
        //        rasterState.Dispose();
        //    }

        //    ////Raises the TextureChanged event
        //    //m_backBufferSource.RaiseTextureChanged(m_renderState);

        //    //Copy contents of direct3D 11 texture to wpf texture. This step makes following possible
        //    // => Move all rendering logic to a background thread and perform only following on gui thread
        //    using (var finishRenderTimeMeasurenment = m_performanceCalculator.BeginMeasureActivityDuration("Render.Finishing"))
        //    {
        //        m_renderDeviceContext.ResolveSubresource(m_backBufferD3D11, 0, m_backBufferForWpf, 0, DXGI.Format.B8G8R8A8_UNorm);
        //        m_renderDeviceContext.Flush();
        //        m_renderDeviceContext.ClearState();

        //        m_d3dImageSource.AddDirtyRect(new Int32Rect(0, 0, m_d3dImageSource.PixelWidth, m_d3dImageSource.PixelHeight));
        //    }
        //}
        //finally
        //{
        //    m_d3dImageSource.Unlock();
        //    renderTimeMeasurenment.Dispose();
        //}

        ///// <summary>
        ///// Creates all view resources.
        ///// </summary>
        //private void CreateViewResourcesOld()
        //{
        //    //Dispose resources of last swap chain
        //    UnloadViewResources();

        //    int width = Math.Max((int)base.ActualWidth, 100);
        //    int height = Math.Max((int)base.ActualHeight, 100);

        //    m_renderDevice = GraphicsCore.Current.HandlerD3D11.Device;
        //    m_renderDeviceContext = m_renderDevice.ImmediateContext;

        //    //Create the swap chain and the render target
        //    m_backBufferD3D11 = GraphicsHelper.CreateRenderTargetTexture(m_renderDevice, width, height);
        //    m_backBufferForWpf = GraphicsHelper.CreateSharedTexture(m_renderDevice, width, height);
        //    m_renderTarget = new D3D11.RenderTargetView(m_renderDevice, m_backBufferD3D11);

        //    //Create the depth buffer
        //    m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(m_renderDevice, width, height);
        //    m_renderTargetDepth = new D3D11.DepthStencilView(m_renderDevice, m_depthBuffer);

        //    //Apply render target size values
        //    m_renderTargetWidth = width;
        //    m_renderTargetHeight = height;

        //    //Recreate viewport
        //    RecreateViewPort();

        //    //Perform first render on the surface
        //    Render();

        //    //Set backbuffer to d3d image
        //    m_d3dImageSource.SetRenderTarget(m_backBufferForWpf);
        //}

        /// <summary>
        /// Called when the image is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!GraphicsCore.IsInitialized) { return; }

            if ((!this.IsInDesignMode()) &&
                (!m_initialized))
            {
                Initialize();
            }
        }

        /// <summary>
        /// Called when size changed event occurred.
        /// </summary>
        private void OnThrottledSizeChanged()
        {
            if (!GraphicsCore.IsInitialized) { return; }

            if (m_initialized)
            {
                m_renderLoop.RefreshViewResources();
            }
        }

        /// <summary>
        /// Called when the image is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (!GraphicsCore.IsInitialized) { return; }

            if ((!this.IsInDesignMode()) &&
                (m_initialized))
            {
                //Disable rendering
                m_initialized = false;
                this.Source = null;

                //Clear view resources
                m_renderLoop.UnloadViewResources();

                //Clear d3d image
                //m_d3dImageSource.IsFrontBufferAvailableChanged -= OnD3DImageIsFrontBufferAvailableChanged;
                GraphicsHelper.SafeDispose(ref m_d3dImageSource);

                m_device = null;
                m_deviceContext = null;
            }
        }

        ///// <summary>
        ///// Raises the AfterRendering event.
        ///// </summary>
        //private void RaiseAfterRendering()
        //{
        //    if (AfterRendering != null) { AfterRendering(this, new Rendering3DArgs(m_renderState)); }
        //}

        ///// <summary>
        ///// Raises the BeforeRendering event.
        ///// </summary>
        //private void RaiseBeforeRendering()
        //{
        //    if (BeforeRendering != null) { BeforeRendering(this, new Rendering3DArgs(m_renderState)); }
        //}

        ///// <summary>
        ///// Raises the BeforeUpdating event.
        ///// </summary>
        //private void RaiseBeforeUpdating(UpdateState updateState)
        //{
        //    if (BeforeUpdating != null) { BeforeUpdating(this, new Updating3DArgs(updateState)); }
        //}

        ///// <summary>
        ///// Recreates current viewport.
        ///// </summary>
        //private void RecreateViewPort()
        //{
        //    int width = Math.Max((int)base.ActualWidth, 100);
        //    int height = Math.Max((int)base.ActualHeight, 100);

        //    //Define the viewport for rendering
        //    D3D11.Viewport viewPort = GraphicsHelper.CreateDefaultViewport(width, height);

        //    //Set viewport and render target on the device
        //    if (m_renderState == null)
        //    {
        //        m_renderState = new RenderState(
        //            m_renderDevice,
        //            m_renderDeviceContext,
        //            m_performanceCalculator,
        //            m_renderTarget, m_renderTargetDepth, viewPort,
        //            Matrix4.Identity);
        //    }
        //    else
        //    {
        //        m_renderState.Reset(
        //            m_renderTarget, m_renderTargetDepth, viewPort,
        //            Matrix4.Identity);
        //    }

        //    //Apply new width and height values of the viewport
        //    m_viewportWidth = width;
        //    m_viewportHeight = height;
        //}

        /// <summary>
        /// Performs rendering.
        /// </summary>
        private void OnRenderTrigger()
        {
            if (!GraphicsCore.IsInitialized) { return; }

            if (m_initialized)
            {
                m_renderLoop.Render();
            }

            //if (m_d3dImageSource == null) { Initialize(); }
            //if (!m_d3dImageSource.IsFrontBufferAvailable) { return; }
            //if (!m_d3dImageSource.HasRenderTarget) { return; }
            //if (this.Width <= 0) { return; }
            //if (this.Height <= 0) { return; }
            //if (!m_d3dImageSource.TryLock(new Duration(TimeSpan.Zero))) 
            //{
            //    m_d3dImageSource.Unlock();
            //    return; 
            //}
            ////m_performanceCalculator.MeasureActivityDuration("Render.Lock", () => m_d3dImageSource.Lock());

            //var renderTimeMeasurenment = m_performanceCalculator.BeginMeasureActivityDuration("Render.Complete");
            //try
            //{
            //    //Set default rastarization state
            //    D3D11.RasterizerState rasterState = null;
            //    bool wireframeEnabled = this.IsWireframeEnabled;
            //    if (wireframeEnabled)
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
            //    DateTime currentTime = DateTime.UtcNow;
            //    TimeSpan updateTime = currentTime - m_lastRenderTime;
            //    if (updateTime.TotalMilliseconds > 100.0) { updateTime = TimeSpan.FromMilliseconds(100.0); }
            //    m_lastRenderTime = currentTime;
            //    UpdateState updateState = new UpdateState(updateTime);

            //    //Apply current target
            //    m_renderState.ApplyCurrentTarget();
            //    m_renderState.ApplyMaterial(null);

            //    //Paint using Direct3D
            //    m_renderDeviceContext.ClearRenderTargetView(m_renderTarget, new SharpDX.Color(0, 0, 0, 0));
            //    m_renderDeviceContext.ClearDepthStencilView(m_renderTargetDepth, D3D11.DepthStencilClearFlags.Depth | D3D11.DepthStencilClearFlags.Stencil, 1f, 0);

            //    //Raise events
            //    RaiseBeforeUpdating(updateState);
            //    RaiseBeforeRendering();

            //    //Call render methods of subclasses
            //    OnDirect3DPaint(m_renderState, updateState);

            //    //Raise AfterRendering event
            //    RaiseAfterRendering();

            //    //Clear current state after rendering
            //    m_renderState.ClearState();

            //    if (wireframeEnabled)
            //    {
            //        m_renderDeviceContext.Rasterizer.State = null;
            //        rasterState.Dispose();
            //    }

            //    ////Raises the TextureChanged event
            //    //m_backBufferSource.RaiseTextureChanged(m_renderState);

            //    //Copy contents of direct3D 11 texture to wpf texture. This step makes following possible
            //    // => Move all rendering logic to a background thread and perform only following on gui thread
            //    using (var finishRenderTimeMeasurenment = m_performanceCalculator.BeginMeasureActivityDuration("Render.Finishing"))
            //    {
            //        m_renderDeviceContext.ResolveSubresource(m_backBufferD3D11, 0, m_backBufferForWpf, 0, DXGI.Format.B8G8R8A8_UNorm);
            //        m_renderDeviceContext.Flush();
            //        m_renderDeviceContext.ClearState();

            //        m_d3dImageSource.AddDirtyRect(new Int32Rect(0, 0, m_d3dImageSource.PixelWidth, m_d3dImageSource.PixelHeight));
            //    }
            //}
            //finally
            //{
            //    m_d3dImageSource.Unlock();
            //    renderTimeMeasurenment.Dispose();
            //}
        }

        ///// <summary>
        ///// Unloads all view resources.
        ///// </summary>
        //private void UnloadViewResources()
        //{
        //    if (m_renderDevice != null)
        //    {
        //        m_renderState = GraphicsHelper.DisposeGraphicsObject(m_renderState);
        //        m_renderTarget2DDxgi = GraphicsHelper.DisposeGraphicsObject(m_renderTarget2DDxgi);
        //        m_renderTargetDepth = GraphicsHelper.DisposeGraphicsObject(m_renderTargetDepth);
        //        m_depthBuffer = GraphicsHelper.DisposeGraphicsObject(m_depthBuffer);
        //        m_renderTarget = GraphicsHelper.DisposeGraphicsObject(m_renderTarget);
        //        m_backBufferForWpf = GraphicsHelper.DisposeGraphicsObject(m_backBufferForWpf);
        //        m_backBufferD3D11 = GraphicsHelper.DisposeGraphicsObject(m_backBufferD3D11);

        //        m_renderDevice = null;
        //    }
        //}

        //public ActivityPerformanceValueContainer PerformanceCalculator
        //{
        //    get { return m_performanceCalculator; }
        //}
    }
}