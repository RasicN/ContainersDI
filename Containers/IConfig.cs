using System;

namespace Containers
{
    public interface IConfig
    {
        Type Bind { get; set; }
        
        Type To { get; set; }
        object ToInstance { get; set; }
        string ConfigName { get; set; }
        bool IsSingleton { get; set; }
        bool IsThreadInstance { get; set; }
        bool IsTransientScope { get; set; }
    }
}