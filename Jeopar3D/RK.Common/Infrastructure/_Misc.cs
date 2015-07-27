using System;

namespace RK.Common.Infrastructure
{
    public class BootstrapperItemArgs : EventArgs
    {
        internal BootstrapperItemArgs(IBootstrapperItem item)
        {
            this.Item = item;
        }

        public IBootstrapperItem Item
        {
            get;
            private set;
        }
    }
}
