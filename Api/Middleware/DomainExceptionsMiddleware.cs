using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AspCoreCardGameEngine.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AspCoreCardGameEngine.Api.Middleware
{
    public class DomainExceptionsMiddleware
    {
        private readonly ILogger<DomainExceptionsMiddleware> _logger;
        private readonly RequestDelegate _next;

        public DomainExceptionsMiddleware(
            ILogger<DomainExceptionsMiddleware> logger,
            RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException exception)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning($"The response has already started, the {nameof(DomainExceptionsMiddleware)} will not be executed.");
                    throw;
                }

                switch (exception.ErrorCode)
                {
                    case DomainErrorCode.Forbidden:
                        _logger.LogWarning($"Forbidden domain exception: {exception.Message}. Stack: {exception.StackTrace}");
                        await WriteJsonError(context, HttpStatusCode.Forbidden, exception.ErrorCode, nameof(HttpStatusCode.Forbidden), exception.Message);
                        return;

                    case DomainErrorCode.InconsistentData:
                        _logger.LogWarning($"InconsistentData domain exception: {exception.Message}. Stack: {exception.StackTrace}");
                        await WriteJsonError(context, HttpStatusCode.InternalServerError, exception.ErrorCode, nameof(HttpStatusCode.InternalServerError), exception.Message);
                        return;

                    case DomainErrorCode.BadRequest:
                    case DomainErrorCode.EntityMissing:
                        await WriteJsonError(context, HttpStatusCode.BadRequest, exception.ErrorCode, exception.Message, exception.Message);
                        return;

                    default:
                        throw;
                }
            }
        }

        private static async Task WriteJsonError(
            HttpContext context,
            HttpStatusCode statusCode,
            DomainErrorCode errorCode,
            string reasonPhrase,
            string errorMessage)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;
            context.Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = reasonPhrase;

            var response = new Dictionary<string, object>
            {
                {"ErrorCode", (int) errorCode},
                {"ErrorMessage", errorMessage},
            };

            var jsonBody = JsonConvert.SerializeObject(response);

            context.Response.ContentLength = Encoding.UTF8.GetByteCount(jsonBody);
            await context.Response.WriteAsync(jsonBody);
        }
    }
}