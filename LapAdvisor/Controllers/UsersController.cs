using LapAdvisor.Model;
using LapAdvisor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace LapAdvisor.Controllers
{
    public class UsersController : Controller
    {
        // Dependency injection
        UserManager<ApplicationUser> _userManager;
        SignInManager<ApplicationUser> _signInManager;
        public UsersController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Login(string returnUrl)
        {
            UserModel model = new UserModel()
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        public async Task<IActionResult> LoginOut()
        {
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }

        public IActionResult Register()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserModel model)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            ApplicationUser user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email
            };

            try
            {
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // إضافة الدور
                    await _userManager.AddToRoleAsync(user, "Customer");

                    // ✔ يرجع مباشرة لصفحة تسجيل الدخول
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    // إرجاع الأخطاء في حالة الفشل
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the account.");
            }

            return View("Register", model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserModel model)
        {
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.Email
            };
            try
            {
                var loginResult = await _signInManager.PasswordSignInAsync(user.Email, model.Password, true, true);
                if (loginResult.Succeeded)
                {
                    if (string.IsNullOrEmpty(model.ReturnUrl))
                        return Redirect("~/");
                    else
                        return Redirect(model.ReturnUrl);
                }
            }
            catch (Exception ex)
            {

            }
            return View(new UserModel());
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
