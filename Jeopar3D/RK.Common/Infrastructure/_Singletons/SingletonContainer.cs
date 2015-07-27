using System;
using System.Collections.Generic;

namespace RK.Common.Infrastructure
{
    public class SingletonContainer
    {
        private Dictionary<Type, object> m_singletons;
        private Dictionary<string, object> m_singletonsByName;
        private object m_singletonsLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonContainer" /> class.
        /// </summary>
        public SingletonContainer()
        {
            m_singletons = new Dictionary<Type, object>();
            m_singletonsByName = new Dictionary<string, object>();
            m_singletonsLock = new object();
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        public void RegisterSingleton<T>()
            where T : class, new()
        {
            this.RegisterSingleton(new T());
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        /// <param name="singletonObject">The object to register.</param>
        public void RegisterSingleton<T>(T singletonObject)
            where T : class
        {
            if (singletonObject == null) { throw new ArgumentNullException("singletonObject"); }

            lock (m_singletonsLock)
            {
                if (m_singletons.ContainsKey(typeof(T))) { throw new InvalidOperationException("There is already a singleton registered for type " + typeof(T).FullName + "!"); }

                m_singletons[typeof(T)] = singletonObject;
                m_singletonsByName[typeof(T).Name] = singletonObject;
            }
        }

        /// <summary>
        /// Registers a singleton on the given name.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        /// <param name="singletonObject">The object to register.</param>
        /// <param name="name">The name for the singleton</param>
        public void RegisterSingleton<T>(T singletonObject, string name)
        {
            lock (m_singletonsLock)
            {
                if (m_singletonsByName.ContainsKey(name)) { throw new InvalidOperationException("There is already a singleton registered for name " + name + "!"); }

                m_singletonsByName[name] = singletonObject;
            }
        }

        /// <summary>
        /// Is there any singleton with the given name?
        /// </summary>
        /// <param name="singletonName">The name of the singleton.</param>
        public bool ContainsSingleton(string singletonName)
        {
            lock (m_singletonsLock)
            {
                return m_singletonsByName.ContainsKey(singletonName);
            }
        }

        /// <summary>
        /// Gets the singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton.</typeparam>
        public T GetSingleton<T>()
            where T : class
        {
            lock (m_singletonsLock)
            {
                return m_singletons[typeof(T)] as T;
            }
        }

        /// <summary>
        /// Is there any singleton with the given type?
        /// </summary>
        /// <param name="singletonName">The type of the singleton.</param>
        internal bool ContainsSingleton(Type type)
        {
            lock (m_singletonsLock)
            {
                return m_singletons.ContainsKey(type);
            }
        }

        /// <summary>
        /// Gets the singleton object of the given type.
        /// </summary>
        /// <param name="typeOfSingleton">The type of the singleton.</param>
        public object this[Type typeOfSingleton]
        {
            get
            {
                lock (m_singletonsLock)
                {
                    return m_singletons[typeOfSingleton];
                }
            }
        }

        /// <summary>
        /// Gets the singleton with the given name.
        /// </summary>
        /// <param name="name">The name of the singleton.</param>
        public object this[string name]
        {
            get
            {
                lock (m_singletonsLock)
                {
                    return m_singletonsByName[name];
                }
            }
        }
    }
}