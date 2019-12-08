using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XPY.WebTemplate2.Infrastructure.Extensions;
using XPY.WebTemplate2.Services.Extensions;

namespace XPY.WebTemplate2.Services
{
    [Inject(ServiceLifetime.Scoped)]
    public class SampleService
    {
        public JwtHelper JwtHelper { get; private set; }
        public SampleService(JwtHelper jwt)
        {
            JwtHelper = jwt;
        }
    }
}
