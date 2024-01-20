using Forum.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.Repositorys
{
    public class LikesRepository : DbContext, IRepository<Likes>
    {

        public LikesRepository(DbContextOptions<LikesRepository> options)
        : base(options)
        {

        }

        public DbSet<Likes> Likes { get; set; }
        public Task DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Likes>> GetAll()
        {
            return await Likes.ToListAsync();
        }

        public async Task Add(Likes like)
        {
            try
            {
                Likes.Add(like);
                await SaveChangesAsync();
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.Message);
            }
        }

        public async Task RemoveLikeAsync(Guid postId, Guid userId)
        {
            var like = await Likes.FirstOrDefaultAsync(like => like.UserId == userId && like.PostId == postId) ?? throw new Exception();
            Likes.Remove(like);
            await SaveChangesAsync();
        }

        public async Task<bool> IsLiked(Guid postId, Guid userId)
        {
            var like = await Likes.FirstOrDefaultAsync(like => like.UserId == userId && like.PostId == postId);
            if (like == null) return false;
            return true;
        }

        public Task<Likes> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Likes> Update(Likes newObject)
        {
            throw new NotImplementedException();
        }

        public int GetPostlikesSum(Guid postId)
        {
            //var postLikes = await Likes.Where(post => post.PostId == postId);
            return Likes.Where(post => post.PostId == postId).Count();

        }
    }
}
