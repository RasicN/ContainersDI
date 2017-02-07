using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Http;
using Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Container = Containers.Container;

namespace ContainersTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void BindingDictionaryReturnsImplementationOfPassedInConfig()
        {
            // Arrange
            var configs = new List<IConfig>
            {
                new Config<ITest1, Test1>(),
                new Config<ITest2>(new Test2()),
                new Config<ITest3, Test3>().AsSingleton(),
                new Config<ITest4>(new Test4()).AsSingleton()
            };

            // Act
            var container = new Container(configs);

            // Assert
            var test1 = container.GetBinding<ITest1>();
            Assert.IsNotNull(test1);
            Assert.IsFalse(string.IsNullOrWhiteSpace(test1.TestString));
            Assert.IsNotNull(container.GetBinding<ITest2>());
            Assert.IsNotNull(container.GetBinding<ITest3>());
            Assert.IsNotNull(container.GetBinding<ITest4>());
        }

        [TestMethod]
        public void CanAddBinding()
        {
            // Arrange
            var container = new Container();
            
            // Act
            container.AddConfig(new Config<ITest1, Test1>());
            container.AddConfig(new Config<ITest2>(new Test2()));

            // Assert
            var test1 = container.GetBinding<ITest1>();
            Assert.IsNotNull(test1);
            Assert.IsFalse(string.IsNullOrWhiteSpace(test1.TestString));
            Assert.IsNotNull(container.GetBinding<ITest2>());
        }

        [TestMethod]
        public void CanGetDependencyResolver()
        {
            // Arrange
            var configs = new List<IConfig>
            {
                new Config<ITest1, Test1>(),
                new Config<ITest2>(new Test2()),
                new Config<ITest3, Test3>().AsSingleton(),
                new Config<ITest4>(new Test4()).AsSingleton()
            };

            var container = new WebApiContainer(configs);
            
            // Act
            container.SetDependencyResolver();

            // Assert
            Assert.IsNotNull(GlobalConfiguration.Configuration.DependencyResolver);
        }

        [TestMethod]
        public void CanDetermineBetweenTwoImplementations()
        {
            // Arrange
            var configs = new List<IConfig>
            {
                new Config<ITest1, Test1>().WithName("Test1"),
                new Config<ITest1, Test1A>().WithName("asdf")
            };

            // Act
            var container = new Container(configs);

            // Assert
            var test1 = container.GetBinding<ITest1>("Test1");
            var test1A = container.GetBinding<ITest1>("asdf");

            Assert.AreEqual("Hello World", test1.TestString);
            Assert.AreEqual("Hello World From Test1 A", test1A.TestString);
        }

        [TestMethod]
        public void CanChangeInstanceOfInterfaceUsedDependingOnConfig()
        {
            // Arrange
            var configs = new List<IConfig>
            {
                new Config<ITest1, Test1>().WithName("Test1"),
                new Config<ITest1, Test1A>().WithName("asdf"),
                new SelfConfig(typeof(Test5))
                    .ResolveConstructorArgumentUsing<Test1>("test1")
                    .ResolveConstructorArgumentUsing<Test1A>("test1A"),
                new Config<ITest2, Test2>()
            };

            // Act
            var container = new Container(configs);

            var test5 = container.GetBinding<Test5>();

            // Assert
            Assert.IsInstanceOfType(test5.Test1, typeof(Test1));
            Assert.IsInstanceOfType(test5.Test1A, typeof(Test1A));
            Assert.IsNotNull(test5.Test2);
        }

        [TestMethod]
        public void CanChainAddBindings()
        {
            var container = new Container()
                .AddConfig(new Config<ITest1, Test1>())
                .AddConfig(new Config<ITest2>(new Test2()))
                .AddConfig(new Config<ITest3, Test3>().AsSingleton())
                .AddConfig(new Config<ITest4>(new Test4()).AsSingleton());

            var test1 = container.GetBinding<ITest1>();
            Assert.IsNotNull(test1);
            Assert.IsFalse(string.IsNullOrWhiteSpace(test1.TestString));
            Assert.IsNotNull(container.GetBinding<ITest2>());
            Assert.IsNotNull(container.GetBinding<ITest3>());
            Assert.IsNotNull(container.GetBinding<ITest4>());
        }

        [TestMethod]
        public void CanOverwriteConfig()
        {
            // Arrange
            var container = new Container()
                .AddConfig(new Config<ITest1, Test1>());

            // Act
            container.OverwriteConfig(new Config<ITest1, Test1A>());

            // Assert
            var test1 = container.GetBinding<ITest1>();
            var expectedTestString = new Test1A().TestString;

            Assert.AreEqual(expectedTestString, test1.TestString);
        }
    }
}
