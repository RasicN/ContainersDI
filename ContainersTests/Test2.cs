namespace ContainersTests
{
    public class Test2 : ITest2
    {
        public Test2()
        {
            TestValue = "Test Value";
        }

        public string TestValue { get; set; }
    }
}
