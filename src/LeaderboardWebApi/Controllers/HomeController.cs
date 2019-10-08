using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace LeaderboardWebApi.Controllers
{
    [OpenApiIgnore]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/openapi");
        }
    }
}
