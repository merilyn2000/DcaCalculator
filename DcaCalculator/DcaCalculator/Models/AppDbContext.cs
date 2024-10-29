using Microsoft.EntityFrameworkCore;

namespace DcaCalculator.Models
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
    }

}
