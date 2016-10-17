using System.Collections.Generic;

namespace $rootnamespace$
{
    public static class ContainerConfig
    {
        public static ICollection<IConfig> Configs => new List<IConfig>
        {
            // Fill in configs here
            // new Config<IType, Type>();
            // new Config<IType>(new Type());
        };
    }
}
