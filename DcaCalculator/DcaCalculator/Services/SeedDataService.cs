using DcaCalculator.Models;

namespace DcaCalculator.Services
{
    public class SeedDataService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task Initialize()
        {
            if (_context.CryptoCurrencies.Any())
            {
                return;
            }

            var cryptocurrencies = new List<CryptoCurrency>();
            var startDate = new DateTime(2020, 1, 1);
            var endDate = DateTime.Today;
            Random random = new();

            while (startDate <= endDate)
            {
                cryptocurrencies.Add(new CryptoCurrency
                {
                    Name = "Bitcoin",
                    Symbol = "BTC",
                    Price = Math.Round(20000m + (decimal)(random.NextDouble() * 20000), 2),
                    Date = startDate
                });

                cryptocurrencies.Add(new CryptoCurrency
                {
                    Name = "Ethereum",
                    Symbol = "ETH",
                    Price = Math.Round(1000m + (decimal)(random.NextDouble() * 1000), 2),
                    Date = startDate
                });

                cryptocurrencies.Add(new CryptoCurrency
                {
                    Name = "Ripple",
                    Symbol = "XRP",
                    Price = Math.Round(0.1m + (decimal)(random.NextDouble() * 0.9), 2),
                    Date = startDate
                });

                startDate = startDate.AddDays(1);
            }

            _context.CryptoCurrencies.AddRange(cryptocurrencies);
            await _context.SaveChangesAsync();
        }
    }
}
