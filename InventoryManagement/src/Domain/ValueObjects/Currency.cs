namespace Domain.ValueObjects;

public sealed record Currency
{
    public string Code { get; }

    private Currency(string code) => Code = code;

    public static Currency FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Currency code cannot be empty.", nameof(code));
        }

        if (code.Length != 3)
        {
            throw new ArgumentException("Currency code must be 3 characters (ISO 4217).", nameof(code));
        }

        return new Currency(code.ToUpperInvariant());
    }
}
