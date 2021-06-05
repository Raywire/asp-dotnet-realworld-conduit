using Conduit.Extensions.CustomExceptionMiddleware;
using Microsoft.AspNetCore.Builder;

namespace Conduit.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
