using BenWebApp.Data;
using BenWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BenWebApp.Data
{
    public class NewPharmaService : IDataService<NewItemPharmaModel>
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;

        public NewPharmaService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ Add new Pharma item
        public async Task AddAsync(NewItemPharmaModel model)
        {
            // Save Pharma item
            await _context.NewItemPharma.AddAsync(model);

            // Update or insert CodePerDept record
            var existingCode = await _context.CodePerDepts
                .FirstOrDefaultAsync(c => c.Department == "Pharmacy");

            if (existingCode != null)
            {
                existingCode.Code = model.ItemCode;
                _context.CodePerDepts.Update(existingCode);
            }
            else
            {
                await _context.CodePerDepts.AddAsync(new CodePerDeptModel
                {
                    Department = "Pharmacy",
                    Code = model.ItemCode
                });
            }

            await _context.SaveChangesAsync();

            // Notify Telegram
            await SendTelegramMessageAsync(model, "Added");
        }

        // ✅ Get all OPEN items
        public async Task<IEnumerable<NewItemPharmaModel>> GetAll()
        {
            return await _context.NewItemPharma
                                 .Where(x => x.IT_Status == "Open")
                                 .OrderByDescending(x => x.RequestDate)
                                 .ToListAsync();
        }

        // ✅ Get item by Id
        public async Task<NewItemPharmaModel?> GetByIdAsync(int id)
        {
            return await _context.NewItemPharma.FirstOrDefaultAsync(x => x.Id == id);
        }

        // ✅ Close item & notify
        public async Task<bool> CloseItemAsync(int id)
        {
            var item = await GetByIdAsync(id);
            if (item == null) return false;

            item.IT_Status = "Closed";
            _context.NewItemPharma.Update(item);
            await _context.SaveChangesAsync();

            await SendTelegramMessageAsync(item, "Closed");
            return true;
        }

        // ✅ Generate next code
        public async Task<long> GetLatestCode()
        {
            var latestCode = await _context.CodePerDepts
                                           .Where(c => c.Department == "Pharmacy")
                                           .OrderByDescending(c => c.Id)
                                           .Select(c => c.Code)
                                           .FirstOrDefaultAsync();

            return latestCode == 0 ? 4000000000000 : latestCode + 1;
        }

        // ✅ Flexible Telegram notification
        public async Task SendTelegramMessageAsync(NewItemPharmaModel item, string action)
        {
            try
            {
                var botToken = _configuration["Telegram:BotToken"];
                var chatId = _configuration["Telegram:ChatId"];

                var message = $"[Pharma Item {action}]\n\n" +
                              $"ItemCode: {item.ItemCode}\n" +
                              $"Description: {item.Description}\n" +
                              $"Generic: {item.GenericName}\n" +
                              $"SmallUnit: {item.SmallUnit}\n" +
                              $"BigUnit: {item.BigUnit}\n" +
                              $"Price: {item.SellingPrice}\n" +
                              $"Requested By: {item.RequestedBy}\n" +
                              $"Date: {item.RequestDate:MM-dd-yyyy}\n" +
                              $"Status: {item.IT_Status}";

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
