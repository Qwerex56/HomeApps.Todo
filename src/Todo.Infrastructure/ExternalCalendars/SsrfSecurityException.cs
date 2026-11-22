namespace Todo.Infrastructure.ExternalCalendars;

/// <summary>An address of an imported calendar points inside the server's network (ADR 3.6).</summary>
internal sealed class SsrfSecurityException(string message) : Exception(message);
