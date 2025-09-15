namespace VemQueCabe.Api.Middlewares;

/// <summary>
/// Middleware to handle exceptions and provide a consistent error response.
/// </summary>
public class ErrorHandleMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    /// Constructor for the ErrorHandleMiddleware.
    /// </summary>
    /// <param name="next">Delegate representing the next middleware in the pipeline.</param>
    public ErrorHandleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    /// <param name="context">Current HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = ex switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            var response = new
            {
                error = ex.Message,
                stackTrace = ex.StackTrace
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
