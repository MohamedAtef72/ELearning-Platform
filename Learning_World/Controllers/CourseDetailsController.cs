using System.Security.Claims;
using Learning_World.Data;
using Learning_World.Models;
using Learning_World.Repositories;
using Learning_World.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Learning_World.Controllers
{
	public class CourseDetailsController : Controller
	{
        private readonly EnrollmentRepository _enrollmentRepository;
        private readonly CoursesRepository _coursesRepository;

        public CourseDetailsController(EnrollmentRepository enrollmentRepository, CoursesRepository coursesRepository)
		{
            _enrollmentRepository = enrollmentRepository;
            _coursesRepository = coursesRepository;
        }

		[Route("Courses/view/{courseId}")]

		public IActionResult ShowCourse(int courseId = 1)
		{
			Course course = _coursesRepository.GetById(courseId);
			if (course == null)
			{
				return View("NotFound404");
			}
			ViewBag.modules = _coursesRepository.GetCourseModules(courseId);
			ViewBag.parts = _coursesRepository.GetCourseParts();
			ViewBag.lessons = _coursesRepository.GetCourseLessons();
			ViewBag.IsEnrolled = false;
			if (User.Identity.IsAuthenticated)
			{
			int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);
			ViewBag.IsEnrolled = _enrollmentRepository.IsEnrolled(courseId, userId);
			}
			return View(course);
		}

		[Authorize]
		[HttpGet]
		public IActionResult Enroll(int courseId)
		{
			var course = _coursesRepository.GetById(courseId);
			if (course == null)
				return View("NotFound404");

			var userRegistration = new UserRegistrationViewModel();
			userRegistration.CourseId = course.CourseId;
			userRegistration.CourseName = course.Title;
			userRegistration.CoursePrice = course.Price;

			return View(userRegistration);
		}
		[Authorize]
		[HttpPost]
		public IActionResult Enroll(UserRegistrationViewModel model)
		{
			if (ModelState.IsValid)
			{
				// Save user information to the database
				//if (model.PaymentMethod == "CreditCard")
				//{
				int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value);

				var CreditPayment = new Payment
				{
					CardName = model.CardName,
					CardNumber = model.CardNumber,
					ExpiryDate = model.ExpiryDate,
					CVC = model.CVC,
					Country = model.Country
				};

				var enrollment = new Enrollment
				{
					CourseId = model.CourseId,
					UserId = userId,
					EnrollmentDate = DateTime.UtcNow,
					Payment = CreditPayment
				};

				_enrollmentRepository.AddEnroll(enrollment, CreditPayment);
				_enrollmentRepository.Save();

				return RedirectToAction("Index", "MyLearning");
				// return RedirectToAction("CourseView", new {courseId = model.Course.CourseId, userId = model.UserId});
			}

			return View("Enroll", model);
		}
	}
}
