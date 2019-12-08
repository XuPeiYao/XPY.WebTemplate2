using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace XPY.WebTemplate2.Services.Extensions
{
    public static class ServicesExtension
    {
        public static void AddServices(this IServiceCollection services)
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in allTypes)
            {
                var attr = type.GetCustomAttribute<InjectAttribute>();
                if (attr == null) continue;

                services.Add(new ServiceDescriptor(attr.ServiceType ?? type, type, attr.LifeTime));
            }
        }
    }
}
