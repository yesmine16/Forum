using System.Drawing;

namespace Forum.Repositorys
{
    using Forum.Models;
    using Forum.Tools;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class UserRepository : DbContext, IRepository<User>
    {
        private readonly IPasswordSecurity _PassWordSecurityContext;

        public UserRepository(DbContextOptions<UserRepository> options, IPasswordSecurity passwordSecurity)
            : base(options)
        {
            _PassWordSecurityContext = passwordSecurity;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Pseudonyme)
                .IsUnique();


            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        public DbSet<User> Users { get; set; }

        public async Task DeleteById(Guid id)
        {
            var user = await Users.FirstOrDefaultAsync(u => u.Id == id);
            ArgumentNullException.ThrowIfNull(user);
            Users.Remove(user);
            await SaveChangesAsync();
           
        }

        public async Task<User> GetById(Guid id)
        {
            var user = await Users.FirstOrDefaultAsync(user => user.Id == id);
            ArgumentNullException.ThrowIfNull(user);
            return user;
        }

        
        public async Task<List<User>> GetAll()
        {
            ArgumentNullException.ThrowIfNull(Users);
            return await Users.ToListAsync();
        }

        public async Task<User> Update(User newObject)
        {
            try
            {
                var existingUser = await Users.FirstOrDefaultAsync(u => u.Id == newObject.Id);

                if (existingUser != null)
                {
                    existingUser.Pseudonyme = newObject.Pseudonyme;
                    existingUser.Email = newObject.Email;
                    existingUser.CheminAvatar = newObject.CheminAvatar;

                    await SaveChangesAsync();

                    return existingUser;
                }
                else
                {
                    throw new ArgumentNullException("User not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Something went wrong updating user " + newObject.Id + ": " + ex.Message);
                throw;
            }

        }

        public async Task<User> ValidUser(LoginModel loginModel)
        {
            try
            {
                if (loginModel == null)
                {
                    throw new ArgumentNullException(nameof(loginModel), "Login model cannot be null");
                }

                if (Users == null)
                {
                    throw new InvalidOperationException("Users DbSet is not initialized");
                }

                var existingUser = new User();
                existingUser.MotDePasse = loginModel.Password;
                _PassWordSecurityContext.hashPasswordWithSalt(ref existingUser);
                existingUser = await Users.FirstOrDefaultAsync(user =>
                    user.Pseudonyme == loginModel.Username && user.MotDePasse == existingUser.MotDePasse);
                
                return existingUser;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await Users.FirstOrDefaultAsync(user => user.Email == email);
            ArgumentNullException.ThrowIfNull(user);
            return user;
        }

        public async Task<User> UpdatePasswordAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            _PassWordSecurityContext.hashPasswordWithSalt(ref user);
            await SaveChangesAsync();
            return user;
        }

        public async Task<bool> IsValidEmail(string email)
        {
            var user = await Users.FirstOrDefaultAsync(user => user.Email == email);

            if (user == null)
            {
                return false;
            }

            return true;
        }

    }
}
