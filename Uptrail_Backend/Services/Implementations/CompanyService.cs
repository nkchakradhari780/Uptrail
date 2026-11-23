using Uptrail_Backend.Common.Constants;
using Uptrail_Backend.Exceptions;
using Uptrail_Backend.Models.Dtos.CompanyDtos;
using Uptrail_Backend.Repositories.Interfaces;
using Uptrail_Backend.Services.Interfaces;

namespace Uptrail_Backend.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        public async Task<Guid> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
        {
            if (createCompanyDto == null)
            {
                throw new BadRequestException("Company data is required.");
            }

            if (string.IsNullOrWhiteSpace(createCompanyDto.Name) ||
                string.IsNullOrWhiteSpace(createCompanyDto.Email) ||
                string.IsNullOrWhiteSpace(createCompanyDto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(createCompanyDto.Address))
            {
                throw new BadRequestException(ResponseMessages.CompanyCreateFieldsRequired);
            }

            if (await _companyRepository.CompanyEmailExistsAsync(createCompanyDto.Email))
            {
                throw new BadRequestException(ResponseMessages.CompanyAlreadyExists);
            }

            return await _companyRepository.CreateCompanyAsync(createCompanyDto);
        }

        public async Task<List<CompanyResponseDto>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllCompaniesAsync();
            if (companies is null || companies.Count == 0)
            {
                throw new NotFoundException(ResponseMessages.NoCompaniesExist);
            }

            return companies;
        }

        public async Task<CompanyResponseDto> GetCompanyByIdAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.CompanyIdRequired);
            }

            var company = await _companyRepository.GetCompanyByIdAsync(companyId);
            return company ?? throw new NotFoundException(string.Format(ResponseMessages.CompanyNotFoundWithId, companyId));
        }

        public async Task<CompanyResponseDto> UpdateCompanyByIdAsync(Guid companyId, UpdateCompanyDto updateCompanyDto)
        {
            if (companyId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.CompanyIdRequired);
            }

            if (updateCompanyDto == null ||
                string.IsNullOrWhiteSpace(updateCompanyDto.Name) ||
                string.IsNullOrWhiteSpace(updateCompanyDto.Email) ||
                string.IsNullOrWhiteSpace(updateCompanyDto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(updateCompanyDto.Address))
            {
                throw new BadRequestException(ResponseMessages.CompanyUpdateFieldsRequired);
            }

            var existingCompany = await _companyRepository.GetCompanyByIdAsync(companyId);
            if (existingCompany == null)
            {
                throw new NotFoundException(string.Format(ResponseMessages.CompanyNotFoundWithId, companyId));
            }

            if (await _companyRepository.CompanyEmailExistsAsync(updateCompanyDto.Email, companyId))
            {
                throw new BadRequestException(ResponseMessages.CompanyAlreadyExists);
            }

            var updatedCompany = await _companyRepository.UpdateCompanyByIdAsync(companyId, updateCompanyDto);
            return updatedCompany ?? throw new NotFoundException(string.Format(ResponseMessages.CompanyNotFoundWithId, companyId));
        }

        public async Task DeleteCompanyByIdAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
            {
                throw new BadRequestException(ResponseMessages.CompanyIdRequired);
            }

            var existingCompany = await _companyRepository.GetCompanyByIdAsync(companyId);
            if (existingCompany == null)
            {
                throw new NotFoundException(string.Format(ResponseMessages.CompanyNotFoundWithId, companyId));
            }

            if (await _companyRepository.HasActiveUsersOrRolesAsync(companyId))
            {
                throw new BadRequestException(ResponseMessages.CompanyHasActiveEntities);
            }

            if (!await _companyRepository.DeleteCompanyByIdAsync(companyId))
            {
                throw new NotFoundException(string.Format(ResponseMessages.CompanyNotFoundWithId, companyId));
            }
        }

        public async Task<List<CompanyResponseDto>> SearchCompaniesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BadRequestException(ResponseMessages.CompanySearchTermRequired);
            }

            var cleanTerm = searchTerm.Trim().Trim('"', '\'');
            if (cleanTerm.StartsWith("?searchTerm=", StringComparison.OrdinalIgnoreCase))
            {
                cleanTerm = cleanTerm.Substring("?searchTerm=".Length).Trim().Trim('"', '\'');
            }
            else if (cleanTerm.StartsWith("searchTerm=", StringComparison.OrdinalIgnoreCase))
            {
                cleanTerm = cleanTerm.Substring("searchTerm=".Length).Trim().Trim('"', '\'');
            }

            if (string.IsNullOrWhiteSpace(cleanTerm))
            {
                throw new BadRequestException(ResponseMessages.CompanySearchTermRequired);
            }

            var companies = await _companyRepository.SearchCompaniesAsync(cleanTerm);
            if (companies is null || companies.Count == 0)
            {
                throw new NotFoundException(ResponseMessages.NoCompaniesExist);
            }

            return companies;
        }
    }
}
