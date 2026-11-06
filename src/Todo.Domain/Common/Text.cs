namespace Todo.Domain.Common;

/// <summary>Small helpers for text typed by people.</summary>
internal static class Text
{
    /// <summary>Trimmed text, or null when blank.</summary>
    public static string? OrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>Control characters are not allowed, except new lines and tabs where <paramref name="multiline"/>.</summary>
    public static bool HasControlCharacters(string value, bool multiline = false) =>
        value.Any(c => char.IsControl(c) && !(multiline && c is '\n' or '\r' or '\t'));
}
