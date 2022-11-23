using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DependencyInjectionContainer;

namespace DIContainerTests
{
    interface IInterface1
    {
        void Method1();
    }
    class Class1 : IInterface1
    {
        public void Method1()
        {
            return;
        }
    }
    class Class2 : IInterface1
    {
        public void Method1()
        {
            return;
        }
    }
    abstract class AbstractClass
    {
        public abstract void Method2();
    }
    abstract class AbstractClass2 : AbstractClass
    {
        
    }
    class Class3 : AbstractClass
    {
        override public void Method2()
        {

        }
    }
    class Class4 : Class3
    {
        public override void Method2()
        {
            base.Method2();
        }
    }

    interface IRepository
    {
        void RepMethod();
    }

    interface IService<T>
        where T : IRepository
    {
        T ServiceMethod();
    }

    class GenericClass<T> : IService<T>
        where T : IRepository
    {
        public T ServiceMethod()
        {
            return default;
        }
    }

    interface IInterface2
    {
        IRepository RetServ();
    }
    class Class5 : IInterface2
    {
        IService<IRepository> service;
        public Class5(IService<IRepository> service)
        {
            this.service = service;
        }
        public IRepository RetServ()
        {
            return service.ServiceMethod();
        }
    }

    enum Interface1Implementations
    {
        First, Second
    }
    enum Interface2Implementations
    {
        First, Second
    }
    enum Interface3Implementations
    {
        First, Second
    }
    interface IInterface3
    {
        int RetInt();
    }
    class ClassInt3 : IInterface3
    {
        public int RetInt()
        {
            return 3;
        }
    }
    class ClassInt4 : IInterface3
    {
        public int RetInt()
        {
            return 4;
        }
    }
    class ClassInt : IInterface3
    {
        IInterface3 interface3;
        public ClassInt([DependencyKey(Interface3Implementations.Second)]IInterface3 i)
        {
            interface3 = i;
        }
        public int RetInt()
        {
            return interface3.RetInt();
        }
    }
}
