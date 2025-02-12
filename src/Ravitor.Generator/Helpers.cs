using System.Text.RegularExpressions;

namespace Ravitor.Generator;

internal static class Helpers
{
    private static Regex ClassNameRegex { get; set; } = new Regex("""^[A-Za-z_][A-Za-z0-9_]*$""");

    public static bool IsValidClassName(this string? className)
    {
        return className is { Length: > 0 } && ClassNameRegex.IsMatch(className);
    }
}
