using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XPY.WebTemplate2.Infrastructure.Extensions.NSwag;
using XPY.WebTemplate2.Services;

namespace XPY.WebTemplate2.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        public class TestEnum
        {
            public const string A = "A";
        }
        [HttpPost]
        public string Post([StringEnumDataType(typeof(TestEnum))]string tt, [FromServices] SampleService jwt)
        {
            return jwt.JwtHelper.BuildToken("userId");
        }
    }
}