using Domain.Common.Exceptions;

namespace Domain.ValueObjects;

public class Email
{
    public string Address { get; private set; }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || !address.Contains("@"))
        {
            throw new DomainException("Invalid email address.");
        }
        Address = address;
    }
}
