using aex.devtest.application.Middleware;

namespace aex.devtest.Controllers
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Adds middleware for global exception logging
        /// </summary> 
        public static void UseExceptionLogHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionLogMiddleware>();
        }

        /// <summary>
        /// Adds middleware for the formatting of custom exceptions
        /// </summary>
        public static void UseExceptionResponseHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionResponseMiddleware>();
        }
    }
}
