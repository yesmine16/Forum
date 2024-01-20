using Microsoft.EntityFrameworkCore;

namespace Forum.Repositorys
{
    public interface IRepository<T> where T : class
    {
        
        public Task<List<T>> GetAll();

        public Task<T> GetById(Guid id);

        public Task<T> Update(T newObject);

        public Task DeleteById(Guid id);
    }
}
