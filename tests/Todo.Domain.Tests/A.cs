using HomeApps.BuildingBlocks.Domain.Errors;

namespace Todo.Domain.Tests;

internal static class A
{
    public static readonly DateTimeOffset Now = new(2026, 11, 6, 12, 0, 0, TimeSpan.Zero);

    public static readonly DateOnly Today = DateOnly.FromDateTime(Now.Date);

    public static string Code(this FluentResults.ResultBase result) => (string)result.Errors[0].Metadata[AppError.CodeKey];
}
