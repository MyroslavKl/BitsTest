using BitsMVCProject.Models;

namespace BitsMVCProject.Services
{
    public interface IContactService
    {
        Task<List<ContactModel>> GetAllAsync();
        Task<ContactModel?> GetByIdAsync(int id);
        Task ImportFromCsvAsync(IFormFile file);
        Task<ContactModel?> UpdateAsync(ContactModel update);
        Task<bool> DeleteAsync(int id);
    }
}
