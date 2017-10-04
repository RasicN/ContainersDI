using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Mvc;
using Ninject;
using Ninject.Syntax;
using Ninject.Web.Common;

namespace Containers
{
    public static class Container
    {
        private static ICollection<IConfig> _configs = new List<IConfig>();
        internal static StandardKernel Kernel;

        
        public static void Initialize(ICollection<IConfig> configs)
        {
            Kernel = null;
            _configs = configs;
            CreateBindings(configs);
        }

        public static T GetBinding<T>(string name = null)
        {
            var config = _configs.First(x => x.Bind == typeof(T));
            
            if (string.IsNullOrEmpty(name))
            {
                return (T)Kernel.Get(config.Bind);
            }

            return (T)Kernel.Get(config.Bind, name);
        }

        public static void AddConfig(IConfig config)
        {
            _configs.Add(config);
            CreateBindings(new List<IConfig> {config});
        }

        public static void OverwriteConfig(IConfig config, string name = null)
        {
            if (config.To != null)
            {
                Kernel.Rebind(config.Bind).To(config.To);
            }
            else
            {
                Kernel.Rebind(config.Bind).ToMethod(x => config.ToInstance);
            }
        }

        private static void CreateBindings(ICollection<IConfig> configuration)
        {
            if (Kernel == null)
            {
                Kernel = new StandardKernel();
            }

            foreach (var config in configuration)
            {
                var binding = config.To != null ? BasicBinding(config) : ToInstanceBinding(config);

                if (config.GetType() == typeof(SelfConfig))
                {
                    var selfConfig = (SelfConfig)config;
                    foreach (var selfConfigConstructorArgument in selfConfig.ConstructorArguments)
                    {
                        binding.WithConstructorArgument(selfConfigConstructorArgument.Key,
                            x => x.Kernel.Get(selfConfigConstructorArgument.Value, selfConfig.ConfigName));
                    }
                }

                SetScope(config, binding);

                if (config.ConfigName != null)
                {
                    binding.Named(config.ConfigName);
                }
            }
        }

        private static void SetScope(IConfig config, IBindingWhenInNamedWithOrOnSyntax<object> binding)
        {
            binding.InRequestScope();

            if (config.IsSingleton)
            {
                binding.InSingletonScope();
            }
            else if (config.IsThreadInstance)
            {
                binding.InThreadScope();
            }
            else if (config.IsTransientScope)
            {
                binding.InTransientScope();
            }
        }

        private static IBindingWhenInNamedWithOrOnSyntax<object> ToInstanceBinding(IConfig config)
        {
            return Kernel.Bind(config.Bind).ToMethod(x => config.ToInstance);
        }

        private static IBindingWhenInNamedWithOrOnSyntax<object> BasicBinding(IConfig config)
        {
            return Kernel.Bind(config.Bind).To(config.To);
        }
    }

    public class WcfContainer
    {
        public WcfContainer()
        {
        }

        public WcfContainer (ICollection<IConfig> configs)
        {
            Container.Initialize(configs);
        }

        public ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddress)
        {
            return new Ninject.NInjectServiceHost(Container.Kernel, serviceType, baseAddress);
        }
    }

    public class WebApiContainer
    {
        public WebApiContainer() { }

        public WebApiContainer(ICollection<IConfig> configs)
        {
            Container.Initialize(configs);
        }

        public void SetDependencyResolver()
        {
            GlobalConfiguration.Configuration.DependencyResolver = new Ninject.NinjectDependencyResolver(Container.Kernel);
        }
    }

    public class MvcContainer
    {
        public MvcContainer() { }

        public MvcContainer(ICollection<IConfig> configs)
        {
            Container.Initialize(configs);
        }

        public void SetDependencyResolver()
        {
            DependencyResolver.SetResolver(new Ninject.NinjectDependencyResolver(Container.Kernel));
        }
    }

    public class WebContainer
    {
        public WebContainer() { }

        public WebContainer(ICollection<IConfig> configs)
        {
            Container.Initialize(configs);
        }

        public void SetDependencyResolver()
        {
            var configurationDependencyResolver = new Ninject.NinjectDependencyResolver(Container.Kernel);

            GlobalConfiguration.Configuration.DependencyResolver = configurationDependencyResolver;
            DependencyResolver.SetResolver(configurationDependencyResolver);
        }
    }
}
