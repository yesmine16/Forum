using Forum.Models;
using Forum.Repositorys;
using Forum.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Forum.Controllers
{
    public class SecurityController : Controller
    {
        private readonly UserRepository _context;
        private readonly PasswordTokenRepository _passwordTokenRepository;
        private readonly IMailService _mailService;
        private readonly IJwtService _jwtService;


        public SecurityController(UserRepository userRepository, IJwtService jwtService, PasswordTokenRepository passwordTokenRepository, IMailService mailService)
        {
            _context = userRepository;
            _jwtService = jwtService;
            _passwordTokenRepository = passwordTokenRepository;
            _mailService = mailService;
        }

        // GET: SecurityController
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                return RedirectToAction("Index");
            }
            var loggedInUser = await _context.ValidUser(model);

            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, loggedInUser.Pseudonyme),
                    new(ClaimTypes.Email, loggedInUser.Email),
                    new(ClaimTypes.Sid, loggedInUser.Id.ToString())
                };

            claims.Add(loggedInUser.Admin ? new Claim(ClaimTypes.Role, "Admin") : new Claim(ClaimTypes.Role, "User"));

            var tokenString = _jwtService.GenerateToken(claims);
            HttpContext.Session.SetString("Token", tokenString);
            return Redirect("http://localhost:5109/posts/posts");
        }

        public ActionResult Logout()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            {
                return NoContent();
            }
            HttpContext.Session.Remove("Token");
            return RedirectToAction("Index");

        }

        public ActionResult ResetPassWord()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassWord([FromForm] PasswordToken tokenTogenerate)
        {
            if (!await _context.IsValidEmail(tokenTogenerate.Email))
            {
                return Unauthorized();
            }
            var token = GenerateToken();
            tokenTogenerate.Token = token;
            tokenTogenerate.ValidationInterval = DateTime.Now;
            await _passwordTokenRepository.SaveTokenAsync(tokenTogenerate);
            var link = GenerateResetPasswordLink(token, tokenTogenerate.Email);
            try
            {
                await _mailService.SendEmail(tokenTogenerate.Email
                    , GetResetPasswordEmailBody(link)
                    , "Reset your password"
                    , true);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return Problem($"internal server error cause {ex.Message} exception was thrown");
            }

        }

        private string GetResetPasswordEmailBody(string link)
        {
            return $@"
                <!DOCTYPE html>
                <html lang=""en"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Password Reset</title>
                </head>
                <body>
                    <p>Hello,</p>
                    <p>We received a request to reset the password associated with this email address. If you made this request, click the link below. If you did not request a password reset, you can safely ignore this email.</p>
                    
                    <p>Reset Password Link:</p>
                    <p><a href=""{link}"">Reset Password</a></p>

                    <p>This link will expire in 20 mins for security reasons.</p>

                    <p>Thank you,</p>
                    <p>Your Application Team</p>
                </body>
                </html>";
        }

        private string GenerateResetPasswordLink(Guid token, string email)
        {
            return $"http://localhost:5109/Security/ChangePassWord/?email={email}&token={token}";
        }

        private Guid GenerateToken()
        {
            return Guid.NewGuid();
        }


        [AllowAnonymous]
        public async Task<ActionResult> ChangePassWord(string email, Guid token)
        {
            if (await _passwordTokenRepository.GetLastValidToken(email) != token)
            {
                return Unauthorized();
            }

            await _passwordTokenRepository.DeleteConsumedToken(token);

            var passwordModification = new ResetPasswordModel
            {
                Email = email
            };
            return View(passwordModification);
        }

        [Authorize]
        public async Task<ActionResult> ChangePassWordLogedIn(Guid id)
        {
            var user = await _context.GetById(id);

            var passwordModification = new ResetPasswordModel
            {
                Email = user.Email
            };
            return View(passwordModification);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePassWord([FromForm] ResetPasswordModel passwordParam)
        {
            if (passwordParam.NewPassword != passwordParam.ConfirmNewPassword)
            {
                return BadRequest();
            }
            var user = await _context.GetByEmail(passwordParam.Email);

            user.MotDePasse = passwordParam.NewPassword;
            await _context.UpdatePasswordAsync(user);
            if(CurrentUser() != Guid.Empty)
            {
                return RedirectToAction("Logout");
            }
            return RedirectToAction("Index");
        }

        public Guid CurrentUser()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.Sid);
            if (userIdClaim == null)
            {
                return Guid.Empty;
            }
            return Guid.Parse(userIdClaim?.Value);
        }
    }
}
