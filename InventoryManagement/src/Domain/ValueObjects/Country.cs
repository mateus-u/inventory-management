namespace Domain.ValueObjects;

public sealed record Country
{
    public string Code { get; }

    private Country(string code) => Code = code;

    public static Country FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Country code cannot be empty.", nameof(code));
        }

        if (code.Length != 2)
        {
            throw new ArgumentException("Country code must be 2 characters (ISO 3166-1 alpha-2).", nameof(code));
        }

        return new Country(code.ToUpperInvariant());
    }
}
