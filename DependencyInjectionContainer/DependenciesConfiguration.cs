using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        public List<DependencyNode> defaultDependencies = new List<DependencyNode>();
        public Dictionary<object, List<DependencyNode>> dependencies = new Dictionary<object, List<DependencyNode>>();

        public void Register<T1, T2>(object enumVal = default, bool isSingleton = false)
        {
            ConfRegister(typeof(T1), typeof(T2), enumVal, isSingleton);
        }

        public void Register(Type dependency, Type implementation, object enumVal = default, bool isSingleton = false)
        {
            ConfRegister(dependency, implementation, enumVal, isSingleton);
        }

        private void ConfRegister(Type dependency, Type implementation, object enumVal, bool isSingleton = false)
        {
            List<DependencyNode> dependencies;
            if (enumVal == null)
                dependencies = defaultDependencies;
            else
            {
                if (!this.dependencies.ContainsKey(enumVal))
                    this.dependencies.Add(enumVal, new List<DependencyNode>());
                dependencies = this.dependencies[enumVal];
            }
            
            if(dependencies.Any( d => d.DependencyType == dependency && d.ImplementationType == implementation))
            {
                dependencies.Where(d => d.DependencyType == dependency && d.ImplementationType == implementation).First().IsSingleton = isSingleton;
            }
            else
            {
                if (dependency.IsGenericType)
                    dependency = dependency.GetGenericTypeDefinition();
                var node = new DependencyNode(dependency, implementation, isSingleton);
                dependencies.Add(node);
            }
        }
    }
}
