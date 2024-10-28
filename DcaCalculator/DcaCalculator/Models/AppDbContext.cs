using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Server.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
    }

}
