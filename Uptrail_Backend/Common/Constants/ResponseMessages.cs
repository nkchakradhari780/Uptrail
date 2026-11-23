namespace Uptrail_Backend.Common.Constants
{
    public class ResponseMessages
    {
        // Platform Admin CRUD Success Messages
        public const string PlatformAdminCreated = "Platform admin created successfully";
        public const string PlatformAdminUpdated = "Platform admin updated successfully";
        public const string PlatformAdminDeleted = "Platform admin deleted successfully";
        public const string PlatformAdminRetrieved = "Platform admin retrieved successfully";
        public const string PlatformAdminsRetrieved = "Platform admins retrieved successfully";
        public const string PlatformAdminLoggedIn = "Platform admin logged in successfully";
        public const string TokenRefreshed = "Access token refreshed successfully";
        public const string PlatformAdminLoggedOut = "Platform admin logged out successfully";

        // Role CRUD Success Messages
        public const string RoleCreated = "Role created successfully";
        public const string RoleUpdated = "Role updated successfully";
        public const string RoleDeleted = "Role deleted successfully";
        public const string RoleRetrieved = "Role retrieved successfully";
        public const string RolesRetrieved = "Roles retrieved successfully";

        // Company CRUD Success Messages
        public const string CompanyCreated = "Company created successfully";
        public const string CompanyUpdated = "Company updated successfully";
        public const string CompanyDeleted = "Company deleted successfully";
        public const string CompanyRetrieved = "Company retrieved successfully";
        public const string CompaniesRetrieved = "Companies retrieved successfully";

        // Platform Admin Error & Validation Messages
        public const string NoPlatformAdminExist = "No platform admin exist";
        public const string PlatformAdminAlreadyExists = "A platform admin with the same email or phone number already exists";
        public const string PlatformAdminNotFound = "Platform admin was not found.";
        public const string PlatformAdminNotFoundWithId = "Platform admin with ID '{0}' was not found.";
        public const string PlatformAdminNotFoundWithEmail = "Platform admin with email '{0}' was not found.";
        public const string PlatformAdminInactiveOrNotFound = "The platform admin is inactive or does not exist.";
        public const string InvalidEmailOrPassword = "Invalid email or password.";
        public const string EmailAndPasswordRequired = "Email and password are required.";
        public const string RefreshTokenRequired = "The refresh token is required.";
        public const string RefreshTokenInvalidOrExpired = "The refresh token is invalid or expired.";
        public const string AdminCreateFieldsRequired = "Name, Email, Password, and PhoneNumber are required.";
        public const string AdminUpdateFieldsRequired = "Name, Email, and PhoneNumber are required.";
        public const string AdminIdRequired = "A valid non-empty platform admin ID is required.";
        public const string EmailRequired = "A valid email address is required.";
        public const string SearchTermRequired = "A search term (name, email, phone number, or ID) is required.";

        // Role Error & Validation Messages
        public const string NoRolesExist = "No roles exist";
        public const string RoleAlreadyExists = "A role with the same name already exists in this company";
        public const string RoleNotFound = "Role was not found.";
        public const string RoleNotFoundWithId = "Role with ID '{0}' was not found.";
        public const string RoleNameRequired = "Role name is required.";
        public const string RoleIdRequired = "A valid non-empty role ID is required.";
        public const string CompanyIdRequired = "A valid non-empty company ID is required.";
        public const string CompanyNotFoundWithId = "Company with ID '{0}' was not found.";
        public const string RoleSearchTermRequired = "A search term (role name, ID, or company ID) is required.";
        public const string RoleHasActiveUsers = "Cannot delete this role because active users are currently assigned to it.";

        // Company Error & Validation Messages
        public const string NoCompaniesExist = "No companies exist";
        public const string CompanyAlreadyExists = "A company with the same email already exists";
        public const string CompanyNotFound = "Company was not found.";
        public const string CompanyCreateFieldsRequired = "Name, Email, PhoneNumber, and Address are required.";
        public const string CompanyUpdateFieldsRequired = "Name, Email, PhoneNumber, and Address are required.";
        public const string CompanySearchTermRequired = "A search term (company name, email, phone, or ID) is required.";
        public const string CompanyHasActiveEntities = "Cannot delete this company because active users or roles are currently associated with it.";

        // General System Messages
        public const string ValidationFailed = "Validation failed. Please verify the submitted data.";
        public const string ConcurrencyError = "The record was modified by another operation. Please refresh and try again.";
        public const string InternalServerError = "An unexpected internal server error occurred.";
    }
}
