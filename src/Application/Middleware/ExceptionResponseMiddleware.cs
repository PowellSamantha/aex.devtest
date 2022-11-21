using aex.devtest.application.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Security;

namespace aex.devtest.application.Middleware
{
    public class ExceptionResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var result = new ControllerResult { Faults = new List<Fault> { } };
            var status = (int)HttpStatusCode.OK;

            switch (ex.GetType().Name)
            {
                case nameof(SecurityException):
                    status = (int)HttpStatusCode.Forbidden;

                    result.Faults.Add(new Fault
                    {
                        Type = nameof(SecurityException),
                        Messasge = ex.Message
                    });

                    break;

                case nameof(ValidationException):
                    status = (int)HttpStatusCode.BadRequest;

                    result.Faults.Add(new Fault
                    {
                        Type = nameof(ValidationException),
                        Messasge = ex.Message
                    });
                    break;
                default:
                    status = (int)HttpStatusCode.InternalServerError;

                    result.Faults.Add(new Fault
                    {
                        Type = ex.GetType().Name,
                        Messasge = ex.Message
                    });

                    break;
            }

            result.Status = (int)status;

            var jsonResult = JsonConvert.SerializeObject(result);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(jsonResult);
        }
    }
}
