namespace ArmGSService.Extensions;

public static class StringExtensions
{
    public static string ReplaceFirstCharToLowercase(this string input)
    {
        return !string.IsNullOrEmpty(input) ? char.ToLower(input[0]) + input[1..] : input;
    }
}