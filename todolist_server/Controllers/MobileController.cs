using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using todolist_server.Data;
using todolist_server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using todolist_server.Models;

namespace todolist_server.Controllers
{
    [ApiController]
    [Route("[controller]/[Action]")]
    public class MobileController : ControllerBase
    {
        private IRepository _repository { get; set; }
        private readonly SignInManager<IdentityUser> _siginManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Email _email;
        private readonly JwtToken _jwtToken;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public MobileController(IRepository repository, Email email, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager,
            JwtToken jwtToken, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _siginManager = signInManager;
            _userManager = userManager;
            _email = email;
            _jwtToken = jwtToken;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                if (email == null || password == null)
                {
                    return BadRequest("Please enter email and password");
                }

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var loginResult = await _siginManager.PasswordSignInAsync(user, password, true, true);
                if (loginResult.Succeeded == true)
                {
                    string appToken = _jwtToken.CreateToken(user.Id, user.UserName, user.Email);
                    return Ok(appToken);
                }
                return BadRequest("Invalid email or password");
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> RegisterAccount(Account account)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = new IdentityUser() { UserName = account.Email, Email = account.Email };
                    var createAccountResult = await _userManager.CreateAsync(user);

                    if (createAccountResult == IdentityResult.Success)
                    {
                        await _userManager.AddPasswordAsync(user, account.Password);
                        await _email.Welcome(user.UserName, user.Email);

                        string appToken = _jwtToken.CreateToken(user.Id, user.UserName, user.Email);
                        return Ok(appToken);
                    }
                    return BadRequest("Email has been used");
                }
                return BadRequest();
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest("Email not found");
                }

                var lockedOut = await _userManager.IsLockedOutAsync(user);
                if (lockedOut == true)
                {
                    return BadRequest("Too many invalid attempt, please try again later");
                }

                await _userManager.UpdateSecurityStampAsync(user);
                var passcode = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPassword");
                await _email.PasswordReset(user.UserName!, user.Email!, passcode);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> ForgetPasswordWithJwtToken()
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var lockedOut = await _userManager.IsLockedOutAsync(user);
                if (lockedOut == true)
                {
                    return BadRequest("Too many invalid attempt, please try again later");
                }

                await _userManager.UpdateSecurityStampAsync(user);
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                return Ok(new { resetToken = resetToken, email = user.Email });
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> VerifyPasscode(string email, string passcode)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var lockedOut = await _userManager.IsLockedOutAsync(user);
                if (lockedOut == true)
                {
                    return BadRequest("Too many invalid attempt, please try again later");
                }

                var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultPhoneProvider, "ResetPassword", passcode);
                if (result == false)
                {
                    await _userManager.AccessFailedAsync(user);
                    return BadRequest("Invalid passcode");
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                return Ok(resetToken);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var lockedOut = await _userManager.IsLockedOutAsync(user);
                if (lockedOut == true)
                {
                    return BadRequest("Too many invalid attempt, please try again later");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.ResetToken, model.Password);
                if (result != IdentityResult.Success)
                {
                    await _userManager.AccessFailedAsync(user);
                    return BadRequest("Invalid token");
                }

                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme) ]
        [HttpGet]
        public async Task<IActionResult> ReadTaskFromTime(int month, int year)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest();
                }

                var tasks = await _repository.ReadTasks(userId, year, month);
                return Ok(tasks);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> ReadTaskFromKeyword(string keyword)
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest();
                }

                var tasks = await _repository.ReadTasksKeyword(userId, keyword);
                return Ok(tasks);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [HttpGet]
        public IActionResult ReadPrivacyPolicy()
        {
            try
            {
                var privacyPolicyPath = Path.Combine(_webHostEnvironment.WebRootPath, "policy/PrivacyPolicy.txt");
                var fileContent = System.IO.File.ReadAllText(privacyPolicyPath);

                return Ok(fileContent);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [HttpGet]
        public IActionResult ReadTermsAndConditions()
        {
            try
            {
                var termsAndConditionsPath = Path.Combine(_webHostEnvironment.WebRootPath, "policy/TermsAndConditions.txt");
                var fileContent = System.IO.File.ReadAllText(termsAndConditionsPath);

                return Ok(fileContent);
            }
            catch
            {
                return StatusCode(500);
            }
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return BadRequest();
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return BadRequest("User not found");
                }

                await _repository.DeleteAllTask(user.Id);
                await _userManager.DeleteAsync(user);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
