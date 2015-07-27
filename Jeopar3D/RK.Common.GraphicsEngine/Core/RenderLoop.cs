using System;
using System.Threading;
using RK.Common.GraphicsEngine.Drawing3D;
using RK.Common.Util;
using D3D11 = SharpDX.Direct3D11;

namespace RK.Common.GraphicsEngine.Core
{
    public class RenderLoop : IDisposable
    {
        public const string ACTIVITY_COUNT_RENDER = "Render";
        public const string ACTIVITY_DURATION_RENDER_COMPLETE = "Render.Complete";
        public const string ACTIVITY_DURATION_RENDER_UPDATE = "Render.Update";
        public const string ACTIVITY_DURATION_RENDER_RENDER_SCENE = "Render.RenderScene";

        //Configuration values
        private SynchronizationContext m_uiSyncContext;
        private bool m_isWireframeEnabled;
        private bool m_discardRendering;
        private bool m_forceNextFrame;
        private Color4 m_clearColor;
        private Camera m_camera;
        private Scene m_scene;
        private bool m_updateOnRender;
        private GraphicsConfiguration m_graphicsConfig;

        //Callback methods for current host object
        private Func<Tuple<D3D11.RenderTargetView, D3D11.DepthStencilView, D3D11.Viewport>> m_actionCreateViewResources;
        private Action m_actionDisposeViewResources;
        private Func<bool> m_actionCheckCanRender;
        private Func<DateTime> m_actionGetTimestamp;
        private Action m_actionPrepareRendering;
        private Action m_actionAfterRendering;
        private Action m_actionPresent;

        //Values needed for runtime
        private DateTime m_lastRenderTime;
        private int m_totalRenderCount;
        private ActivityPerformanceValueContainer m_performanceCalculator;
        private RenderState m_renderState;

        //Direct3D resources
        private D3D11.Device m_device;
        private D3D11.DeviceContext m_deviceImmediateContext;
        private D3D11.RenderTargetView m_renderTarget;
        private D3D11.DepthStencilView m_renderTargetDepth;
        private D3D11.Viewport m_viewport;

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderLoop" /> class.
        /// </summary>
        public RenderLoop(
            Func<Tuple<D3D11.RenderTargetView, D3D11.DepthStencilView, D3D11.Viewport>> actionCreateViewResources,
            Action actionDisposeViewResources,
            Func<bool> actionCheckCanRender,
            Action actionPrepareRendering,
            Action actionAfterRendering,
            Action actionPresent)
        {
            //Assign all given actions
            m_actionCreateViewResources = actionCreateViewResources;
            m_actionDisposeViewResources = actionDisposeViewResources;
            m_actionCheckCanRender = actionCheckCanRender;
            m_actionPrepareRendering = actionPrepareRendering;
            m_actionAfterRendering = actionAfterRendering;
            m_actionPresent = actionPresent;

            //Assign default actions
            m_actionGetTimestamp = () => DateTime.UtcNow;

            //Create default objects
            m_updateOnRender = true;
            m_camera = new Drawing3D.Camera();
            m_clearColor = Color4.CornflowerBlue;
            m_isWireframeEnabled = false;
            m_scene = new Scene();
            m_graphicsConfig = new GraphicsConfiguration();

            //Create additional values
            m_performanceCalculator = new ActivityPerformanceValueContainer();

            if (!GraphicsCore.IsInitialized) { return; }

            //Get global objects
            m_device = GraphicsCore.Current.HandlerD3D11.Device;
            m_deviceImmediateContext = m_device.ImmediateContext;
        }

        public void ResetRenderCounter()
        {
            m_totalRenderCount = 0;
        }

        /// <summary>
        /// Refreshes the view resources.
        /// </summary>
        public void RefreshViewResources()
        {
            if (m_device == null) { return; }

            //Dispose current view resources
            m_actionDisposeViewResources();

            //Recreate view resources
            var generatedViewResources = m_actionCreateViewResources();
            m_renderTarget = generatedViewResources.Item1;
            m_renderTargetDepth = generatedViewResources.Item2;
            m_viewport = generatedViewResources.Item3;

            //Create or update current renderstate
            if (m_renderState == null)
            {
                m_renderState = new RenderState(
                    m_device, m_deviceImmediateContext, m_performanceCalculator,
                    m_renderTarget, m_renderTargetDepth, m_viewport,
                    m_camera);
            }

            //Configure this loop to render the next frame
            m_forceNextFrame = true;
        }

        /// <summary>
        /// Unloads all view resources.
        /// </summary>
        public void UnloadViewResources()
        {
            if (m_device == null) { return; }

            m_actionDisposeViewResources();
        }

        int blub = 0;

