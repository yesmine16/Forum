using Forum.Models;
using Forum.Repositorys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Forum.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly PostsRepositroy _context;
        private readonly MessagesRepository _comments;
        private readonly LikesRepository _likes;


        public PostsController(PostsRepositroy context, MessagesRepository comments,LikesRepository likes)
        {
            _context = context;
            _comments = comments;
            _likes = likes;
        }


        // GET: PostsController
        public async Task<IActionResult> Index()
        {
            var currentUser = CurrentUser();
            var posts = await _context.GetAllWithDetails(currentUser);
            posts = posts.OrderBy(post => post.DateCreationMessage).ToList();
            return View(posts);
        }

        public async Task<IActionResult> Posts()
        {
            var currentUser = CurrentUser();
            var posts = await _context.GetAllWithDetails();

            return View(posts);
        }

        // GET: PostsController/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var posts = await _context.GetAllWithDetails();
            var post = posts.Where(post => post.Id == id).FirstOrDefault();
            post.Messages = await _comments.GetAll(post.Id);
            return View(post);
        }

        // GET: PostsController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                var currentUser = CurrentUser();
                post.UserId = currentUser;
                post.DateCreationMessage = DateTime.Now;
                await _context.CreatePost(post);
            }

            return RedirectToAction("Posts");
        }

        // GET: PostsController/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var post = await _context.GetById(id);
                if (post.UserId != CurrentUser())
                {
                    return Unauthorized();
                }
                return View(post);
            }
            catch
            {
                return Redirect("http://localhost:5109/Home/Error");
            }
        }

        // POST: PostsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post)
        {
            try
            {
                await _context.Update(post);
                return RedirectToAction("Index");
            }
            catch
            {
                return Redirect("http://localhost:5109/Home/Error");
            }
        }
               
        // POST: PostsController/Delete/5
        [HttpPost]
        public async Task <IActionResult> Delete(Guid postId)
        {
            try
            {
                var user = CurrentUser();
                await _context.DeleteById(postId);
                await _likes.RemoveLikeAsync(postId,user);
                await _comments.DeleteById(postId,user);
                return Ok();
            }
            catch
            {
                return Redirect("http://localhost:5109/Home/Error");
            }
        }

        public async Task<IActionResult> PostsByUserName(string username)
        {
            var posts = await _context.GetPostsByUserName(username);

            return View(posts);
        }

        public Guid CurrentUser()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.Sid);
            
            return Guid.Parse(userIdClaim?.Value);
        }
    }
}
