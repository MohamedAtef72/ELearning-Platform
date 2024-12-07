using System.Security.Claims;
using Learning_World.Models;
using Learning_World.ViewModels;
using Learning_World.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Learning_World.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserProfileRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public ProfileController(UserProfileRepository userRepository, UserManager<User> userManager, SignInManager<User> signInManager, IWebHostEnvironment hostingEnvironment)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostingEnvironment = hostingEnvironment;
        }

        [Route("Profile/Show")]
        public async Task<IActionResult> Show()
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var viewModel = await _userRepository.GetUserProfile(userId);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Edit()
        {
            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var viewModel = await _userRepository.GetUserProfile(userId);

            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveEdit(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            int userId = int.Parse(User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetUserById(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.Name;
            user.Email = model.Email;

            bool updateResult = await _userRepository.UpdateUser(user, model.ImageFile, _hostingEnvironment.WebRootPath);

            if (!updateResult)
            {
                return NotFound();
            }

            await UpdateUserClaims(user, false);

            return RedirectToAction("Login", "Account");
        }

        private async Task UpdateUserClaims(User user, bool remember)
        {
            await _signInManager.SignOutAsync();

            var claims = new List<Claim>
            {
                new Claim("Image", user.Image)
            };

            await _signInManager.SignInWithClaimsAsync(user, isPersistent: remember, claims);
        }
    }
}