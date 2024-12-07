using Learning_World.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace Learning_World.Filters
{
    // The outer class should derive from TypeFilterAttribute
    public class LessonsAuthorizationFilter : TypeFilterAttribute
    {
        public LessonsAuthorizationFilter() : base(typeof(LessonsAuthorizationImplementation))
        {
        }

        // Inner class for the actual implementation of IAuthorizationFilter
        private class LessonsAuthorizationImplementation : IAuthorizationFilter
        {
            private readonly ElearningPlatformContext _db;

            public LessonsAuthorizationImplementation(ElearningPlatformContext db)
            {
                _db = db;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                // Ensure the user is authenticated
                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                    return;
                }

                // Get the user ID from the ClaimsPrincipal
                var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // Extract moduleId from the route data
                if (context.RouteData.Values.TryGetValue("moduleId", out var moduleIdValue) &&
                    int.TryParse(moduleIdValue.ToString(), out var moduleId))
                {
                    // Fetch courseId from the module
                    var courseId = GetCourseIdByModuleId(moduleId);
                    if (courseId == null || !IsUserEnrolledInCourse(userId, courseId.Value))
                    {
                        // If the user is not enrolled in the course, redirect to an overview page
                        context.Result = new RedirectToActionResult("CoursesOverView", "Courses", null);
                    }
                }
                else
                {
                    // If moduleId is missing or invalid, return a bad request
                    context.Result = new BadRequestResult();
                }
            }

            // Helper method to get courseId by moduleId
            private int? GetCourseIdByModuleId(int moduleId)
            {
                // Use FirstOrDefault to safely query the module, return null if not found
                var module = _db.Modules.FirstOrDefault(m => m.ModuleId == moduleId);
                return module?.CourseId;
            }

            // Helper method to check if the user is enrolled in the course
            private bool IsUserEnrolledInCourse(int userId, int courseId)
            {
                return _db.Enrollments.Any(e => e.UserId == userId && e.CourseId == courseId);
            }
        }
    }
}
