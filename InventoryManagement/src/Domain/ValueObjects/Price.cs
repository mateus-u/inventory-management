namespace Domain.ValueObjects;

public sealed record Price
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Price() 
    {
        Currency = null!;
    }

    private Price(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Price Create(decimal amount, Currency currency)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));
        }

        if (currency == null)
        {
            throw new ArgumentNullException(nameof(currency), "Currency cannot be null.");
        }

        return new Price(amount, currency);
    }
}
