using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;

namespace Uptrail_Backend.Services.Interfaces
{
    public interface IPlatformAdminService
    {
        Task<Guid> CreatePlatformAdminAsync(CreatePlatformAdminDto platformAdmin);
        Task<List<PlatformAdminResponseDto>> GetAllPlatformAdminsAsync();
        Task<PlatformAdminResponseDto> GetPlatformAdminByIdAsync(Guid id);
        Task<List<PlatformAdminResponseDto>> SearchPlatformAdminsAsync(string searchTerm);
        Task<PlatformAdminResponseDto> UpdatePlatformAdminByIdAsync(Guid id, UpdatePlatformAdminDto platformAdmin);
        Task DeletePlatformAdminByIdAsync(Guid id);
    }
}
