using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using CDS.APICore.Helpers;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace CDS.APICore.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                HttpResponse response = context.Response;
                response.ContentType = "application/json";

                switch (error)
                {
                    case AppException _:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException _:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                string result = JsonConvert.SerializeObject(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
