// <copyright file="ErrorHandlingMiddleware.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Middlewares;

using System.Net;
using Microsoft.AspNetCore.Http;

/// <summary>
/// Middleware for handling exceptions and returning appropriate error responses.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private static ILogger<ErrorHandlingMiddleware> logger = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger for error logging.</param>
    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        this.next = next;
        ErrorHandlingMiddleware.logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to handle the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (this is not null)
            {
                await this.next(context);
            }
            else
            {
                //what?
                throw new ArgumentNullException(nameof(ErrorHandlingMiddleware));
            }
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        object? errors = null;

        if (exception is RestException re)
        {
            statusCode = (int)re.Code;

            if (re.Message != null & re.Message is string)
            {
                errors = new[] { re.Message };
            }
        }
        else
        {
            statusCode = (int)HttpStatusCode.InternalServerError;
            errors = "An internal server error has occured.";
        }

        logger.LogError(exception, "Error occurred - Errors: {Errors} - Source: {Source} - TargetSite: {TargetSite}",
            errors, exception.Source, exception.TargetSite?.Name);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new { errors }));
    }
}
