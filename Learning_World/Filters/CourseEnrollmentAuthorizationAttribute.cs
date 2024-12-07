using Learning_World.Data;
using Learning_World.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learning_World.Filters
{


    public class CourseEnrollmentAuthorizationAttribute : TypeFilterAttribute
    {
        public CourseEnrollmentAuthorizationAttribute() : base(typeof(CourseEnrollmentAuthorizationFilter))
        {
        }

        private class CourseEnrollmentAuthorizationFilter : IAuthorizationFilter
        {
            private readonly ElearningPlatformContext _db;

            public CourseEnrollmentAuthorizationFilter(ElearningPlatformContext db)
            {
                _db = db;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var userId = int.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var courseId = GetCourseIdFromRouteData(context);


                if (courseId == null || !IsUserEnrolledInCourse(userId, courseId.Value))
                {
                    context.Result = new RedirectToActionResult("CoursesOverView", "Courses", null);
                }
                
            }

            private int? GetCourseIdFromRouteData(AuthorizationFilterContext context)
            {
                if (context.RouteData.Values.TryGetValue("id", out var idValue) && int.TryParse(idValue.ToString(), out int id))
                {
                    return id;
                }
                return null;
            }

            private bool IsUserEnrolledInCourse(int userId, int courseId)
            {
                return _db.Enrollments.Any(e => e.UserId == userId && e.CourseId == courseId);
            }
        }
    }
}


