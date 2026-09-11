    using Serilog;
    using System.Text.Json;
    using TehnologijeWebaProjektPokemonAndrijaFiringer.Models.DTO;

    namespace TehnologijeWebaProjektPokemonAndrijaFiringer.Middlewares
    {
        public class ExceptionMiddleware
        {
            private readonly RequestDelegate _next;

            public ExceptionMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "An unhandled exception occurred.");
                    await HandleExceptionAsync(context, ex);
                }
            }

            private static Task HandleExceptionAsync(HttpContext context, Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;

                var response = new ErrorResponseDto
                {
                    StatusCode = 500,
                    Message = ex.Message // Now returns the ACTUAL error (e.g. "Duplicate username")
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

                return context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
            }
        }
    }