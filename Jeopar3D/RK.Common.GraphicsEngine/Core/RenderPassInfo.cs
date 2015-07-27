using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RK.Common.GraphicsEngine.Core
{
    public class RenderPassInfo
    {
        public static readonly RenderPassInfo PASS_PLAIN_RENDER = new RenderPassInfo("DefaultPlainRender");
        public static readonly RenderPassInfo PASS_TRANSPARENT_RENDER = new RenderPassInfo("DefaultTransparentRender");

        private static List<RenderPassInfo> s_renderPasses;
        private static ReadOnlyCollection<RenderPassInfo> s_renderPassesPublic;

        private string m_name;

        /// <summary>
        /// Initializes the <see cref="RenderPassInfo" /> class.
        /// </summary>
        static RenderPassInfo()
        {
            s_renderPasses = new List<RenderPassInfo>();
            s_renderPassesPublic = new ReadOnlyCollection<RenderPassInfo>(s_renderPasses);

            s_renderPasses.Add(PASS_PLAIN_RENDER);
            s_renderPasses.Add(PASS_TRANSPARENT_RENDER);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderPassInfo" /> class.
        /// </summary>
        internal RenderPassInfo(string name)
        {
            m_name = name;
        }

        /// <summary>
        /// Gets a collection containing all render passes.
        /// </summary>
        public static ReadOnlyCollection<RenderPassInfo> AllRenderPasses
        {
            get { return s_renderPassesPublic; }
        }

        /// <summary>
        /// Gets the name of this pass.
        /// </summary>
        public string Name
        {
            get { return m_name; }
        }
    }
}
