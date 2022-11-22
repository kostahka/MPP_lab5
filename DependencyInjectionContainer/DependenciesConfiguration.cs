using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        public List<DependencyNode> dependencies = new List<DependencyNode>();

        public void Register<T1, T2>(bool isSingleton = false)
        {
            ConfRegister(typeof(T1), typeof(T2), isSingleton);
        }

        public void Register(Type dependency, Type implementation, bool isSingleton = false)
        {
            ConfRegister(dependency, implementation, isSingleton);
        }

        private void ConfRegister(Type dependency, Type implementation, bool isSingleton = false)
        {
            if(dependencies.Any( d => d.DependencyType == dependency && d.ImplementationType == implementation))
            {
                dependencies.Where(d => d.DependencyType == dependency && d.ImplementationType == implementation).First().IsSingleton = isSingleton;
            }
            else
            {
                var node = new DependencyNode(dependency, implementation, isSingleton);
                dependencies.Add(node);
            }
        }
    }
}
