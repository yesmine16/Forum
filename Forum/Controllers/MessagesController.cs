using Azure;
using Forum.Models;
using Forum.Repositorys;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Forum.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly MessagesRepository _context;
        public MessagesController(MessagesRepository context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(string commentContent, string postId)
        {
            var message = new Messages
            {
                Id = Guid.NewGuid(),
                userId = Guid.Parse(User.FindFirstValue(ClaimTypes.Sid)),
                TimeStamp = DateTime.Now,
                Issuer = User.FindFirstValue(ClaimTypes.Name),
                Content = commentContent,
                PostId = Guid.Parse(postId)
            };

            _context.Add(message);
                await _context.SaveChangesAsync();
                return Ok();

        }

        [HttpGet]
        public async Task<IActionResult> CommentsSum(Guid postId)
        {
            return Ok(await _context.CommentsSum(postId));
        }
    }
}
