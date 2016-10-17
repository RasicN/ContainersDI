namespace ContainersTests
{
    public class Test1 : ITest1
    {
        public void TestMethod()
        {
            
        }

        public string TestString => "Hello World";
    }

    public class Test1A : ITest1
    {
        public void TestMethod()
        {

        }

        public string TestString => "Hello World From Test1 A";
    }
}
