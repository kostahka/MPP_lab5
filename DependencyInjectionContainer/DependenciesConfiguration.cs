using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        public Dictionary<object, List<DependencyNode>> dependencies = new Dictionary<object, List<DependencyNode>>();

        public void Register<T1, T2>(object enumVal = null, bool isSingleton = false)
        {
            ConfRegister(typeof(T1), typeof(T2), enumVal, isSingleton);
        }

        public void Register(Type dependency, Type implementation, object enumVal = null, bool isSingleton = false)
        {
            ConfRegister(dependency, implementation, isSingleton);
        }

        private void ConfRegister(Type dependency, Type implementation, object enumVal, bool isSingleton = false)
        {
            if (!dependencies.ContainsKey(enumVal))
                dependencies.Add(enumVal, new List<DependencyNode>());
            if(dependencies[enumVal].Any( d => d.DependencyType == dependency && d.ImplementationType == implementation))
            {
                dependencies[enumVal].Where(d => d.DependencyType == dependency && d.ImplementationType == implementation).First().IsSingleton = isSingleton;
            }
            else
            {
                if (dependency.IsGenericType)
                    dependency = dependency.GetGenericTypeDefinition();
                var node = new DependencyNode(dependency, implementation, isSingleton);
                dependencies[enumVal].Add(node);
            }
        }
    }
}
