using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WebApplication3.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Added for logging

namespace WebApplication3.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AccountController> _logger; // Added for logging

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AccountController> logger) // Injected ILogger
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger; // Assigned logger
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {model.Email} logged in successfully.");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        _logger.LogWarning($"Failed login attempt for user {model.Email}.");
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"Error during login for user {model.Email}.");
                    ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address
                };

                try
                {
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation($"User {model.Email} registered and logged in successfully.");
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogWarning($"Error registering user {model.Email}: {error.Description}");
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"Error during registration for user {model.Email}.");
                    ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogWarning("Attempted to access profile without a logged-in user.");
                    return RedirectToAction("Login");
                }

                var model = new ProfileViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address
                };

                return View(model);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile.");
                TempData["ErrorMessage"] = "An error occurred while loading your profile. Please try again.";
                return RedirectToAction("Index", "Home"); // Or another appropriate error page
            }
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        _logger.LogWarning("Attempted to update profile without a logged-in user.");
                        return RedirectToAction("Login");
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Address = model.Address;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"User {user.Email} profile updated successfully.");
                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        _logger.LogWarning($"Error updating profile for user {user.Email}: {error.Description}");
                    }
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, $"Error during profile update for user with ID {model.Id}.");
                    ModelState.AddModelError(string.Empty, "An error occurred while updating your profile. Please try again.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out successfully.");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during user logout.");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning("Access denied to a restricted resource.");
            return View();
        }

        // This method should be called when the application starts
        public async Task CreateAdminUser()
        {
            try
            {
                // Create Admin role if it doesn't exist
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    _logger.LogInformation("Admin role created successfully.");
                }

                // Create Admin user if it doesn't exist
                var adminEmail = "admin@example.com";
                var adminUser = await _userManager.FindByEmailAsync(adminEmail);
                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        FirstName = "Admin",
                        LastName = "User"
                    };

                    var result = await _userManager.CreateAsync(adminUser, "Admin123!"); // Consider using a more secure way to set initial admin password
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(adminUser, "Admin");
                        _logger.LogInformation($"Admin user {adminEmail} created and assigned to Admin role.");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            _logger.LogError($"Error creating admin user: {error.Description}");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error during admin user/role creation.");
            }
        }
    }
}