using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using todolist_server.Data;
using todolist_server.Models;
using todolist_server.Services;

namespace todolist_server.Controllers
{
    public class WebController : Controller
    {
        private IRepository _repository { get; set; }
        private readonly SignInManager<IdentityUser> _siginManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Email _email;

        public WebController(IRepository repository, Email email, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _siginManager = signInManager;
            _userManager = userManager;
            _email = email;
        }


        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Action"] = "Login";
            ViewBag.Animation = "Enter";
            return View(new Account());
        }

        [HttpPost]
        public async Task<IActionResult> Login(Account account, string device)
        {
            ViewData["Action"] = "Login";

            ViewBag.Animation = "TryAgain";
            if (account.Email == null || account.Password == null)
            {
                ModelState.AddModelError("Custom_Empty", "Please enter email and password");
                return View("Login", account);
            }

            var user = await _userManager.FindByEmailAsync(account.Email);
            if (user == null)
            {
                ModelState.AddModelError("Custom_NotFound", "User not found");
                return View("Login", account);
            }

            var loginResult = await _siginManager.PasswordSignInAsync(user, account.Password, true, true);
            if (loginResult.Succeeded == false)
            {
                ModelState.AddModelError("Custom_Invalid", "Invalid email or password");
                return View("Login", account);
            }

            if (device == "mobile")
            {
                return RedirectToAction("Task");
            }
            else
            {
                ViewBag.Animation = "Leave";
                return View("Login", account);
            }
            
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _siginManager.SignOutAsync();
            return RedirectToAction("Login");
        }



        [HttpGet]
        public IActionResult RegisterAccount()
        {
            ViewData["Action"] = "RegisterAccount";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccount(Account account)
        {
            ViewData["Action"] = "RegisterAccount";

            if (ModelState.IsValid)
            {
                var user = new IdentityUser() { UserName = account.Email, Email = account.Email };
                var createAccountResult = await _userManager.CreateAsync(user);

                if (createAccountResult == IdentityResult.Success)
                {
                    await _userManager.AddPasswordAsync(user, account.Password);
                    await _email.Welcome(user.UserName, user.Email);
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("Custom_DuplicateEmail", "Email has been used");
            }
            return View();
        }


        [HttpGet]
        public IActionResult ForgetPassword()
        {
            ViewData["Action"] = "ForgetPassword";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            ViewData["Action"] = "ForgetPassword";

            var user = await _userManager.FindByEmailAsync(email);  
            if (user == null)
            {
                ModelState.AddModelError("Custom_InvalidEmail", "Email not found");
                return View();
            }

            var lockedOut = await _userManager.IsLockedOutAsync(user);
            if (lockedOut == true)
            {
                ModelState.AddModelError("Custom_TooManyAttempt", "Too many invalid attempt, please try again later");
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(user); //generate a unique token on every reqeust
            var passcode = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPassword");
            await _email.PasswordReset(user.UserName!, user.Email!, passcode);
            return RedirectToAction("VerifyPasscode", new { email = user.Email });
        }

        [HttpGet]
        public IActionResult VerifyPasscode(string email)
        {
            ViewData["Action"] = "VerifyPasscode";
            return View("VerifyPasscode", email);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyPasscode(string email, string passcode)
        {
            ViewData["Action"] = "VerifyPasscode";

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("Custom_InvalidEmail", "User not found");
                return View("VerifyPasscode", email);
            }

            var lockedOut = await _userManager.IsLockedOutAsync(user);
            if (lockedOut == true)
            {
                ModelState.AddModelError("Custom_TooManyAttempt", "Too many invalid attempt, please try again later");
                return View("VerifyPasscode", email);
            }

            var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPassword", passcode);
            if (result == false)
            {
                await _userManager.AccessFailedAsync(user);
                ModelState.AddModelError("Custom_InvalidAttempt", "Invalid passcode");
                return View("VerifyPasscode", email);
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            return RedirectToAction("ResetPassword", new { email = email, resetToken = resetToken });
        }



        [HttpGet]
        public IActionResult ResetPassword(string email, string resetToken)
        {
            ViewData["Action"] = "ResetPassword";

            if (email == null || resetToken == null)
            {
                return RedirectToAction("Task");
            }

            var model = new ResetPassword { Email = email, ResetToken = resetToken };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            ViewData["Action"] = "ResetPassword";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Custom_InvalidEmail", "User not found");
                return View(model);
            }

            var lockedOut = await _userManager.IsLockedOutAsync(user);
            if (lockedOut == true)
            {
                ModelState.AddModelError("Custom_TooManyAttempt", "Too many invalid attempt, please try again later");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password);
            if (result != IdentityResult.Success)
            {
                await _userManager.AccessFailedAsync(user);
                ModelState.AddModelError("Custom_InvalidAttempt", "Invalid token");
                return View(model);
            }
            
            return RedirectToAction("Task");
        }



        [HttpGet]
        [Authorize(AuthenticationSchemes = "Identity.Application")]
        public async Task<IActionResult> Task(int? month, int? year, string? keyword)
        {
            ViewData["Action"] = "Task";

            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            ViewData["year"] = year ?? DateTime.Now.Year;
            ViewData["month"] = (month ?? DateTime.Now.Month).ToString("D2");

            List<TaskInfo> tasks;
            if (keyword == null)
            {
                tasks = await _repository.ReadTasks(userId, year ?? DateTime.Now.Year, month ?? DateTime.Now.Month);
            }
            else
            {
                tasks = await _repository.ReadTasksKeyword(userId, keyword);
            }

            return View(tasks);
        }



        [HttpGet]
        [Authorize(AuthenticationSchemes = "Identity.Application")]
        public async Task<IActionResult> CounterCheckTask(int month, int year)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return BadRequest();
            }

            var tasks = await _repository.ReadTasks(userId, year, month);
            return Ok(tasks);
        }
    }
}
