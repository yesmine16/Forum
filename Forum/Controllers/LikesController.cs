using Forum.Models;
using Forum.Repositorys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Forum.Controllers
{
    public class LikesController : Controller
    {
        public readonly LikesRepository _likesRepository;

        public LikesController(LikesRepository LikesRepository)
        {
            _likesRepository = LikesRepository;
        }

        // GET: LikesController
        public ActionResult Index()
        {
            return View();
        }

        // GET: LikesController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LikesController/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddLike(Guid idPost)
        {
            var like = new Likes();
            like.UserId = CurrentUser();
            like.PostId = idPost;
            try
            {
                await _likesRepository.Add(like);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            

        }

        [HttpPost]
        public async Task<IActionResult> RemoveLike(Guid idPost, Guid userId)
        {
            
            try
            {
                await _likesRepository.RemoveLikeAsync(idPost, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpPost]
        public async Task<IActionResult> IsLiked(Guid idPost, Guid userId)
        {

            try
            {
                var result = await _likesRepository.IsLiked(idPost, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        public IActionResult LikesSum(Guid postId)
        {
            int likesCount = _likesRepository.GetPostlikesSum(postId);
            return Ok(likesCount);
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

        // POST: LikesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LikesController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LikesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LikesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LikesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
