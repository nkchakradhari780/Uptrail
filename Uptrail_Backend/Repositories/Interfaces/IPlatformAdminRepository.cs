using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;

namespace Uptrail_Backend.Repositories.Interfaces
{
    public interface IPlatformAdminRepository
    {
        Task<Guid> CreatePlatformAdminAsync(CreatePlatformAdminDto pAdmin);
        Task<List<PlatformAdminResponseDto>> GetAllPlatformAdminAsync();
        Task<PlatformAdminResponseDto?> GetPlatformAdminByIdAsync(Guid platformAdminId);
        Task<PlatformAdminResponseDto?> GetPlatformAdminByEmailAsync(string email);
        Task<List<PlatformAdminResponseDto>> SearchPlatformAdminsAsync(string searchTerm);
        Task<PlatformAdminResponseDto?> UpdatePlatformAdminByIdAsync(Guid platformAdminId, UpdatePlatformAdminDto pAdmin);
        Task<bool> DeletePlatformAdminById(Guid platformAdminId);
        Task<bool> PlatformAdminEmailExistsAsync(string email);
        Task<bool> PlatformAdminPhoneExistsAsync(string phoneNumber);
    }
}
