using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Owin.Sample.Controllers
{
    [Route("sample")]
    public class SampleController : ControllerBase
    {
        [Route("")]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        [Route("withparams/{x}/{y}")]
        public IEnumerable<string> Get(int x, string y)
        {
            return new[] { "value1", "value2" };
        }
    }
}
