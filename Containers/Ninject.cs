using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Syntax;

namespace Containers
{
    internal class Ninject
    {
        public class NInjectInstanceProvider : IInstanceProvider, IContractBehavior
        {
            private readonly IKernel kernel;

            public NInjectInstanceProvider(IKernel kernel)
            {
                if (kernel == null) throw new ArgumentNullException("kernel");
                this.kernel = kernel;
            }

            public object GetInstance(InstanceContext instanceContext, Message message)
            {
                return GetInstance(instanceContext);
            }

            public object GetInstance(InstanceContext instanceContext)
            {
                return kernel.Get(instanceContext.Host.Description.ServiceType);
            }

            public void ReleaseInstance(InstanceContext instanceContext, object instance)
            {
                kernel.Release(instance);
            }

            public void AddBindingParameters(ContractDescription contractDescription,
                ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ContractDescription contractDescription,
                ServiceEndpoint endpoint, ClientRuntime clientRuntime)
            {
            }

            public void ApplyDispatchBehavior(ContractDescription contractDescription,
                ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
            {
                dispatchRuntime.InstanceProvider = this;
            }

            public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
            {
            }
        }

        public class NInjectServiceHost : ServiceHost
        {
            public NInjectServiceHost(IKernel kernel, Type serviceType, params Uri[] baseAddresses)
                : base(serviceType, baseAddresses)
            {
                if (kernel == null) throw new ArgumentNullException("kernel");
                foreach (var cd in ImplementedContracts.Values)
                {
                    cd.Behaviors.Add(new NInjectInstanceProvider(kernel));
                }
            }
        }

        public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
        {
            private readonly IKernel _kernel;

            public NinjectDependencyResolver(IKernel kernel) : base(kernel)
            {
                _kernel = kernel;
            }

            public IDependencyScope BeginScope()
            {
                return new NinjectDependencyScope(_kernel.BeginBlock());
            }
        }

        public class NinjectDependencyScope : IDependencyScope
        {
            private IResolutionRoot _resolver;

            public NinjectDependencyScope(IResolutionRoot resolver)
            {
                _resolver = resolver;
            }

            public object GetService(Type serviceType)
            {
                if (_resolver == null)
                    throw new ObjectDisposedException("this", "This scope has been disposed");

                return _resolver.TryGet(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                if (_resolver == null)
                    throw new ObjectDisposedException("this", "This scope has been disposed");

                return _resolver.GetAll(serviceType);
            }

            public void Dispose()
            {
                var disposable = _resolver as IDisposable;
                disposable?.Dispose();

                _resolver = null;
            }
        }
    }
    
}
