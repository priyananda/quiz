using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using RK.Common.Util;

#if WINRT
using Windows.UI.Xaml;
#endif

namespace RK.Common.Infrastructure
{
    public class RKApplication
    {
        private static RKApplication s_current;

        private Assembly m_mainAssembly;
        private string[] m_startupArguments;
        private ApplicationMessageHandler m_uiMessageHandler;
        private Dictionary<Type, object> m_services;
        private Bootstrapper m_bootstrapper;
        private SingletonContainer m_singletons;
        private bool m_booted;

        /// <summary>
        /// Prevents a default instance of the <see cref="RKApplication" /> class from being created.
        /// </summary>
        private RKApplication()
        {
            m_uiMessageHandler = new ApplicationMessageHandler();
            m_services = new Dictionary<Type, object>();
            m_bootstrapper = new Bootstrapper();
            m_singletons = new SingletonContainer();
        }

        /// <summary>
        /// Initializes the RKApplication object.
        /// </summary>
        /// <param name="mainAssembly">The main assembly of the application.</param>
        public static void Initialize(Assembly mainAssembly, string[] startupArguments)
        {
            if (s_current != null) { throw new CommonLibraryException("RKApplication is already initialized!"); }

            //Do all initializations
            RKApplication newApplication = new RKApplication();
            newApplication.m_mainAssembly = mainAssembly;
            newApplication.m_startupArguments = startupArguments;

            //Apply created instance
            s_current = newApplication;
        }

        /// <summary>
        /// Creates all default services.
        /// </summary>
        /// <param name="mainWindow">The mainwindow instance.</param>
        public void CreateDefaultServices(Window mainWindow)
        {
            //TODO..
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        public void RegisterSingleton<T>()
            where T : class, new()
        {
            m_singletons.RegisterSingleton<T>();
        }

        /// <summary>
        /// Registers a new singleton of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the singleton to create an object of.</typeparam>
        /// <param name="singletonObject">The object to register.</param>
        public void RegisterSingleton<T>(T singletonObject)
            where T : class
        {
            m_singletons.RegisterSingleton(singletonObject);
        }

        /// <summary>
        /// Registers a new item for the bootstraper.
        /// </summary>
        /// <typeparam name="T">The type of the item to register.</typeparam>
        public void RegisterBootstrapperItem<T>()
            where T : IBootstrapperItem, new()
        {
            m_bootstrapper.RegisterBootstrapperItem<T>();
        }

        /// <summary>
        /// Registers a new item for the bootstraper.
        /// </summary>
        /// <typeparam name="T">The type of the item to register.</typeparam>
        /// <param name="bootstrapperItemToAdd">The item to register.</param>
        public void RegisterBootstrapperItem<T>(T bootstrapperItemToAdd)
            where T : IBootstrapperItem
        {
            m_bootstrapper.RegisterBootstrapperItem(bootstrapperItemToAdd);
        }

        /// <summary>
        /// Registers the given service.
        /// </summary>
        /// <typeparam name="T">The type of the service to register.</typeparam>
        /// <param name="service">The service object to register.</param>
        public void RegisterService<T>(T service)
            where T : class
        {
            Type serviceType = typeof(T);

            if (!serviceType.GetTypeInfo().IsInterface) { throw new CommonLibraryException("Service type musst be an interface!"); }
            if (service == null) { throw new ArgumentNullException("service"); }

            m_services[serviceType] = service;
        }

        /// <summary>
        /// Tries to get the service of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service to get.</typeparam>
        public T TryGetService<T>()
            where T : class
        {
            Type serviceType = typeof(T);

            if (!serviceType.GetTypeInfo().IsInterface) { throw new CommonLibraryException("Service type musst be an interface!"); }

            //Tries to return the service of the given service type
            if (m_services.ContainsKey(serviceType)) { return m_services[serviceType] as T; }
            else { return null; }
        }

        /// <summary>
        /// Gets the service of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        public T GetService<T>()
            where T : class
        {
            T result = TryGetService<T>();
            if (result == null) { throw new CommonLibraryException("Service " + typeof(T).FullName + " not found!"); }
            return result;
        }

        /// <summary>
        /// Gets the current instance of this application.
        /// </summary>
        public static RKApplication Current
        {
            get
            {
                if (s_current == null)
                {
                    throw new CommonLibraryException("RKApplication object not initialized!");
                }
                return s_current;
            }
        }

        /// <summary>
        /// Is the application initialized?
        /// </summary>
        public static bool IsInitialized
        {
            get { return s_current != null; }
        }

        /// <summary>
        /// Gets the name of this product.
        /// </summary>
        public string ProductName
        {
            get
            {
                AssemblyProductAttribute productAttribute = m_mainAssembly.GetCustomAttribute(typeof(AssemblyProductAttribute))
                    as AssemblyProductAttribute;
                if (productAttribute != null)
                {
                    return productAttribute.Product;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the version of this product.
        /// </summary>
        public string ProductVersion
        {
            get
            {
                AssemblyInformationalVersionAttribute versionAttribute = m_mainAssembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute))
                    as AssemblyInformationalVersionAttribute;
                if (versionAttribute != null)
                {
                    return versionAttribute.InformationalVersion;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the message handler of the ui.
        /// </summary>
        public ApplicationMessageHandler UIMessageHandler
        {
            get { return m_uiMessageHandler; }
        }

        /// <summary>
        /// Gets the application's bootstrapper.
        /// </summary>
        public Bootstrapper Bootstrapper
        {
            get { return m_bootstrapper; }
        }

        /// <summary>
        /// Gets a container holding all registered singletons.
        /// </summary>
        public SingletonContainer Singletons
        {
            get { return m_singletons; }
        }
    }
}