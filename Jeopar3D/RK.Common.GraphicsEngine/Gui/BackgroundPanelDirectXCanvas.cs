using System;
using System.Reactive.Linq;
using System.Threading;
using RK.Common;
using RK.Common.GraphicsEngine.Core;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.Util;
using SharpDX;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using D3D11 = SharpDX.Direct3D11;

//Some namespace mappings
using DXGI = SharpDX.DXGI;

namespace RK.Common.GraphicsEngine.Gui
{
    // Using SwapChainBackgroundPanel to render to the background of the WinRT app
    //  see http://msdn.microsoft.com/en-us/library/windows/apps/hh825871.aspx

    public class BackgroundPanelDirectXCanvas : IDisposable
    {
        private bool m_initialized;
        private SwapChainBackgroundPanel m_targetPanel;
        private float m_currentDpi;
        private Size m_lastRefreshTargetSize;

        private RenderLoop m_renderLoop;

        //Event handling
        private IDisposable m_observerSizeChanged;

        //Resources from Direct3D 11
        private DXGI.Factory2 m_factory;
        private DXGI.SwapChain1 m_swapChain;
        private DXGI.ISwapChainBackgroundPanelNative m_targetPanelInterface;
        private D3D11.Device m_renderDevice;
        private D3D11.DeviceContext m_renderDeviceContext;
        private D3D11.Texture2D m_backBuffer;
        private D3D11.Texture2D m_depthBuffer;
        private D3D11.RenderTargetView m_renderTarget;
        private D3D11.DepthStencilView m_renderTargetDepth;

        private DispatcherTimer m_renderTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundPanelDirectXCanvas" /> class.
        /// </summary>
        /// <param name="targetPanel">The target panel.</param>
        public BackgroundPanelDirectXCanvas(SwapChainBackgroundPanel targetPanel)
        {
            m_lastRefreshTargetSize = new Size(0.0, 0.0);
            m_currentDpi = DisplayProperties.LogicalDpi;
            m_targetPanel = targetPanel;
            m_targetPanelInterface = ComObject.As<DXGI.ISwapChainBackgroundPanelNative>(targetPanel);

            //Create the RenderLoop object
            m_renderLoop = new Core.RenderLoop(
                OnRenderLoopCreateViewResources,
                OnRenderLoopDisposeViewResources,
                OnRenderLoopCheckCanRender,
                OnRenderLoopPrepareRendering,
                OnRenderLoopAfterRendering,
                OnRenderLoopPresent);

            Initialize3D();

            //Attach to SizeChanged event (refresh view resources only after a specific time)
            var observerSizeChanged = m_observerSizeChanged = Observable.FromEventPattern<SizeChangedEventArgs>(targetPanel, "SizeChanged")
                .Throttle(TimeSpan.FromSeconds(0.5))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe((eArgs) => OnTargetPanelThrottledSizeChanged(eArgs.Sender, eArgs.EventArgs));
            targetPanel.SizeChanged += OnTargetPanelSizeChanged;

            //Start render time when this control is loaded
            targetPanel.Loaded += (sender, eArgs) =>
            {
                m_renderTimer = new DispatcherTimer();
                m_renderTimer.Interval = TimeSpan.FromMilliseconds(10.0);
                m_renderTimer.RegisterSafeTick(OnRenderTimerTick);
                m_renderTimer.Start();
            };

            //Define unloading behavior
            targetPanel.Unloaded += (sender, eArgs) =>
            {
                //Clear created references
                observerSizeChanged.Dispose();
                m_renderTimer.Stop();
                m_renderTimer = null;
                targetPanel.SizeChanged -= OnTargetPanelSizeChanged;
                targetPanel.KeyDown -= OnTargetPanelKeyDown;
                m_targetPanel = null;
                m_targetPanelInterface.Dispose();
                m_targetPanelInterface = null;

                try
                {
                    this.Unload3D();
                }
                catch (Exception ex)
                {
                    CommonUtil.RaiseUnhandledException(this, ex);
                }
            };
            targetPanel.KeyDown += OnTargetPanelKeyDown;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Unload3D();
        }

