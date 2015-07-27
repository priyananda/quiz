using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RK.Common.Mvvm;
using RK.Common.Util;

namespace RK.Common.Infrastructure
{
    public class Bootstrapper : PropertyChangedBase
    {
        private IBootstrapperItem m_currentItem;
        private List<IBootstrapperItem> m_items;
        private bool m_booted;

        public event EventHandler<BootstrapperItemArgs> ItemExecuted;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper" /> class.
        /// </summary>
        public Bootstrapper()
        {
            m_items = new List<IBootstrapperItem>();
        }

        /// <summary>
        /// Registers a new item for the bootstraper.
        /// </summary>
        /// <typeparam name="T">The type of the item to register.</typeparam>
        public void RegisterBootstrapperItem<T>()
            where T : IBootstrapperItem, new()
        {
            this.RegisterBootstrapperItem(new T());
        }

        /// <summary>
        /// Registers a new item for the bootstraper.
        /// </summary>
        /// <typeparam name="T">The type of the item to register.</typeparam>
        /// <param name="bootstrapperItemToAdd">The item to register.</param>
        public void RegisterBootstrapperItem<T>(T bootstrapperItemToAdd)
            where T : IBootstrapperItem
        {
            if (m_booted) { throw new InvalidOperationException("The application object has already booted!"); }

            if (!m_items.Contains(bootstrapperItemToAdd))
            {
                m_items.Add(bootstrapperItemToAdd);
            }
        }

        /// <summary>
        /// Runs all items within this bootstrapper.
        /// </summary>
        public async Task Run()
        {
            foreach (IBootstrapperItem actItem in m_items)
            {
                //Update current item property
                this.CurrentItem = actItem;

                //Execute the item
                await actItem.Execute();

                //Raise item executed event
                ItemExecuted.Raise(this, new BootstrapperItemArgs(actItem));
            }

            this.CurrentItem = null;
            this.Booted = true;
        }

        /// <summary>
        /// Are all items finished?
        /// </summary>
        public bool Booted
        {
            get { return m_booted; }
            private set
            {
                if (m_booted != value)
                {
                    m_booted = value;
                    base.RaisePropertyChanged(() => this.Booted);
                }
            }
        }

        /// <summary>
        /// Gets the currently executing item.
        /// </summary>
        public IBootstrapperItem CurrentItem
        {
            get { return m_currentItem; }
            private set
            {
                if (m_currentItem != value)
                {
                    m_currentItem = value;
                    base.RaisePropertyChanged(() => CurrentItem);
                }
            }
        }
    }
}
