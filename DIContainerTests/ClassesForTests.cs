using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
