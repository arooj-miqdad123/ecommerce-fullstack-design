using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EcommerceApp.Data;

namespace EcommerceApp.Controllers
{
    // Controller to manage user authentication, registration, and session states
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        // Injecting ASP.NET Core Identity managers and AppDbContext via Dependency Injection
        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
        }

        // Action to render the login page, capturing an optional redirect URL destination
        public IActionResult Login(string? returnUrl = null)
        {
            // Populating layout categories for navigation elements
            ViewBag.AllCategories = _db.Categories.ToList();
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // Action to process credentials and authenticate the user
        [HttpPost]
        [ValidateAntiForgeryToken] // Protects against CSRF attacks
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            ViewBag.AllCategories = _db.Categories.ToList();

            // Attempting password-based sign-in with persistent cookie tracking enabled
            var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);

            if (result.Succeeded)
                // Redirecting locally to prevent open-redirect vulnerabilities
                return LocalRedirect(returnUrl ?? "/");

            // Adding validation error to display on UI if the login fails
            ModelState.AddModelError("", "Invalid email or password. Please try again.");
            return View();
        }

        // Action to render the user registration view
        public IActionResult Register()
        {
            ViewBag.AllCategories = _db.Categories.ToList();
            return View();
        }

        // Action to process registration data and provision new identity credentials
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string name, string email, string password)
        {
            ViewBag.AllCategories = _db.Categories.ToList();

            // Mapping incoming parameters to a new IdentityUser model instance
            var user = new IdentityUser { UserName = email, Email = email };

            // Creating the user entry along with secure password hashing under the hood
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // Ensure default 'User' security role node exists in store, if not create it
                if (!await _roleManager.RoleExistsAsync("User"))
                    await _roleManager.CreateAsync(new IdentityRole("User"));

                // Assigning the default security level role to the new profile
                await _userManager.AddToRoleAsync(user, "User");

                // Immediately establish an active authenticated session for the user
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Append creation validation faults to dictionary for front-end parsing
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View();
        }

        // Action to clear user cookie context data and destroy current session
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}