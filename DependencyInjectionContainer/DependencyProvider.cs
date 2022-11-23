using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyProvider
    {
        private DependenciesConfiguration config;
        public DependencyProvider(DependenciesConfiguration configuration)
        {
            config = configuration;

            foreach(var dependency in config.dependencies)
            {
                if(
                    dependency.ImplementationType.IsClass &&
                    !dependency.ImplementationType.IsAbstract &&
                    (
                    dependency.ImplementationType == dependency.DependencyType ||
                    dependency.ImplementationType.GetInterfaces().Contains(dependency.DependencyType) ||
                    dependency.ImplementationType.IsSubclassOf(dependency.DependencyType) ||
                    (dependency.DependencyType.IsGenericType && dependency.ImplementationType.GetInterfaces()
                        .Any( i => i.GetGenericTypeDefinition().Equals(dependency.DependencyType.GetGenericTypeDefinition()))
                        && !dependency.IsSingleton)
                    )
                    )
                {
                    if (dependency.IsSingleton)
                        dependency.SingletonImplementation = Resolve(dependency.ImplementationType);
                }
                else
                {
                    throw new TypeLoadException("Uncorrect dependencies in configuration");
                }
            }
        }
        public T Resolve<T>()
        {
            Type type = typeof(T);

            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                var temp = ResolveMany(type.GetGenericArguments().FirstOrDefault()).Where(r => r != null);

                return (T)typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[] { temp });
            }
            else
                return (T)(ResolveMany(type).FirstOrDefault(r => r != null) ?? GenerateObject(type));
        }
        private object Resolve(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                var temp = ResolveMany(type.GetGenericArguments().FirstOrDefault()).Where(r => r != null);

                return typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[] { temp });
            }
            else
                return (ResolveMany(type).FirstOrDefault(r => r != null) ?? GenerateObject(type));
        }
        private IEnumerable<object> ResolveMany(Type type)
        {
            return config.dependencies.Select(c =>
           {
               if(c.DependencyType == type)
               {
                   var node = config.dependencies.Where(d => d.DependencyType == type).First();
                   if (node.IsSingleton && node.SingletonImplementation != null)
                   {
                       return node.SingletonImplementation;
                   }
                   else
                   {
                       return node.SingletonImplementation = GenerateObject(node.ImplementationType);
                   }
               }
               if (c.DependencyType.IsGenericTypeDefinition && type.IsGenericType && type.GetGenericTypeDefinition() == c.DependencyType)
               {
                   if (c.IsSingleton && c.SingletonImplementation != null)
                   {
                       return c.SingletonImplementation;
                   }
                   return GenerateObject(c.ImplementationType.MakeGenericType(type.GetGenericArguments()));
               }

               return null;
           });
        }

        private object GenerateObject(Type type)
        {
            var constructor = type.GetConstructors().Single();
            var cParams = constructor.GetParameters();

            return constructor.Invoke(cParams.Select( p => Resolve(p.ParameterType)).ToArray());
        }
    }
}
