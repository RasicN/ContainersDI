using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace $rootnamespace$
{
    public class ContainerConfig : ServiceHostFactory
    {
        public static ICollection<IConfig> Configs => new List<IConfig>
        {
            // Fill in configs here
            // new Config<IType, Type>();
            // new Config<IType>(new Type());
        };

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new WcfContainer(Configs).CreateServiceHost(serviceType, baseAddresses);
        }
    }
}
