using Azure.Core;
using Learning_World.Data;
using Learning_World.Extentions;
using Learning_World.Models;
using Learning_World.Repositories;
using Learning_World.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class CoursesController : Controller
{
    private readonly CoursesRepository _coursesRepository;

    public CoursesController(CoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> CoursesOverView(List<string> Levels, List<string> Prices, List<string> Rates, string sortOrder = "MostPopular", string search = "", int pageNo = 1)
    {
        var courseVM = await Courses_VM_Mapper(_coursesRepository.GetAllCourses(), Levels, Prices, Rates, sortOrder, search, pageNo);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_CoursesOverviewPartial", courseVM);
        }

        return View(courseVM);
    }

    [NonAction]
    public IQueryable<Course> SortCourses(IQueryable<Course> courses, string sortBy)
    {
        return sortBy switch
        {
            "MostPopular" => courses.OrderByDescending(c => c.MaxEnrollment),
            "HighestRated" => courses.OrderByDescending(c => c.AverageRating),
            "Newest" => courses.OrderByDescending(c => c.CreationDate),
            _ => courses
        };
    }

    [NonAction]
    public IQueryable<Course> SearchCourses(IQueryable<Course> courses, string search)
    {
        if (!string.IsNullOrEmpty(search))
        {
            courses = courses.Where(c => c.Title.Contains(search));
        }
        return courses;
    }

    [NonAction]
    public async Task<CoursesOverviewViewModel> Courses_VM_Mapper(IQueryable<Course> courses, List<string> Levels, List<string> Prices, List<string> Rates, string sortBy, string search, int pageNumber)
    {
        courses = SortCourses(courses, sortBy);
        courses = SearchCourses(courses, search);
        courses = FilterCourses(courses, Levels, Prices, Rates);

        var paginatedCourses = await PaginatedList<Course>.Create(courses, pageNumber, 6);

        return new CoursesOverviewViewModel
        {
            Search = search,
            Sort = sortBy,
            Levels = Levels,
            Prices = Prices,
            Rates = Rates,
            Page = paginatedCourses
        };
    }

    [NonAction]
    public IQueryable<Course> FilterCourses(IQueryable<Course> courses, List<string> Levels, List<string> Prices, List<string> Rates)
    {
        if (Levels != null && Levels.Any())
        {
            courses = courses.Where(c => Levels.Contains(c.DifficultyLevel));
        }

        if (Prices != null && Prices.Any())
        {
            courses = courses.Where(c => Prices.Contains(c.Price > 0 ? "Paid" : "Free"));
        }

        if (Rates != null && Rates.Any())
        {
            var selectedRates = Rates.Select(decimal.Parse).ToList();
            courses = courses.Where(c => selectedRates.Any(rate => c.AverageRating >= rate));
        }

        return courses;
    }
}
