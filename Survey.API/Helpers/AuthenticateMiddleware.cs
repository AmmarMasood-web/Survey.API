namespace Survey.API.Helpers;

public class AuthenticateMiddleware
{
    private readonly RequestDelegate _requestDelegate;
    public AuthenticateMiddleware(RequestDelegate requestDelegate)
    {
        this._requestDelegate = requestDelegate;
    }
    public async Task Invoke(HttpContext context)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            context.Response.StatusCode = 401;
            return;
        }
        await _requestDelegate(context);
    }
}
