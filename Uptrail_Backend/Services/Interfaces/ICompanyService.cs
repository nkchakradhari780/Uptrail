using Uptrail_Backend.Models.Dtos.CompanyDtos;

namespace Uptrail_Backend.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Guid> CreateCompanyAsync(CreateCompanyDto createCompanyDto);
        Task<List<CompanyResponseDto>> GetAllCompaniesAsync();
        Task<CompanyResponseDto> GetCompanyByIdAsync(Guid companyId);
        Task<CompanyResponseDto> UpdateCompanyByIdAsync(Guid companyId, UpdateCompanyDto updateCompanyDto);
        Task DeleteCompanyByIdAsync(Guid companyId);
        Task<List<CompanyResponseDto>> SearchCompaniesAsync(string searchTerm);
    }
}
