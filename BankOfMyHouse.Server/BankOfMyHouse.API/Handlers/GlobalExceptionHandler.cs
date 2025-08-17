using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace BankOfMyHouse.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		_logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

		var (statusCode, title, detail) = GetErrorResponse(exception);

		var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
		{
			Status = (int)statusCode,
			Title = title,
			Detail = detail,
			Instance = httpContext.Request.Path
		};

		// Add timestamp for debugging
		problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

		httpContext.Response.StatusCode = (int)statusCode;
		await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

		return true; // Exception handled
	}

	private static (HttpStatusCode statusCode, string title, string detail) GetErrorResponse(Exception exception)
	{
		return exception switch
		{
			// Business logic exceptions that should return 400 Bad Request
			InvalidOperationException ex when ex.Message.Contains("Sender and receiver IBANs cannot be the same") 
				=> (HttpStatusCode.BadRequest, "Invalid Transaction", "Sender and receiver IBANs cannot be the same"),
			
			InvalidOperationException ex when ex.Message.Contains("Insufficient funds") 
				=> (HttpStatusCode.BadRequest, "Insufficient Funds", "The account does not have sufficient funds for this transaction"),
			
			InvalidOperationException ex when ex.Message.Contains("Currency") && ex.Message.Contains("not found") 
				=> (HttpStatusCode.BadRequest, "Invalid Currency", "The specified currency code is not valid"),
			
			InvalidOperationException ex when ex.Message.Contains("Username is already taken") 
				=> (HttpStatusCode.BadRequest, "Username Unavailable", "The username is already taken"),
			
			InvalidOperationException ex when ex.Message.Contains("Email is already registered") 
				=> (HttpStatusCode.BadRequest, "Email Already Registered", "An account with this email already exists"),

			// Entity not found exceptions that should return 404 Not Found
			InvalidOperationException ex when ex.Message.Contains("Sender account") && ex.Message.Contains("not found") 
				=> (HttpStatusCode.NotFound, "Account Not Found", "The sender account was not found"),
			
			InvalidOperationException ex when ex.Message.Contains("User") && ex.Message.Contains("not found") 
				=> (HttpStatusCode.NotFound, "User Not Found", "The specified user was not found"),
			
			InvalidOperationException ex when ex.Message.Contains("Company not found") 
				=> (HttpStatusCode.NotFound, "Company Not Found", "The specified company was not found"),

			InvalidOperationException ex when ex.Message.Contains("bankAccount") && ex.Message.Contains("not found") 
				=> (HttpStatusCode.NotFound, "Bank Account Not Found", "The specified bank account was not found"),

			// Authentication/Authorization exceptions
			SecurityTokenException => (HttpStatusCode.Unauthorized, "Authentication Failed", "Invalid or expired token"),
			UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Access Denied", "You do not have permission to access this resource"),

			// Not implemented features
			NotImplementedException => (HttpStatusCode.NotImplemented, "Feature Not Implemented", "This feature is not yet implemented"),

			// Validation exceptions
			ArgumentNullException ex => (HttpStatusCode.BadRequest, "Missing Required Data", $"Required parameter '{ex.ParamName}' is missing"),
			ArgumentException ex => (HttpStatusCode.BadRequest, "Invalid Request", ex.Message),

			// Default case for unhandled exceptions
			_ => (HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred")
		};
	}
}