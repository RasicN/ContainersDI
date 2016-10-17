using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace Containers
{
    public class Container
    {
        private readonly ICollection<IConfig> _configs;
        internal StandardKernel Kernel;
        public Container(ICollection<IConfig> configs)
        {
            _configs = configs;
            Kernel = CreateBindings(configs);
        }

        public Container()
        {
            _configs = new List<IConfig>();
        }

        public T GetBinding<T>(string name = null)
        {
            var config =_configs.First(x => x.Bind == typeof(T));
            
            if (string.IsNullOrEmpty(name))
            {
                return (T)Kernel.Get(config.Bind);
            }

            return (T)Kernel.Get(config.Bind, name);
        }

        public Container AddConfig(IConfig config)
        {
            _configs.Add(config);
            CreateBindings(new List<IConfig> {config});

            return this;
        }

        private StandardKernel CreateBindings(ICollection<IConfig> configuration)
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

                if (config.IsSingleton)
                {
                    binding.InSingletonScope();
                }
                if (config.ConfigName != null)
                {
                    binding.Named(config.ConfigName);
                }
            }
            return Kernel;
        }

        private IBindingWhenInNamedWithOrOnSyntax<object> ToInstanceBinding(IConfig config)
        {
            return Kernel.Bind(config.Bind).ToMethod(x => config.ToInstance);
        }

        private IBindingWhenInNamedWithOrOnSyntax<object> BasicBinding(IConfig config)
        {
            return Kernel.Bind(config.Bind).To(config.To);
        }
    }

    public class WcfContainer : Container
    {
        public WcfContainer(ICollection<IConfig> configs) : base(configs)
        {
        }

        public ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddress)
        {
            return new Ninject.NInjectServiceHost(Kernel, serviceType, baseAddress);
        }
    }

    public class WebApiContainer : Container
    {
        public WebApiContainer(ICollection<IConfig> configs) : base(configs)
        {
        }

        public void SetDependencyResolver()
        {
            GlobalConfiguration.Configuration.DependencyResolver = new Ninject.NinjectDependencyResolver(Kernel);
        }
    }
}
