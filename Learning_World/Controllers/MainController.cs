using System.Diagnostics;
using Learning_World.Data;
using Learning_World.Repositories;
using Learning_World.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Learning_World.Controllers
{
    public class MainController : Controller
    {
        private readonly CoursesRepository _coursesRepository;

        public MainController(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }
        public IActionResult Index()
        {
            var topCourses = _coursesRepository.GetMostPopularCourses(3);
            return View(topCourses);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [Route("Home/Error")]
        public IActionResult Error(int? statusCode)
        {
            var errorModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorCode = statusCode
            };
            switch (statusCode)
            {
                case 404:
                    errorModel.ErrorMessage = "Page Not Found";
                    return View(errorModel);
                case 500:
                    errorModel.ErrorMessage = "Internal Server Error";
                    return View(errorModel);
                case 403:
                    errorModel.ErrorMessage = "Access Denied";
                    return View(errorModel);
                default:
                    errorModel.ErrorMessage = "An unexpected error occurred";
                    return View(errorModel);
            }
        }

    }
}
