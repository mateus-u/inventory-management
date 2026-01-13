using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

public sealed record Currency
{
    public string Code { get; }

    private Currency(string code) => Code = code;

    public static Currency FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Currency code cannot be empty.");
        }

        if (code.Length != 3)
        {
            throw new DomainException("Currency code must be 3 characters");
        }

        return new Currency(code.ToUpperInvariant());
    }
}
