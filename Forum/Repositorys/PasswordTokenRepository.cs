using Forum.Models;
using Forum.Tools;
using Microsoft.EntityFrameworkCore;

namespace Forum.Repositorys
{
    public class PasswordTokenRepository : DbContext
    {
        public PasswordTokenRepository(DbContextOptions<PasswordTokenRepository> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<PasswordToken> Tokens { get; set; }

        public async Task<Guid?> GetLastValidToken(string email)
        {
            var tokens = Tokens.Where(token => token.Email == email && token.ValidationInterval >= DateTime.Now)
                .OrderBy(date => date.ValidationInterval);
            PasswordToken? token = await tokens.FirstOrDefaultAsync();
            return token?.Token;
        }

        public async Task<PasswordToken> GetTokenById(Guid token)
        {
            var actualToken = await Tokens.FirstOrDefaultAsync(tokens => tokens.Token == token);
            ArgumentNullException.ThrowIfNull(actualToken);
            return actualToken;
        }
        public async Task SaveTokenAsync(PasswordToken token)
        {
            var addMinutes = token.ValidationInterval.AddMinutes(20);
            token.ValidationInterval = addMinutes;

            Add(token);
            await SaveChangesAsync();
        }

        public async Task DeleteConsumedToken(Guid token)
        {
            try
            {
                var actualTokenObject = await GetTokenById(token);
                Tokens.Remove(actualTokenObject);
                await SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine($"cannot remove token {token} is is missing or {e.Message} caused the issue");
            }
        }
    }
}
