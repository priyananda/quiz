using System;
using System.ComponentModel;

namespace RK.Common.Util
{
    /// <summary>
    /// This class holds all information about message subscriptions. It implements IDisposable for unsubscribing
    /// from the message.
    /// </summary>
    public class MessageSubscription : IDisposable
    {
        private ApplicationMessageHandler m_messageHandler;
        private Type m_messageType;
        private Delegate m_targetHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSubscription"/> class.
        /// </summary>
        /// <param name="messageHandler">The message handler.</param>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="targetHandler">The target handler.</param>
        internal MessageSubscription(ApplicationMessageHandler messageHandler, Type messageType, Delegate targetHandler)
        {
            m_messageHandler = messageHandler;
            m_messageType = messageType;
            m_targetHandler = targetHandler;
        }

        /// <summary>
        /// Unsubscribes this subscription.
        /// </summary>
        public void Unsubscribe()
        {
            this.Dispose();
        }

        /// <summary>
        /// Sends the given message to the target.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="message"></param>
        internal void Publish<MessageType>(MessageType message)
            where MessageType : ApplicationMessage
        {
            if (!this.IsDisposed)
            {
                Action<MessageType> targetDelegate = m_targetHandler as Action<MessageType>;
                if (targetDelegate != null)
                {
                    targetDelegate(message);
                }
            }
        }

        /// <summary>
        /// Clears this message.
        /// </summary>
        internal void Clear()
        {
            m_messageHandler = null;
            m_messageType = null;
            m_targetHandler = null;
        }

        /// <summary>
        /// Disposes this object.
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed)
            {
                m_messageHandler.Unsubscribe(this);
            }
        }

        /// <summary>
        /// Is this subscription valid?
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return
                    (m_messageHandler == null) ||
                    (m_messageType == null) ||
                    (m_targetHandler == null);
            }
        }

        /// <summary>
        /// Gets the corresponding MessageHandler object.
        /// </summary>
        public ApplicationMessageHandler MessageHandler
        {
            get { return m_messageHandler; }
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public Type MessageType
        {
            get { return m_messageType; }
        }

        /// <summary>
        /// Gets the name of the message type.
        /// </summary>
        public string MessageTypeName
        {
            get { return m_messageType.Name; }
        }

        /// <summary>
        /// Gets the name of the target object.
        /// </summary>
        public string TargetObjectName
        {
            get
            {
                if (m_targetHandler == null) { return string.Empty; }
                if (m_targetHandler.Target == null) { return string.Empty; }
                return m_targetHandler.Target.ToString();
            }
        }

#if DESKTOP
        /// <summary>
        /// Gets the name of the target method.
        /// </summary>
        public string TargetMethodName
        {
            get
            {
                if (m_targetHandler == null) { return string.Empty; }
                if (m_targetHandler.Target == null) { return string.Empty; }
                if (m_targetHandler.Method == null) { return string.Empty; }
                return m_targetHandler.Method.Name;
            }
        }
#endif
    }
}
