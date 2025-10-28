using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

		var problemDetails = new ProblemDetails
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
				=> (HttpStatusCode.BadRequest, "Transazione Non Valida", "IBAN del mandante e destinatario non possono essere uguali"),

			InvalidOperationException ex when ex.Message.Contains("Insufficient funds")
				=> (HttpStatusCode.BadRequest, "Fondi Insufficienti", "Il conto non dispone di fondi sufficienti per questa transazione"),

			InvalidOperationException ex when ex.Message.Contains("Currency") && ex.Message.Contains("not found")
				=> (HttpStatusCode.BadRequest, "Valuta Non Valida", "La valuta specificata non e' valida"),

			InvalidOperationException ex when ex.Message.Contains("Username is already taken")
				=> (HttpStatusCode.BadRequest, "Username Non Disponibile", "Questo username e' gia' stato utilizzato"),

			InvalidOperationException ex when ex.Message.Contains("Email is already registered")
				=> (HttpStatusCode.BadRequest, "Email Gia' Registrata", "Un account con questa email e' gia' stato utilizzato"),

			// Entity not found exceptions that should return 404 Not Found
			InvalidOperationException ex when ex.Message.Contains("Sender account") && ex.Message.Contains("not found")
				=> (HttpStatusCode.NotFound, "Account Non Trovato", "L'account del destinatario non e' stato trovato"),

			InvalidOperationException ex when ex.Message.Contains("User") && ex.Message.Contains("not found")
				=> (HttpStatusCode.NotFound, "Utente Non Trovato", "L'utente specificato non e' stato trovato"),

			InvalidOperationException ex when ex.Message.Contains("Company not found")
				=> (HttpStatusCode.NotFound, "Azienda Non Trovata", "L'azienda specificata non e' stata trovata"),

			InvalidOperationException ex when ex.Message.Contains("bankAccount") && ex.Message.Contains("not found")
				=> (HttpStatusCode.NotFound, "Conto Bancario Non Trovato", "Il conto bancario specificato non e' stato trovato"),

			// Authentication/Authorization exceptions
			SecurityTokenException => (HttpStatusCode.Unauthorized, "Autenticazione Fallita", "Token non valido o scaduto"),
			UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Accesso Negato", "Non hai i permessi per accedere a questa risorsa"),

			// Not implemented features
			NotImplementedException => (HttpStatusCode.NotImplemented, "Funzionalita' Non Implementata", "Questa funzionalita' non e' ancora stata implementata"),

			// Validation exceptions
			ArgumentNullException ex => (HttpStatusCode.BadRequest, "Dati Obbligatori Mancanti", $"Il parametro obbligatorio '{ex.ParamName}' e' mancante"),
			ArgumentException ex => (HttpStatusCode.BadRequest, "Richiesta Non Valida", ex.Message),

			// Default case for unhandled exceptions
			_ => (HttpStatusCode.InternalServerError, "Errore Interno del Server", "Si e' verificato un errore imprevisto")
		};
	}
}