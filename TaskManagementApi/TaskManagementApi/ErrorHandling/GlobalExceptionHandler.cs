using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TaskManagementApi.Application.Exceptions;

namespace TaskManagementApi.ErrorHandling
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService, IHostEnvironment env)
        {
            _logger = logger;
            _problemDetailsService = problemDetailsService;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var (status, title, detail) = exception switch
            {
                InvalidReferenceException => (400, "Invalid reference", exception.Message),
                NotFoundException    => (404, "Resource not found", exception.Message),
                _ => (500, "An unexpected error occurred.", _env.IsDevelopment() ? exception.Message : null)
            };

            if (status >= 500)
                _logger.LogError(exception, "Unhandled exception");
            else
                _logger.LogInformation("Handled {Exception}: {Message}", exception.GetType().Name, exception.Message);

            httpContext.Response.StatusCode = status;

            return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = title,
                    Detail = detail
                }
            });
        }

    }
}
