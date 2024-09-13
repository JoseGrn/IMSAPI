using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace IMSAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "hola mundo";
        }
    }
}
