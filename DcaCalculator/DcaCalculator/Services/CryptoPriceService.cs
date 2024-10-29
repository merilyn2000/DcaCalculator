using DcaCalculator.Models;
using Microsoft.EntityFrameworkCore;

namespace DcaCalculator.Services
{
    public interface ICryptoPriceService
    {
        Task<Dictionary<string, string>> GetCryptocurrencies();
        Task<decimal> GetLatestPrice(string symbol);
        Task<decimal> GetHistoricalPrice(string symbol, DateTime date);
    }

    public class CryptoPriceService(AppDbContext dbContext) : ICryptoPriceService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<Dictionary<string, string>> GetCryptocurrencies()
        {
            return await _dbContext.CryptoCurrencies
                .GroupBy(x => x.Symbol)
                .Select(x => new
                {
                    Symbol = x.Key,
                    x.First().Name
                })
                .Distinct()
                .ToDictionaryAsync(x => x.Symbol, x => x.Name);
        }

        public async Task<decimal> GetLatestPrice(string symbol)
        {
            return await _dbContext.CryptoCurrencies
                .Where(c => c.Symbol == symbol)
                .OrderByDescending(c => c.Date)
                .Select(x => x.Price)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetHistoricalPrice(string symbol, DateTime date)
        {
            return await _dbContext.CryptoCurrencies
                .Where(c => c.Symbol == symbol && c.Date.Date == date.Date)
                .Select(x => x.Price)
                .FirstOrDefaultAsync();
        }
    }
}
