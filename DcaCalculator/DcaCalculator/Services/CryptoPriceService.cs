using DcaCalculator.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DcaCalculator.Services
{
    public interface ICryptoPriceService
    {
        Task<Dictionary<string, string>> GetCryptocurrencies();

        Task<decimal> GetLatestPrice(string symbol);
        Task<Dictionary<string, decimal>> GetLatestPrices(IEnumerable<string> symbols);

        Task<Dictionary<DateTime, decimal>> GetHistoricalPrices(string symbol, DateTime startDate, DateTime endDate);
        Task<Dictionary<string, Dictionary<DateTime, decimal>>> GetMultipleHistoricalPrices(IEnumerable<string> symbols, DateTime startDate, DateTime endDate);

        Task<DateTime> GetOldestDate();

        Task<decimal> GetLatestEURConversionRate();
    }

    public class CryptoPriceService(AppDbContext dbContext, HttpClient httpClient) : ICryptoPriceService
    {
        private readonly AppDbContext _dbContext = dbContext;
        private readonly HttpClient _httpClient = httpClient;

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
                .Where(x => x.Symbol == symbol)
                .OrderByDescending(c => c.Date)
                .Select(x => x.Price)
                .FirstOrDefaultAsync();
        }

        public async Task<Dictionary<string, decimal>> GetLatestPrices(IEnumerable<string> symbols)
        {
            return await _dbContext.CryptoCurrencies
               .Where(x => symbols.Contains(x.Symbol))
               .OrderByDescending(x => x.Date)
               .GroupBy(x => x.Symbol)
               .Select(x => new
               {
                   Symbol = x.Key,
                   x.First().Price
               })
               .ToDictionaryAsync(x => x.Symbol, x => x.Price);
        }

        public async Task<Dictionary<DateTime, decimal>> GetHistoricalPrices(string symbol, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.CryptoCurrencies
                .Where(x => x.Symbol == symbol && x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date)
                .ToDictionaryAsync(x => x.Date.Date, x => x.Price);
        }

        public async Task<Dictionary<string, Dictionary<DateTime, decimal>>> GetMultipleHistoricalPrices(IEnumerable<string> symbols, DateTime startDate, DateTime endDate)
        {
            var historicalData = await _dbContext.CryptoCurrencies
                .Where(x => symbols.Contains(x.Symbol) && x.Date.Date >= startDate && x.Date.Date <= endDate)
                .ToListAsync();

            var historicalPrices = new Dictionary<string, Dictionary<DateTime, decimal>>();

            foreach (var entry in historicalData)
            {
                if (!historicalPrices.TryGetValue(entry.Symbol, out Dictionary<DateTime, decimal>? value))
                {
                    value = [];
                    historicalPrices[entry.Symbol] = value;
                }

                value[entry.Date.Date] = entry.Price;
            }

            return historicalPrices;
        }

        public async Task<DateTime> GetOldestDate()
        {
            return await _dbContext.CryptoCurrencies
                .OrderBy(x => x.Date)
                .Select(x => x.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetLatestEURConversionRate()
        {
            var response = await _httpClient.GetStringAsync("https://api.exchangerate-api.com/v4/latest/USD");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true 
            };

            var exchangeRates = JsonSerializer.Deserialize<ExchangeRates>(response, options);

            if (exchangeRates != null && exchangeRates.Rates != null && exchangeRates.Rates.TryGetValue("EUR", out decimal euroRate))
            {
                return euroRate;
            }

            throw new Exception("EUR rate not found in response or deserialization failed.");
        }

        private sealed record ExchangeRates
        {
            public string Provider { get; init; } = string.Empty;
            public string WARNING_UPGRADE_TO_V6 { get; init; } = string.Empty;
            public string Terms { get; init; } = string.Empty;
            public string Base { get; init; } = string.Empty;
            public string Date { get; init; } = string.Empty;
            public long TimeLastUpdated { get; init; }
            public Dictionary<string, decimal> Rates { get; init; } = [];
        }
    }
}