        /// <summary>
        /// Renders this instance.
        /// </summary>
        public void Render()
        {
            if (m_device == null) { return; }
            if (m_discardRendering)
            {
                if (!m_forceNextFrame) { return; }
            }
            if (m_renderTarget == null) { return; }
            if (m_renderTargetDepth == null) { return; }

            //Reset force render flag
            m_forceNextFrame = false;

            //Check here wether we can render or not
            if (!m_actionCheckCanRender()) { return; }

            //Perform some preparation for rendering
            m_actionPrepareRendering();

            var renderTimeMeasurenment = m_performanceCalculator.BeginMeasureActivityDuration(ACTIVITY_DURATION_RENDER_COMPLETE);
            try
            {
                //Set default rastarization state
                D3D11.RasterizerState rasterState = null;
                bool isWireframeEnabled = m_isWireframeEnabled;
                if (isWireframeEnabled)
                {
                    rasterState = new D3D11.RasterizerState(m_device, new D3D11.RasterizerStateDescription()
                    {
                        CullMode = D3D11.CullMode.Back,
                        FillMode = D3D11.FillMode.Wireframe,
                        IsFrontCounterClockwise = false,
                        DepthBias = 0,
                        SlopeScaledDepthBias = 0f,
                        DepthBiasClamp = 0f,
                        IsDepthClipEnabled = true,
                        IsAntialiasedLineEnabled = false,
                        IsMultisampleEnabled = false,
                        IsScissorEnabled = false
                    });
                    m_deviceImmediateContext.Rasterizer.State = rasterState;
                }

                //Create update state object
                DateTime currentTime = m_actionGetTimestamp();
                TimeSpan updateTime = currentTime - m_lastRenderTime;
                if (updateTime.TotalMilliseconds > 100.0) { updateTime = TimeSpan.FromMilliseconds(100.0); }
                m_lastRenderTime = currentTime;
                UpdateState updateState = new UpdateState(updateTime);

                //Update render state
                m_renderState.Reset(m_renderTarget, m_renderTargetDepth, m_viewport, m_camera);
                m_renderState.ApplyMaterial(null);

                //Paint using Direct3D
                m_deviceImmediateContext.ClearRenderTargetView(m_renderTarget, new SharpDX.Color(0, 0, 0, 0));
                m_deviceImmediateContext.ClearDepthStencilView(m_renderTargetDepth, D3D11.DepthStencilClearFlags.Depth | D3D11.DepthStencilClearFlags.Stencil, 1f, 0);

                //Render currently configured scene
                if ((m_scene != null) && (m_camera != null))
                {
                    m_renderState.PushScene(m_scene);
                    try
                    {
                        //Updates current scene
                        if (m_updateOnRender)
                        {
                            m_performanceCalculator.MeasureActivityDuration(ACTIVITY_DURATION_RENDER_UPDATE, () =>
                            {
                                m_scene.Update(updateState);
                            });
                        }

                        //Current render state
                        m_performanceCalculator.MeasureActivityDuration(ACTIVITY_DURATION_RENDER_RENDER_SCENE, () =>
                        {
                            m_scene.Render(m_renderState);
                        });
                    }
                    finally
                    {
                        m_renderState.PopScene();
                    }
                }

                //Clear current state after rendering
                m_renderState.ClearState();

                if (isWireframeEnabled)
                {
                    m_deviceImmediateContext.Rasterizer.State = null;
                    rasterState.Dispose();
                }

                if (m_totalRenderCount < Int32.MaxValue) { m_totalRenderCount++; }

                //Presents all contents on the screen
                m_actionPresent();
            }
            finally
            {
                //m_d3dImageSource.Unlock();
                renderTimeMeasurenment.Dispose();
            }

            //Finish rendering now
            m_actionAfterRendering();

            m_performanceCalculator.NotifyDoneActivity(ACTIVITY_COUNT_RENDER);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.UnloadViewResources();
        }

        public int TotalRenderCount
        {
            get { return m_totalRenderCount; }
        }

        /// <summary>
        /// Is wireframe mode enabled?
        /// </summary>
        public bool IsWireframeEnabled
        {
            get { return m_isWireframeEnabled; }
            set { m_isWireframeEnabled = value; }
        }

        /// <summary>
        /// Discard rendering?
        /// </summary>
        public bool DiscardRendering
        {
            get { return m_discardRendering; }
            set { m_discardRendering = value; }
        }

        public SynchronizationContext UISynchronizationContext
        {
            get { return m_uiSyncContext; }
            set { m_uiSyncContext = value; }
        }

        /// <summary>
        /// Gets current performance calculator.
        /// </summary>
        public ActivityPerformanceValueContainer PerformanceCalculator
        {
            get { return m_performanceCalculator; }
        }

        /// <summary>
        /// Gets or sets the current clear color.
        /// </summary>
        public Color4 ClearColor
        {
            get { return m_clearColor; }
            set { m_clearColor = value; }
        }

        /// <summary>
        /// Gets or sets an action which gets a timestamp.
        /// </summary>
        public Func<DateTime> ActionGetTimestamp
        {
            get { return m_actionGetTimestamp; }
            set
            {
                if (value == null) { return; }
                m_actionGetTimestamp = value;
            }
        }

        /// <summary>
        /// Gets or sets current camera object.
        /// </summary>
        public Camera Camera
        {
            get { return m_camera; }
            set
            {
                if (value == null) { m_camera = new Camera(); }
                else { m_camera = value; }
            }
        }

        /// <summary>
        /// Gets or sets currently used 3d scene.
        /// </summary>
        public Scene Scene
        {
            get { return m_scene; }
            set
            {
                if (value == null) { m_scene = new Scene(); }
                else { m_scene = value; }
            }
        }

        /// <summary>
        /// Perform update call on render?
        /// </summary>
        public bool UpdateOnRender
        {
            get { return m_updateOnRender; }
            set { m_updateOnRender = value; }
        }

        /// <summary>
        /// Gets or sets current graphics configuration.
        /// </summary>
        public GraphicsConfiguration GraphicsConfiguration
        {
            get { return m_graphicsConfig; }
            set
            {
                if (value == null) { m_graphicsConfig = new GraphicsConfiguration(); }
                else { m_graphicsConfig = value; }
            }
        }
    }
}