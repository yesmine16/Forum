using Forum.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.Repositorys
{
    public class MessagesRepository : DbContext, IRepository<Messages>
    {

        public MessagesRepository(DbContextOptions<MessagesRepository> options)
        :base(options)
        {

        }

        public DbSet<Messages> Messages { get; set; }

        public async Task<List<Messages>> GetAll(Guid id)
        {
            if (Messages == null)
            {
                throw new ArgumentNullException();
            }

            var messages =  await Messages.ToListAsync();
            return messages.Where(message => message.PostId == id).ToList();
        }

        public async Task<int> CommentsSum(Guid id)
        {
            if (Messages == null)
            {
                throw new ArgumentNullException();
            }

            var messages = await Messages.ToListAsync();
            return messages.Where(message => message.PostId == id).ToList().Count();
        }

        public async Task CreateComment(Messages message)
        {
            var counter = 0;
            retry:

            try
            {
                Messages.Add(message);
            await SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (counter > 3) throw;
                counter++;
                goto retry;

            }
        }

        public Task<List<Messages>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<Messages> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Messages> Update(Messages newObject)
        {
            throw new NotImplementedException();
        }

        public Task DeleteById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteById(Guid postId,Guid userId)
        {
            var comment = await Messages.FirstOrDefaultAsync(comment => comment.userId == userId && comment.PostId == postId) ?? throw new Exception();
            Messages.Remove(comment);
            await SaveChangesAsync();
        }
    }
}
