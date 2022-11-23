using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKey : Attribute
    {
        public object enumVal { get; }
        public DependencyKey(object enumVal)
        {
            this.enumVal = enumVal;
        }
    }
}
