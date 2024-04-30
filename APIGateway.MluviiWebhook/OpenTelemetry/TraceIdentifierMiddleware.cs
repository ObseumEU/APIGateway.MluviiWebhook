using System.Diagnostics;

namespace APIGateway.MluviiWebhook.OpenTelemetry
{
    public class TraceIdentifierMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdentifierMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<TraceIdentifierMiddleware> logger)
        {
            using (logger.BeginScope(new { TraceId = Activity.Current?.TraceId.ToString() }))
            {
                await _next(context);
            }
        }
    }

}
