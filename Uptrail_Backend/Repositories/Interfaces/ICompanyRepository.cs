using Uptrail_Backend.Models.Dtos.CompanyDtos;

namespace Uptrail_Backend.Repositories.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Guid> CreateCompanyAsync(CreateCompanyDto createCompanyDto);
        Task<List<CompanyResponseDto>> GetAllCompaniesAsync();
        Task<CompanyResponseDto?> GetCompanyByIdAsync(Guid companyId);
        Task<CompanyResponseDto?> UpdateCompanyByIdAsync(Guid companyId, UpdateCompanyDto updateCompanyDto);
        Task<bool> DeleteCompanyByIdAsync(Guid companyId);
        Task<List<CompanyResponseDto>> SearchCompaniesAsync(string searchTerm);
        Task<bool> CompanyEmailExistsAsync(string email, Guid? excludeCompanyId = null);
        Task<bool> HasActiveUsersOrRolesAsync(Guid companyId);
    }
}
