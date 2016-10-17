namespace ContainersTests
{
    public class Test5
    {
        public Test5(ITest1 test1, ITest1 test1A, ITest2 test2)
        {
            Test1 = test1;
            Test1A = test1A;
            Test2 = test2;
        }

        public ITest1 Test1 { get; }
        public ITest1 Test1A { get; }
        public ITest2 Test2 { get; set; }
    }
}
