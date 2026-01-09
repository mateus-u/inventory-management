using Domain.Common.Entities;
using Domain.Common.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Supplier : BaseEntity
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }

    public Email Email { get; protected set; }
    public Currency Currency { get; protected set; }
    public Country Country { get; protected set; }

    protected Supplier() { }

    public Supplier(string name, Email email, Currency currency, Country country): base()
    {
        Id = Guid.NewGuid();

        if(string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Name cannot be empty.");
        }

        if(email == null)
        {
            throw new DomainException("Email cannot be null.");
        }

        if(currency == null)
        {
            throw new DomainException("Currency cannot be null.");
        }

        if(country == null)
        {
            throw new DomainException("Country cannot be null.");
        }

        Name = name;

        Email = email;
        Currency = currency;
        Country = country;
    }
}
