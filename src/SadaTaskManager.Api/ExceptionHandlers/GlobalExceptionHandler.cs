using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SadaTaskManager.Domain;

namespace SadaTaskManager.Api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly IProblemDetailsService _problemDetailsService;
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
	{
		_problemDetailsService = problemDetailsService;
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		ProblemDetails problemDetails = new ProblemDetails();

		if (exception is DomainException)
		{
			_logger.LogWarning("Regra de negócio violada: {Message}", exception.Message);

			problemDetails.Status = StatusCodes.Status400BadRequest;
			problemDetails.Title = "Requisição inválida";
			problemDetails.Detail = exception.Message;
		}
		else
		{
			_logger.LogError(exception, "Erro inesperado em {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);

			problemDetails.Status = StatusCodes.Status500InternalServerError;
			problemDetails.Title = "Erro interno";
			problemDetails.Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.";
		}

		httpContext.Response.StatusCode = problemDetails.Status.Value;

		return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			ProblemDetails = problemDetails,
			Exception = exception
		});
	}
}
