using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uptrail_Backend.Common.Constants;
using Uptrail_Backend.Common.Responses;
using Uptrail_Backend.Models.Dtos.CompanyDtos;
using Uptrail_Backend.Services.Interfaces;

namespace Uptrail_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(userIdStr, out var id) ? id : null;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCompanyDto request)
        {
            if (request != null && (!request.CreatedBy.HasValue || request.CreatedBy.Value == Guid.Empty))
            {
                request.CreatedBy = GetCurrentUserId();
            }

            var id = await _companyService.CreateCompanyAsync(request!);
            var response = ApiResponseFactory.SuccessResponse(id, ResponseMessages.CompanyCreated);
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<CompanyResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(ApiResponseFactory.SuccessResponse(companies, ResponseMessages.CompaniesRetrieved));
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(ApiResponse<List<CompanyResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var companies = await _companyService.SearchCompaniesAsync(searchTerm);
            return Ok(ApiResponseFactory.SuccessResponse(companies, ResponseMessages.CompaniesRetrieved));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CompanyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            return Ok(ApiResponseFactory.SuccessResponse(company, ResponseMessages.CompanyRetrieved));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<CompanyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyDto request)
        {
            if (request != null && (!request.UpdatedBy.HasValue || request.UpdatedBy.Value == Guid.Empty))
            {
                request.UpdatedBy = GetCurrentUserId();
            }

            var company = await _companyService.UpdateCompanyByIdAsync(id, request!);
            return Ok(ApiResponseFactory.SuccessResponse(company, ResponseMessages.CompanyUpdated));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _companyService.DeleteCompanyByIdAsync(id);
            return Ok(ApiResponseFactory.SuccessResponse<object?>(null, ResponseMessages.CompanyDeleted));
        }
    }
}
