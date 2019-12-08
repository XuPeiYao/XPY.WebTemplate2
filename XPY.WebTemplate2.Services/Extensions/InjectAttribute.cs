using Microsoft.Extensions.DependencyInjection;
using System;

namespace XPY.WebTemplate2.Services.Extensions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class InjectAttribute : Attribute
    {
        public ServiceLifetime LifeTime { get; private set; }
        public Type ServiceType { get; set; }
        public InjectAttribute(ServiceLifetime lifetime)
        {
            LifeTime = lifetime;
        }
    }
}