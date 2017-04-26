using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainersTests
{
    public class Test6 : ITest6
    {
        public string Name { get; set; }

        public Test6(ITest6Dependency test)
        {
            Name = test.Name;
        }
    }

    public interface ITest6
    {
        string Name { get; set; }
    }

    public class Test6Dependency : ITest6Dependency
    {
        public Test6Dependency(Type name)
        {
            Name = name.ToString();
        }

        public string Name { get; set; }
    }

    public interface ITest6Dependency
    {
        string Name { get; set; }
    }
}
