using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

public sealed record Country
{
    public string Code { get; }

    private Country(string code) => Code = code;

    public static Country FromCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Country code cannot be empty.");
        }

        if (code.Length != 2)
        {
            throw new DomainException("Country code must be 2 characters.");
        }

        return new Country(code.ToUpperInvariant());
    }
}