        /// <summary>
        /// Renders all contents of the screen
        /// </summary>
        public void Render()
        {
            if (!GraphicsCore.IsInitialized) { return; }

            if (m_initialized)
            {
                m_renderLoop.Render();
            }
        }

        /// <summary>
        /// Called when the size of the target panel has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.SizeChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnTargetPanelThrottledSizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (!GraphicsCore.IsInitialized) { return; }

                //Ignore event, if nothing has changed..
                if (m_lastRefreshTargetSize == m_targetPanel.RenderSize) { return; }

                if (m_initialized)
                {
                    m_renderLoop.RefreshViewResources();
                }
            }
            catch (Exception ex)
            {
                CommonUtil.RaiseUnhandledException(this, ex);
            }
        }

        /// <summary>
        /// Called when any key was pressed on the target panel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyRoutedEventArgs" /> instance containing the event data.</param>
        private void OnTargetPanelKeyDown(object sender, KeyRoutedEventArgs e)
        {
            //try
            //{
            //    switch (e.Key)
            //    {
            //        case Windows.System.VirtualKey.F12:
            //            if (m_targetPanel.Children.ContainsAny(typeof(PerformanceAnalysisControl)))
            //            {
            //                m_targetPanel.Children.RemoveAll(typeof(PerformanceAnalysisControl));
            //            }
            //            else
            //            {
            //                PerformanceAnalysisControl analysisControl = new PerformanceAnalysisControl();
            //                analysisControl.DataContext = m_renderLoop.PerformanceCalculator;
            //                m_targetPanel.Children.Add(analysisControl);
            //            }
            //            break;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    CommonUtil.RaiseUnhandledException(this, ex);
            //}
        }

        /// <summary>
        /// Called when the render timer ticks.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void OnRenderTimerTick(object sender, object e)
        {
            this.Render();
        }

        private void OnTargetPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!GraphicsCore.IsInitialized) { return; }

            m_renderLoop.Camera.SetScreenSize((int)e.NewSize.Width, (int)e.NewSize.Height);

            //Refresh view resources directly on big resize steps
            if ((Math.Abs(e.NewSize.Width - m_lastRefreshTargetSize.Width) > 300) ||
                (Math.Abs(e.NewSize.Height - m_lastRefreshTargetSize.Height) > 300))
            {
                m_renderLoop.RefreshViewResources();
            }
        }

        /// <summary>
        /// Creates the swap chain.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="device">The device.</param>
        /// <param name="desc">The desc.</param>
        private DXGI.SwapChain1 CreateSwapChain(DXGI.Factory2 factory, D3D11.Device1 device, DXGI.SwapChainDescription1 desc)
        {
            //Creates the swap chain for XAML composition
            DXGI.SwapChain1 swapChain = factory.CreateSwapChainForComposition(device, ref desc, null);

            //Associate the SwapChainBackgroundPanel with the swap chain
            m_targetPanelInterface.SwapChain = swapChain;

            //Returns the new swap chain
            return swapChain;
        }

        /// <summary>
        /// Creates the swap chain description.
        /// </summary>
        /// <returns>A swap chain description</returns>
        private DXGI.SwapChainDescription1 CreateSwapChainDescription()
        {
            int qualityLevels = m_renderDevice.CheckMultisampleQualityLevels(DXGI.Format.B8G8R8A8_UNorm, 2);

            // SwapChain description
            var desc = new SharpDX.DXGI.SwapChainDescription1()
            {
                // Automatic sizing
                Width = Width,
                Height = Height,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                Stereo = false,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = SharpDX.DXGI.Usage.BackBuffer | SharpDX.DXGI.Usage.RenderTargetOutput,
                BufferCount = 2,
                Scaling = SharpDX.DXGI.Scaling.Stretch,
                SwapEffect = SharpDX.DXGI.SwapEffect.FlipSequential,
            };
            return desc;
        }

        /// <summary>
        /// Disposes all loaded view resources.
        /// </summary>
        private void OnRenderLoopDisposeViewResources()
        {
            m_renderTargetDepth = GraphicsHelper.DisposeGraphicsObject(m_renderTargetDepth);
            m_depthBuffer = GraphicsHelper.DisposeGraphicsObject(m_depthBuffer);
            m_renderTarget = GraphicsHelper.DisposeGraphicsObject(m_renderTarget);
            m_backBuffer = GraphicsHelper.DisposeGraphicsObject(m_backBuffer);
            m_swapChain = GraphicsHelper.DisposeGraphicsObject(m_swapChain);
        }

        /// <summary>
        /// Create all view resources.
        /// </summary>
        private Tuple<D3D11.RenderTargetView, D3D11.DepthStencilView, D3D11.Viewport> OnRenderLoopCreateViewResources()
        {
            //Create the swap chain and the render target
            m_swapChain = CreateSwapChain(
                GraphicsCore.Current.HandlerDXGI.Factory,
                GraphicsCore.Current.HandlerD3D11.Device,
                CreateSwapChainDescription());
            m_backBuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(m_swapChain, 0);
            m_renderTarget = new D3D11.RenderTargetView(m_renderDevice, m_backBuffer);

            //Create the depth buffer
            m_depthBuffer = GraphicsHelper.CreateDepthBufferTexture(m_renderDevice, this.Width, this.Height);
            m_renderTargetDepth = new D3D11.DepthStencilView(m_renderDevice, m_depthBuffer);

            //Define the viewport for rendering
            D3D11.Viewport viewPort = GraphicsHelper.CreateDefaultViewport(this.Width, this.Height);
            m_lastRefreshTargetSize = m_targetPanel.RenderSize;

            return Tuple.Create(m_renderTarget, m_renderTargetDepth, viewPort);
        }

        /// <summary>
        /// Called when RenderLoop object checks wheter it is possible to render.
        /// </summary>
        private bool OnRenderLoopCheckCanRender()
        {
            if (this.Width <= 0) { return false; }
            if (this.Height <= 0) { return false; }
            if (!m_initialized) { return false; }

            return true;
        }

        private void OnRenderLoopPrepareRendering()
        {
        }

        /// <summary>
        /// Called when RenderLoop wants to present its results.
        /// </summary>
        private void OnRenderLoopPresent()
        {
            // Present all rendered stuff on screen
            // First parameter indicates synchronization with vertical blank
            //  see http://msdn.microsoft.com/en-us/library/windows/desktop/bb174576(v=vs.85).aspx
            //  see example http://msdn.microsoft.com/en-us/library/windows/apps/hh825871.aspx

            m_swapChain.Present(1, DXGI.PresentFlags.None);
        }

        /// <summary>
        /// Called when RenderLoop has finished rendering.
        /// </summary>
        private void OnRenderLoopAfterRendering()
        {
        }

        /// <summary>
        /// Initialize 3d graphics with direct3D 11.
        /// </summary>
        private void Initialize3D()
        {
            if (!GraphicsCore.IsInitialized) { return; }

            //Get all factories
            m_factory = GraphicsCore.Current.HandlerDXGI.Factory;

            //Get all devices
            m_renderDevice = GraphicsCore.Current.HandlerD3D11.Device;
            m_renderDeviceContext = m_renderDevice.ImmediateContext;

            //Creates swap chain and render target
            if ((this.Width > 0) && (this.Height > 0))
            {
                m_renderLoop.RefreshViewResources();
            }

            //Update local init-flag
            m_initialized = true;
        }

        /// <summary>
        /// Unloads current 3D content.
        /// </summary>
        private void Unload3D()
        {
            if (!GraphicsCore.IsInitialized) { return; }

            if (m_initialized)
            {
                //Disable rendering
                m_initialized = false;

                //Clear view resources
                m_renderLoop.UnloadViewResources();

                m_factory = null;
                m_renderDevice = null;
                m_renderDeviceContext = null;
            }
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

        /// <summary>
        /// Gets current renderloop object.
        /// </summary>
        public RenderLoop RenderLoop
        {
            get { return m_renderLoop; }
        }

        /// <summary>
        /// Width of the swap chain to create or resize.
        /// </summary>
        public int Width
        {
            get
            {
                return (int)(m_targetPanel.RenderSize.Width * m_currentDpi / 96.0);
            }
        }

        /// <summary>
        /// Height of the swap chain to create or resize.
        /// </summary>
        public int Height
        {
            get
            {
                return (int)(m_targetPanel.RenderSize.Height * m_currentDpi / 96.0);
            }
        }
    }
}