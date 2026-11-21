namespace Uptrail_Backend.Common.Responses
{
    public class ApiResponseFactory
    {
        public static ApiResponse<T> SuccessResponse<T>(T data, string message = "Request successful")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> ErrorResponse<T>(string message, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }
}
