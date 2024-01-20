using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Forum.Models;
using Forum.Repositorys;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Forum.Tools;

namespace Forum.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserRepository _context;
        private readonly IPasswordSecurity _PassWordSecurityContext;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public UserController(UserRepository context, IPasswordSecurity passwordSecurityContext, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _PassWordSecurityContext = passwordSecurityContext;
            _hostingEnvironment = hostingEnvironment;

        }

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _context.GetAll();
            return View(users);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // ReSharper disable once Mvc.ViewNotResolved
            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _PassWordSecurityContext.hashPasswordWithSalt(ref user);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }


        // GET: User/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }


            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(User user, IFormFile avatarFile)
        {
            try
            {
                if (avatarFile != null && avatarFile.Length > 0)
                {

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;

                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(fileStream);
                    }

                    user.CheminAvatar = "/uploads/" + uniqueFileName;
                }
                else
                {
                    user.CheminAvatar = string.Empty;
                }
                user.Id = CurrentUser();
                await _context.Update(user);
                return Redirect("http://localhost:5109/User/Profile");
            }
            
            catch(ArgumentNullException)
            {
                return Redirect("http://localhost:5109/Home/Error");
            }



        }

        // GET: User/Delete/5
        public IActionResult Delete()
        {
            // ReSharper disable once Mvc.ViewNotResolved
            return View();
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _context.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = CurrentUser();
                var currentUser = await _context.GetById(userId);
                return View(currentUser);
            }

            catch
            {
                return Redirect("http://localhost:5109/Home/Error");
            }


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
        // GET: signup
        [AllowAnonymous]
        public  IActionResult SignUp()
        {
            if(CurrentUser() != Guid.Empty)
            {
                return RedirectToAction("Profile");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(User user, IFormFile avatarFile)
        {
                if (avatarFile != null && avatarFile.Length > 0)
                {

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + avatarFile.FileName;

                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatarFile.CopyToAsync(fileStream);
                    }

                    user.CheminAvatar = "/uploads/" + uniqueFileName;
                }
                else
                {
                    user.CheminAvatar = string.Empty;
                }

                _PassWordSecurityContext.hashPasswordWithSalt(ref user);
                user.Admin = false;
                user.Actif = true;

                _context.Add(user);
                await _context.SaveChangesAsync();

                return Redirect("http://localhost:5109/security/Index");
            
        }
    }
}
