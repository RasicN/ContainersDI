using System;
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
        [TestInitialize]
        public void Setup()
        {
            Container.Initialize(new List<IConfig>());
        }

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
            Container.Initialize(configs);

            // Assert
            var test1 = Container.GetBinding<ITest1>();
            Assert.IsNotNull(test1);
            Assert.IsFalse(string.IsNullOrWhiteSpace(test1.TestString));
            Assert.IsNotNull(Container.GetBinding<ITest2>());
            Assert.IsNotNull(Container.GetBinding<ITest3>());
            Assert.IsNotNull(Container.GetBinding<ITest4>());
        }

        [TestMethod]
        public void CanAddBinding()
        {
            // Act
            Container.AddConfig(new Config<ITest1, Test1>());
            Container.AddConfig(new Config<ITest2>(new Test2()));

            // Assert
            var test1 = Container.GetBinding<ITest1>();
            Assert.IsNotNull(test1);
            Assert.IsFalse(string.IsNullOrWhiteSpace(test1.TestString));
            Assert.IsNotNull(Container.GetBinding<ITest2>());
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

            var Container = new WebApiContainer(configs);
            
            // Act
            Container.SetDependencyResolver();

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
            Container.Initialize(configs);

            // Assert
            var test1 = Container.GetBinding<ITest1>("Test1");
            var test1A = Container.GetBinding<ITest1>("asdf");

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
            Container.Initialize(configs);

            var test5 = Container.GetBinding<Test5>();

            // Assert
            Assert.IsInstanceOfType(test5.Test1, typeof(Test1));
            Assert.IsInstanceOfType(test5.Test1A, typeof(Test1A));
            Assert.IsNotNull(test5.Test2);
        }

        [TestMethod]
        public void CanChainAddBindings()
        {
            Container.AddConfig(new Config<ITest1, Test1>());
            Container.AddConfig(new Config<ITest2>(new Test2()));
            Container.AddConfig(new Config<ITest3, Test3>().AsSingleton());
            Container.AddConfig(new Config<ITest4>(new Test4()).AsSingleton());

            var test1 = Container.GetBinding<ITest1>();
            Assert.IsNotNull(test1);
            Assert.IsFalse(string.IsNullOrWhiteSpace(test1.TestString));
            Assert.IsNotNull(Container.GetBinding<ITest2>());
            Assert.IsNotNull(Container.GetBinding<ITest3>());
            Assert.IsNotNull(Container.GetBinding<ITest4>());
        }

        [TestMethod]
        public void CanOverwriteConfig()
        {
            // Arrange
           Container.AddConfig(new Config<ITest1, Test1>());

            // Act
            Container.OverwriteConfig(new Config<ITest1, Test1A>());

            // Assert
            var test1 = Container.GetBinding<ITest1>();
            var expectedTestString = new Test1A().TestString;

            Assert.AreEqual(expectedTestString, test1.TestString);
        }
    }
}
