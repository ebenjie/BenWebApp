using BenWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BenWebApp.Data
{
    public class NewCSRService : IDataService<NewItemCSRModel>
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        public NewCSRService(IConfiguration configuration, DataContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task AddAsync(NewItemCSRModel model)
        {
            // Save Pharma item
            await _context.NewItemCSR.AddAsync(model);

            // Update or insert CodePerDept record
            var existingCode = await _context.CodePerDepts
                .FirstOrDefaultAsync(c => c.Department == "CSR");

            if (existingCode != null)
            {
                existingCode.Code = model.ItemCode;
                _context.CodePerDepts.Update(existingCode);
            }
            else
            {
                await _context.CodePerDepts.AddAsync(new CodePerDeptModel
                {
                    Department = "CSR",
                    Code = model.ItemCode
                });
            }

            await _context.SaveChangesAsync();

            // Notify Telegram
            await SendTelegramMessageAsync(model);
        }

        public async Task<bool> CloseItemAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item == null) return false;

            item.IT_Status = "Closed";
            _context.NewItemCSR.Update(item);
            await _context.SaveChangesAsync();

            await SendTelegramMessageAsync(item);
            return true;
        }

        public async Task<IEnumerable<NewItemCSRModel>> GetAll()
        {
            return await _context.NewItemCSR
                                 .Where(x => x.IT_Status == "Open")
                                 .OrderByDescending(x => x.RequestDate)
                                 .ToListAsync();
        }

        public async Task<NewItemCSRModel?> GetByIdAsync(int id)
        {
            return await _context.NewItemCSR.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<long> GetLatestCode()
        {
            var latestCode = await _context.CodePerDepts
                                           .Where(c => c.Department == "CSR")
                                           .OrderByDescending(c => c.Id)
                                           .Select(c => c.Code)
                                           .FirstOrDefaultAsync();

            return latestCode == 0 ? 4000000000000 : latestCode + 1;
        }

        public async Task SendTelegramMessageAsync(NewItemCSRModel item)
        {
            try
            {
                var botToken = _configuration["Telegram:BotToken"];
                var chatId = _configuration["Telegram:ChatId"];

                var message = $"[CSR Item Code Request]\n\n" +
                              $"ItemCode: {item.ItemCode}\n" +
                              $"Description: {item.Description}\n" +
                              $"ItemCategory: {item.ItemCategory}\n" +
                              $"Conversion: {item.Conversion}\n" +
                              $"UnitCost: {item.UnitCost}\n" +
                              $"SmallUnit: {item.SmallUnit}\n" +
                              $"BigUnit: {item.BigUnit}\n" +
                              $"MarkUp: {item.MarkUp}\n" +
                              $"Price: {item.SellingPrice}\n" +
                              $"Requested By: {item.RequestedBy}\n" +
                              $"Date: {item.RequestDate:MM-dd-yyyy}";
                              //$"Status: {item.IT_Status}";

                using var client = new HttpClient();
                var url = $"https://api.telegram.org/bot{botToken}/sendMessage" +
                          $"?chat_id={chatId}&text={Uri.EscapeDataString(message)}";

                await client.GetAsync(url);
            }
            catch (Exception ex)
            {
                // log error but don’t throw (so DB save still succeeds)
                Console.WriteLine($"Telegram send failed: {ex.Message}");
            }
        }

        
    }
}
