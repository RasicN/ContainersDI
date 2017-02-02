using System;
using System.Collections.Generic;

namespace Containers
{
    public class Config : ConfigBase
    {
    }
    public class Config<TBind> : ConfigBase
    {
        public Config(object toInstance) : base(typeof(TBind), toInstance)
        {
            Bind = typeof(TBind);
            To = null;
        }
    }

    public class Config<TBind, TTo> : ConfigBase
    {
        public Config() : base(typeof(TBind), typeof(TTo))
        {
            Bind = typeof(TBind);
            To = typeof(TTo);
        }
    }
    
    public class SelfConfig : ConfigBase
    {
        public SelfConfig(Type self, IDictionary<string, Type> constructorArguments = null)
        {
            Bind = self;
            To = self;
            ConstructorArguments = constructorArguments ?? new Dictionary<string, Type>();
        }

        public IDictionary<string, Type> ConstructorArguments { get; set; }
    }

    public class ConfigBase : IConfig
    {
        public ConfigBase(Type bind, Type to)
        {
            Bind = bind;
            To = to;
            SetConfigNameDefault();
        }

        public ConfigBase()
        {
            SetConfigNameDefault();
        }

        protected ConfigBase(Type bind, object toInstance)
        {
            Bind = bind;
            ToInstance = toInstance;
            SetConfigNameDefault();
        }

        public Type Bind { get; set; }
        public Type To { get; set; }
        public bool IsSingleton { get; set; }
        public object ToInstance { get; set; }

        public string ConfigName { get; set; }
        public bool IsThreadInstance { get; set; }

        private void SetConfigNameDefault()
        {
            ConfigName = To != null ? To.ToString() : ToInstance?.ToString();
        }
    }

    public static class ConfigBaseExtensions
    {
        public static ConfigBase AsSingleton(this ConfigBase configBase)
        {
            configBase.IsSingleton = true;
            configBase.IsThreadInstance = false;
            return configBase;
        }

        public static ConfigBase AsThreadInstance(this ConfigBase configBase)
        {
            configBase.IsSingleton = false;
            configBase.IsThreadInstance = true;
            return configBase;
        }

        public static ConfigBase WithName(this ConfigBase configBase, string name)
        {
            configBase.ConfigName = name;
            return configBase;
        }

        public static SelfConfig ResolveConstructorArgumentUsing<T>(this SelfConfig config, string paramName)
        {
            config.ConstructorArguments.Add(paramName, typeof(T));
            return config;
        }
    }

}
