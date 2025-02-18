using System.Text.RegularExpressions;

namespace Blackwing.Generator;

internal static class Helpers
{
    private static Regex ClassNameRegex { get; set; } = new Regex("""^[A-Za-z_][A-Za-z0-9_]*$""");
    private static Regex NamespaceRegex { get; set; } = new Regex("""^[A-Za-z_][A-Za-z0-9_.]*[A-Za-z0-9_]$""");

    public static bool IsValidClassName(this string? className)
    {
        return className is { Length: > 0 } && ClassNameRegex.IsMatch(className);
    }

    public static bool IsValidNamespace(this string? namesapce)
    {
        return namesapce is { Length: > 0 } && NamespaceRegex.IsMatch(namesapce);
    }
}
