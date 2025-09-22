using BenWebApp.Models;

namespace BenWebApp.Data
{
    public class NewCSRService : IDataService<NewItemCSRModel>
    {
        public Task AddAsync(NewItemCSRModel model)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<NewItemCSRModel>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task SendTelegramMessenger(NewItemCSRModel model)
        {
            throw new NotImplementedException();
        }
    }
}
