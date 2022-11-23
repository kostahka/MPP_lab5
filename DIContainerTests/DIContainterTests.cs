using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DependencyInjectionContainer;
using System.Collections.Generic;
using System.Linq;
namespace DIContainerTests
{
    [TestClass]
    public class DIContainterTests
    {
        private DependenciesConfiguration config;
        private DependencyProvider provider;

        [TestInitialize]
        public void SetUp()
        {
            config = new DependenciesConfiguration();
        }

        [TestMethod]
        public void TestInterfaceClass()
        {
            config.Register<IInterface1, Class1>();
            provider = new DependencyProvider(config);
            var actual = provider.Resolve<IInterface1>();
            actual.Method1();
        }
        [TestMethod]
        public void TestSingletonClass()
        {
            config.Register<IInterface1, Class1>(isSingleton: true);
            provider = new DependencyProvider(config);
            var actual = provider.Resolve<IInterface1>();
            var expected = provider.Resolve<IInterface1>();
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestIEnumerableClasses()
        {
            config.Register<IInterface1, Class1>();
            config.Register<IInterface1, Class2>();
            provider = new DependencyProvider(config);
            var actual = provider.Resolve<IEnumerable<IInterface1>>();
            Assert.AreEqual(2, actual.ToArray().Length);
        }
        [TestMethod]
        public void TestAbstractClass()
        {
            config.Register<AbstractClass, Class3>();
            provider = new DependencyProvider(config);
            var actual = provider.Resolve<AbstractClass>();
            actual.Method2();
        }
        [TestMethod]
        public void TestReflectedClass()
        {
            config.Register<Class3, Class4>();
            provider = new DependencyProvider(config);
            var actual = provider.Resolve<Class3>();
            actual.Method2();
        }
        [TestMethod]
        public void TestGenericClass()
        {
            config.Register(typeof(IService<>), typeof(GenericClass<>));
            provider = new DependencyProvider(config);
            var obj = provider.Resolve<GenericClass<IRepository>>();
            var actual = obj.ServiceMethod();
            IRepository expected = default;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestConstructorClass()
        {
            config.Register<IInterface2, Class5>();
            config.Register(typeof(IService<>), typeof(GenericClass<>));
            provider = new DependencyProvider(config);
            var obj = provider.Resolve<IInterface2>();
            var actual = obj.RetServ();
            IRepository expected = default;
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void TestIncorectDependency()
        {
            bool pass = false;
            config.Register<AbstractClass, AbstractClass2>();
            try
            {
                provider = new DependencyProvider(config);
            }
            catch(Exception e)
            {
                pass = true;
            }
            Assert.IsTrue(pass);
        }
    }
}
