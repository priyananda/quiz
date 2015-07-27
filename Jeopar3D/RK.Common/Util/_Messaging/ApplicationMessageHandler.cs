using System;
using System.Collections.Generic;
using System.Threading;

namespace RK.Common.Util
{
    /// <summary>
    /// Main class for application messaging. 
    /// </summary>
    public class ApplicationMessageHandler
    {
        private SynchronizationContext m_syncContext;
        private Dictionary<Type, List<MessageSubscription>> m_messageSubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMessageHandler"/> class.
        /// </summary>
        public ApplicationMessageHandler()
        {
            m_syncContext = SynchronizationContext.Current;

            m_messageSubscriptions = new Dictionary<Type, List<MessageSubscription>>();
        }

        /// <summary>
        /// Gets a collection containing all active subscriptions.
        /// </summary>
        public List<MessageSubscription> GetAllSubscriptions()
        {
            List<MessageSubscription> result = new List<MessageSubscription>();

            foreach (KeyValuePair<Type, List<MessageSubscription>> actPair in m_messageSubscriptions)
            {
                foreach (MessageSubscription actSubscription in actPair.Value)
                {
                    result.Add(actSubscription);
                }
            }

            return result;
        }

        /// <summary>
        /// Subscribes to the given MessageType.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="messageHandler">Action to perform on incoming message.</param>
        public MessageSubscription Subscribe<MessageType>(Action<MessageType> messageHandler)
            where MessageType : ApplicationMessage
        {
            Type currentType = typeof(MessageType);

            MessageSubscription newOne = new MessageSubscription(this, currentType, messageHandler);
            if (m_messageSubscriptions.ContainsKey(currentType))
            {
                m_messageSubscriptions[currentType].Add(newOne);
            }
            else
            {
                List<MessageSubscription> newList = new List<MessageSubscription>();
                newList.Add(newOne);
                m_messageSubscriptions[currentType] = newList;
            }

            return newOne;
        }

#if DESKTOP
        /// <summary>
        /// Subscribes to the given MessageType during the livetime of the given framework element.
        /// Events Loaded and Unloaded are used for subscribing / unsubscribing.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="messageHandler">Action to perform on incoming message.</param>
        /// <param name="target">The target FrameworkElement.</param>
        public void SubscribeOnFrameworkElement<MessageType>(System.Windows.FrameworkElement target, Action<MessageType> messageHandler)
            where MessageType : ApplicationMessage
        {
            MessageSubscription subscription = null;

            //Create handler delegates
            System.Windows.RoutedEventHandler onHandleCreated = (sender, eArgs) =>
            {
                if (subscription == null)
                {
                    subscription = Subscribe(messageHandler);
                }
            };
            System.Windows.RoutedEventHandler onHandleDestroyed = (inner, eArgs) =>
            {
                if (subscription != null)
                {
                    Unsubscribe(subscription);
                    subscription = null;
                }
            };

            //Attach to events and subscribe on message, if handle is already created
            target.Loaded += onHandleCreated;
            target.Unloaded += onHandleDestroyed;
            if (target.IsLoaded)
            {
                subscription = Subscribe(messageHandler);
            }
        }

        /// <summary>
        /// Subscribes to the given MessageType during the livetime of the given control.
        /// Events OnHandleCreated and OnHandleDestroyed are used for subscribing / unsubscribing.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="messageHandler">Action to perform on incoming message.</param>
        /// <param name="target">The target control.</param>
        public void SubscribeOnControl<MessageType>(System.Windows.Forms.Control target, Action<MessageType> messageHandler)
            where MessageType : ApplicationMessage
        {
            MessageSubscription subscription = null;

            //Create handler delegates
            EventHandler onHandleCreated = (sender, eArgs) =>
            {
                if (subscription == null)
                {
                    subscription = Subscribe(messageHandler);
                }
            };
            EventHandler onHandleDestroyed = (inner, eArgs) =>
            {
                if (subscription != null)
                {
                    Unsubscribe(subscription);
                    subscription = null;
                }
            };

            //Attach to events and subscribe on message, if handle is already created
            target.HandleCreated += onHandleCreated;
            target.HandleDestroyed += onHandleDestroyed;
            if (target.IsHandleCreated)
            {
                subscription = Subscribe(messageHandler);
            }
        }
#endif

        /// <summary>
        /// Clears the given MessageSubscription.
        /// </summary>
        /// <param name="messageSubscription">The subscription to clear.</param>
        public void Unsubscribe(MessageSubscription messageSubscription)
        {
            if (!messageSubscription.IsDisposed)
            {
                Type messageType = messageSubscription.MessageType;

                //Remove subscription from internal list
                if (m_messageSubscriptions.ContainsKey(messageType))
                {
                    List<MessageSubscription> listOfSubscriptions = m_messageSubscriptions[messageType];
                    listOfSubscriptions.Remove(messageSubscription);
                    if (listOfSubscriptions.Count == 0)
                    {
                        m_messageSubscriptions.Remove(messageType);
                    }
                }

                //Clear given subscription
                messageSubscription.Clear();
            }
        }

        /// <summary>
        /// Sends the given message to all subscribers.
        /// </summary>
        public void Publish<MessageType>()
            where MessageType : ApplicationMessage, new()
        {
            Publish<MessageType>(new MessageType());
        }

        /// <summary>
        /// Sends the given message to all subscribers.
        /// </summary>
        /// <typeparam name="MessageType">Type of the message.</typeparam>
        /// <param name="message">The message to send.</param>
        public void Publish<MessageType>(MessageType message)
            where MessageType : ApplicationMessage
        {
            List<Exception> occurredExceptions = null;

            //Notify all subscribed targets
            Type currentType = typeof(MessageType);
            if (m_messageSubscriptions.ContainsKey(currentType))
            {
                List<MessageSubscription> subscriptions = m_messageSubscriptions[currentType];
                for (int loop = 0; loop < subscriptions.Count; loop++)
                {
                    MessageSubscription actSubscription = subscriptions[loop];
                    try
                    {
                        actSubscription.Publish(message);
                    }
                    catch (Exception ex)
                    {
                        if (occurredExceptions == null) { occurredExceptions = new List<Exception>(); }
                        occurredExceptions.Add(ex);
                    }
                }
            }

            //Notify all exceptions occurred during publish progress
            if ((occurredExceptions != null) &&
               (occurredExceptions.Count > 0))
            {
                throw new MessagePublishException(typeof(MessageType), occurredExceptions);
            }
        }
    }
}
