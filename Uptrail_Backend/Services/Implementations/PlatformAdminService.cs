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
            if (await _repository.PlatformAdminEmailExistsAsync(platformAdmin.Email)
                || await _repository.PlatformAdminPhoneExistsAsync(platformAdmin.PhoneNumber))
            {
                throw new BadRequestException(ResponseMessages.PlatformAdminAlreadyExists);
            }

            platformAdmin.Password = _passwordHasher.HashPassword(platformAdmin.Password);
            return await _repository.CreatePlatformAdminAsync(platformAdmin);
        }

        public Task<List<PlatformAdminResponseDto>> GetAllPlatformAdminsAsync()
        {
            return _repository.GetAllPlatformAdminAsync();
        }

        public async Task<PlatformAdminResponseDto> GetPlatformAdminByIdAsync(Guid id)
        {
            var admin = await _repository.GetPlatformAdminByIdAsync(id);
            return admin ?? throw new NotFoundException($"Platform admin with ID '{id}' was not found.");
        }

        public async Task<List<PlatformAdminResponseDto>> SearchPlatformAdminsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new BadRequestException("A search term (name, email, phone number, or ID) is required.");
            }

            return await _repository.SearchPlatformAdminsAsync(searchTerm.Trim());
        }

        public async Task<PlatformAdminResponseDto> UpdatePlatformAdminByIdAsync(
            Guid id,
            UpdatePlatformAdminDto platformAdmin)
        {
            var existing = await _repository.GetPlatformAdminByIdAsync(id);
            if (existing is null)
            {
                throw new NotFoundException($"Platform admin with ID '{id}' was not found.");
            }

            var emailChanged = !string.Equals(existing.Email, platformAdmin.Email, StringComparison.OrdinalIgnoreCase);
            var phoneChanged = !string.Equals(existing.PhoneNumber, platformAdmin.PhoneNumber, StringComparison.OrdinalIgnoreCase);

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
            if (!await _repository.DeletePlatformAdminById(id))
            {
                throw new NotFoundException($"Platform admin with ID '{id}' was not found.");
            }
        }
    }
}
