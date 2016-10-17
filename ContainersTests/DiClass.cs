using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainersTests
{
    public class DiClass
    {
        private readonly ITest1 _test1;

        public DiClass(ITest1 test1)
        {
            _test1 = test1;
        }

        public ITest1 GeTest1()
        {
            return _test1;
        }
    }
}
