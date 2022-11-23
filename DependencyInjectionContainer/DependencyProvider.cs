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

            foreach(var enumDependencies in config.dependencies.Values.Append(config.defaultDependencies).ToList())
            {
                foreach (var dependency in enumDependencies)
                {
                    if (
                        dependency.ImplementationType.IsClass &&
                        !dependency.ImplementationType.IsAbstract &&
                        (
                        dependency.ImplementationType == dependency.DependencyType ||
                        dependency.ImplementationType.GetInterfaces().Contains(dependency.DependencyType) ||
                        dependency.ImplementationType.IsSubclassOf(dependency.DependencyType) ||
                        (dependency.DependencyType.IsGenericType && dependency.ImplementationType.GetInterfaces()
                            .Any(i => i.GetGenericTypeDefinition().Equals(dependency.DependencyType.GetGenericTypeDefinition()))
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
        }
        public T Resolve<T>(object enumVal = default)
        {
            Type type = typeof(T);

            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                var temp = ResolveMany(type.GetGenericArguments().FirstOrDefault(), enumVal).Where(r => r != null);

                return (T)typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[] { temp });
            }
            else
                return (T)(ResolveMany(type, enumVal).FirstOrDefault(r => r != null) ?? GenerateObject(type));
        }
        private object Resolve(Type type, object enumVal = default)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
            {
                var temp = ResolveMany(type.GetGenericArguments().FirstOrDefault(), enumVal).Where(r => r != null);

                return typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(type.GetGenericArguments()).Invoke(null, new object[] { temp });
            }
            else
                return (ResolveMany(type, enumVal).FirstOrDefault(r => r != null) ?? GenerateObject(type));
        }
        private IEnumerable<object> ResolveMany(Type type, object enumVal)
        {
            List<DependencyNode> dependencies;
            if (enumVal == null)
                dependencies = config.defaultDependencies;
            else
            {
                if (!config.dependencies.ContainsKey(enumVal))
                    return null;
                else
                    dependencies = config.dependencies[enumVal];
            }
            
            return dependencies.Select(c =>
           {
               if(c.DependencyType == type)
               {
                   if (c.IsSingleton && c.SingletonImplementation != null)
                   {
                       return c.SingletonImplementation;
                   }
                   else
                   {
                       return c.SingletonImplementation = GenerateObject(c.ImplementationType);
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

            var genParams = cParams.Select(p =>
            {
                if(p.GetCustomAttributes(false).Any( a => a is DependencyKey))
                {
                    var enumVal = (p.GetCustomAttributes(false).FirstOrDefault(a => a is DependencyKey) as DependencyKey).enumVal;
                    return Resolve(p.ParameterType, enumVal);
                }
                return Resolve(p.ParameterType);
            });

            return constructor.Invoke(genParams.ToArray());
        }
    }
}
