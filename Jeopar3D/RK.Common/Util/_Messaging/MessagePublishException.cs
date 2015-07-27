using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RK.Common.Util
{
    /// <summary>
    /// An exception info holding all information about exceptions occurred during publishing a message.
    /// </summary>
    public class MessagePublishException : CommonLibraryException
    {
        private Type m_messageType;
        private List<Exception> m_publishExceptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublishException" /> class.
        /// </summary>
        public MessagePublishException()
            : base("Exceptions where raised while processing a message")
        {
            m_messageType = null;
            m_publishExceptions = new List<Exception>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagePublishException"/> class.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="publishExceptions">Exceptions raised during publish process.</param>
        public MessagePublishException(Type messageType, List<Exception> publishExceptions)
            : base("Exceptions where raised while processing message of type " + messageType.FullName + "!")
        {
            m_messageType = messageType;
            m_publishExceptions = publishExceptions;

            if (m_publishExceptions == null) { m_publishExceptions = new List<Exception>(); }
        }

        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        public Type MessageType
        {
            get { return m_messageType; }
        }

        /// <summary>
        /// Gets a list containing all exceptions.
        /// </summary>
        public List<Exception> PublishExceptions
        {
            get { return m_publishExceptions; }
        }
    }
}
