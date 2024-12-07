using Learning_World.Data;
using Learning_World.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Learning_World.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

    }
}
