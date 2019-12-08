using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XPY.WebTemplate2.Services;

namespace XPY.WebTemplate2.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        [HttpPost]
        public string Post([FromServices] SampleService jwt)
        {
            return jwt.JwtHelper.BuildToken("userId");
        }
    }
}