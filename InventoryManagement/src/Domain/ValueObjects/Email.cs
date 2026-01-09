namespace Domain.ValueObjects;

public class Email
{
    public string Address { get; private set; }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address) || !address.Contains("@"))
        {
            throw new ArgumentException("Invalid email address.", nameof(address));
        }
        Address = address;
    }
}
