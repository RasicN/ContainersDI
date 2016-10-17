using System;

namespace Containers
{
    public interface IConfig
    {
        Type Bind { get; set; }
        bool IsSingleton { get; set; }
        Type To { get; set; }
        object ToInstance { get; set; }
        string ConfigName { get; set; }
    }
}