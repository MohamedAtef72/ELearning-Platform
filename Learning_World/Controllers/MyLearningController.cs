using Learning_World.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Learning_World.Models;
using Learning_World.ViewModels;
using System.Reflection;
using Microsoft.Data.SqlClient.DataClassification;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Learning_World.Repositories;

namespace Learning_World.Controllers
{
	[Authorize]
	public class MyLearningController : Controller
	{
        private readonly MyLearningRepository _myLearningRepository;

		public MyLearningController(MyLearningRepository myLearningRepository)
		{
            _myLearningRepository = myLearningRepository;
        }

		public IActionResult Index()
		{
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

			var courses = _myLearningRepository.FilterCourses(userId);
			return View(_myLearningRepository.GetMyLearningCourses(courses, userId));
		}

		public IActionResult MyCourses()
		{
			return View();
		}

		public IActionResult InProgressCourses()
		{
			int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

			var courses = _myLearningRepository.FilterCourses(userId);
			//	.Where(c => c.Value < 100).Select(c => new
			//{
			//	CourseName = c.Key,  // Assuming c.Key is the course name
			//	CompletionPercentage = c.Value,
			//	IsCompleted = c.Value == 100  // Completed if the value is 100%
			//}).ToList();
			//List<MyLearningCoursesViewModel> myLearningCoursesViewModels = courses.Select(course => new MyLearningCoursesViewModel
			//{
			//	UserId = userId,  // Assign UserId from your controller context
			//	Course = _context.Courses.FirstOrDefault(e => e.CourseId == course.CourseName),
			//	CompletionPercentage = course.CompletionPercentage,
			//	IsCompleted = course.IsCompleted
			//}).ToList();
			var inProgressCourses = _myLearningRepository.GetMyLearningCourses(courses, userId);

			return PartialView("_MyLearningCoursesPartial", inProgressCourses);
		}

		public IActionResult CompletedCourses()
		{
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

			var courses = _myLearningRepository.FilterCourses(userId).Where(c => c.Value == 100);  // Only select completed courses
                                                                                                   //.Select(c => new
                                                                                                   //{
                                                                                                   //	CourseName = c.Key,  // Assuming c.Key is the course name
                                                                                                   //	CompletionPercentage = c.Value,
                                                                                                   //	IsCompleted = c.Value == 100  // Completed if the value is 100%
                                                                                                   //}).ToList();


            //	List<MyLearningCoursesViewModel> myLearningCoursesViewModels = courses.Select(course => new MyLearningCoursesViewModel
            //	{
            //		UserId = userId,  // Assign UserId from your controller context
            //		Course = _context.Courses.FirstOrDefault(e => e.CourseId == course.CourseName),
            //		CompletionDate = (from lc in _context.LessonCompletions
            //						  join module in _context.Modules on lc.ModuleId equals module.ModuleId
            //						  where module.CourseId == course.CourseName && lc.UserId == userId
            //						  orderby lc.CompletionDate descending
            //						  select lc.CompletionDate).FirstOrDefault(), // Get 
            //		CompletionPercentage = course.CompletionPercentage,
            //		IsCompleted = course.IsCompleted
            //	}).ToList();

            var completedCourses = _myLearningRepository.GetMyLearningCourses(courses.ToDictionary(), userId);

            return PartialView("_MyLearningCoursesPartial", completedCourses);
		}

		
	}
}
