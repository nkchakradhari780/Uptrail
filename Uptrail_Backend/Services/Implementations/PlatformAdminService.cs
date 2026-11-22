using Uptrail_Backend.Common.Constants;
using Uptrail_Backend.Exceptions;
using Uptrail_Backend.Helpers.Interfaces;
using Uptrail_Backend.Models.Dtos.PlatformAdminDtos;
using Uptrail_Backend.Repositories.Interfaces;
using Uptrail_Backend.Services.Interfaces;

namespace Uptrail_Backend.Services.Implementations
{
    public class PlatformAdminService : IPlatformAdminService
    {
        private readonly IPlatformAdminRepository _repository;
        private readonly IPasswordHasher _passwordHasher;

        public PlatformAdminService(
            IPlatformAdminRepository repository,
            IPasswordHasher passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Guid> CreatePlatformAdminAsync(CreatePlatformAdminDto platformAdmin)
        {
            if (platformAdmin == null
                || string.IsNullOrWhiteSpace(platformAdmin.Name)
                || string.IsNullOrWhiteSpace(platformAdmin.Email)
                || string.IsNullOrWhiteSpace(platformAdmin.Password)
                || string.IsNullOrWhiteSpace(platformAdmin.PhoneNumber))
            {
                throw new BadRequestException("Name, Email, Password, and PhoneNumber are required.");
            }

            if (await _repository.PlatformAdminEmailExistsAsync(platformAdmin.Email)
                || await _repository.PlatformAdminPhoneExistsAsync(platformAdmin.PhoneNumber))
            {
                throw new BadRequestException(ResponseMessages.PlatformAdminAlreadyExists);
            }

            platformAdmin.Password = _passwordHasher.HashPassword(platformAdmin.Password);
            return await _repository.CreatePlatformAdminAsync(platformAdmin);
        }

        public async Task<List<PlatformAdminResponseDto>> GetAllPlatformAdminsAsync()
        {
            var admins = await _repository.GetAllPlatformAdminAsync();
            if (admins is null || admins.Count == 0)
            {
                throw new NotFoundException(ResponseMessages.NoPlatformAdminExist);
            }

            return admins;
        }

        public async Task<PlatformAdminResponseDto> GetPlatformAdminByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BadRequestException("A valid non-empty platform admin ID is required.");
            }

            var admin = await _repository.GetPlatformAdminByIdAsync(id);
            return admin ?? throw new NotFoundException($"Platform admin with ID '{id}' was not found.");
        }

        public async Task<PlatformAdminResponseDto> GetPlatformAdminByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new BadRequestException("A valid email address is required.");
            }

            var admin = await _repository.GetPlatformAdminByEmailAsync(email.Trim());
            return admin ?? throw new NotFoundException($"Platform admin with email '{email}' was not found.");
        }

        public async Task<List<PlatformAdminResponseDto>> SearchPlatformAdminsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BadRequestException("A search term (name, email, phone number, or ID) is required.");
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
                throw new BadRequestException("A search term (name, email, phone number, or ID) is required.");
            }

            var admins = await _repository.SearchPlatformAdminsAsync(cleanTerm);
            if (admins is null || admins.Count == 0)
            {
                throw new NotFoundException(ResponseMessages.NoPlatformAdminExist);
            }

            return admins;
        }

        public async Task<PlatformAdminResponseDto> UpdatePlatformAdminByIdAsync(
            Guid id,
            UpdatePlatformAdminDto platformAdmin)
        {
            if (id == Guid.Empty)
            {
                throw new BadRequestException("A valid non-empty platform admin ID is required.");
            }

            if (platformAdmin == null
                || string.IsNullOrWhiteSpace(platformAdmin.Name)
                || string.IsNullOrWhiteSpace(platformAdmin.Email)
                || string.IsNullOrWhiteSpace(platformAdmin.PhoneNumber))
            {
                throw new BadRequestException("Name, Email, and PhoneNumber are required.");
            }

            var existing = await _repository.GetPlatformAdminByIdAsync(id);
            if (existing is null)
            {
                throw new NotFoundException($"Platform admin with ID '{id}' was not found.");
            }

            var emailChanged = !string.Equals(existing.Email, platformAdmin.Email.Trim(), StringComparison.OrdinalIgnoreCase);
            var phoneChanged = !string.Equals(existing.PhoneNumber, platformAdmin.PhoneNumber.Trim(), StringComparison.OrdinalIgnoreCase);

            if ((emailChanged && await _repository.PlatformAdminEmailExistsAsync(platformAdmin.Email))
                || (phoneChanged && await _repository.PlatformAdminPhoneExistsAsync(platformAdmin.PhoneNumber)))
            {
                throw new BadRequestException(ResponseMessages.PlatformAdminAlreadyExists);
            }

            var updated = await _repository.UpdatePlatformAdminByIdAsync(id, platformAdmin);
            return updated ?? throw new NotFoundException($"Platform admin with ID '{id}' was not found.");
        }

        public async Task DeletePlatformAdminByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new BadRequestException("A valid non-empty platform admin ID is required.");
            }

            if (!await _repository.DeletePlatformAdminById(id))
            {
                throw new NotFoundException($"Platform admin with ID '{id}' was not found.");
            }
        }
    }
}
