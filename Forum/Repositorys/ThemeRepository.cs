using Forum.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.Repositorys
{
    public class ThemeRepository : DbContext, IRepository<Theme>
    {
        public ThemeRepository(DbContextOptions<ThemeRepository> options)
            : base(options)
        {
        }

        public DbSet<Theme> Themes { get; set; }

        public async Task DeleteById(Guid id)
        {
            var theme = await GetById(id);
            if (theme != null)
            {
                Themes.Remove(theme);
                await SaveChangesAsync();
            }
        }

        public async Task<List<Theme>> GetAll()
        {
            return await Themes.ToListAsync();
        }

        public async Task<Theme> GetById(Guid id)
        {
            return await Themes.FindAsync(id) ?? throw new ArgumentNullException();
        }

        public async Task<Theme> Update(Theme newObject)
        {
            var existingTheme = await Themes.FindAsync(newObject.id);

            if (existingTheme != null)
            {
                existingTheme.titre = newObject.titre;
                await SaveChangesAsync();
                return existingTheme;
            }
            throw new ArgumentException();
        }
    }
}
