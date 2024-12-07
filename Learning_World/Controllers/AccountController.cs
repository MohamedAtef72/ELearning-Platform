using Learning_World.Models;
using Learning_World.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Learning_World.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _UserManager;
        private readonly SignInManager<User> _SignInManager;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _UserManager = userManager;
            _SignInManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel user)
        {
            if (ModelState.IsValid)
            {
                // UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(); 
                // mapping
                User applicationUser = new User()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Image = "default.jpg"
                };
                // add userpassword for hashing
                IdentityResult result = await _UserManager.CreateAsync(applicationUser, user.Password);
                if (result.Succeeded)
                {
                    // add role user by default id need admin call the office
                    //  await _UserManager.AddToRoleAsync(applicationUser, "user");
                    // cookie  
                    await _UserManager.AddToRoleAsync(applicationUser, "student");//to make any one student
                    await _SignInManager.SignInWithClaimsAsync(applicationUser, false, new List<Claim>() {
                            new("Image",applicationUser.Image)
                        });
                    return RedirectToAction("Index", "Main");
                }
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            return View("Register", user);
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Main");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel user)
        {
            if (ModelState.IsValid)
            {
                // check if found 
                var AppUser = await _UserManager.FindByNameAsync(user.Name);
                if (AppUser != null)
                {
                    bool exsist = await _UserManager.CheckPasswordAsync(AppUser, user.Password);
                    if (exsist)
                    {
                        await _SignInManager.SignInWithClaimsAsync(AppUser, user.Remember, new List<Claim>() {
                            new("Image",AppUser.Image)
                        });
                      
                        return RedirectToAction("Index", "Main");
                    }
                    ModelState.AddModelError("Password", "password is incorrect");
                }
                else
                {
                    ModelState.AddModelError("", "UserName or Password is Incorrect");
                }
            }
            return View("Login", user);
        }
        public async Task<IActionResult> Logout()
        {
            await _SignInManager.SignOutAsync();
            Response.Cookies.Delete("UserId");
            return RedirectToAction("Login");
        }
    }
}
