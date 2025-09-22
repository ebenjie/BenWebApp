using BenWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BenWebApp.Data
{
    public interface IDataService<T>
    {
        Task AddAsync(T model);
        Task<IEnumerable<T>> GetAll();
        Task<long> GetLatestCode();
        Task<T?> GetByIdAsync(int id);
        Task<bool> CloseItemAsync(int id);
        Task SendTelegramMessageAsync(T model, string action);
    }
}
