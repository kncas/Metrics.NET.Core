
namespace Owin.Sample.Controllers
{
    using System.Web.Http;

    [RoutePrefix("sampleignore")]
    public class SampleIgnoreController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        [Route("")]
        public string Get()
        {
            return "get";
        }
    }
}
