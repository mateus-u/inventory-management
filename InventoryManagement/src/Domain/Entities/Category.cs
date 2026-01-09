using Domain.Common.Entities;
using Domain.Common.Exceptions;

namespace Domain.Entities;

public class Category : BaseEntity
{
    public Guid Id { get; protected set; }
    public string Name { get; protected set; }
    public string Shortcode { get; protected set; }

    public Category? Parent { get; protected set; }

    protected Category() { }

    public Category(string name, string shortcode, Category? parent = null) : base()
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Name cannot be empty.");
        }
        if (string.IsNullOrWhiteSpace(shortcode))
        {
            throw new DomainException("Shortcode cannot be empty.");
        }

        Id = Guid.NewGuid();

        Shortcode = shortcode;
        Name = name;

        Parent = parent;
    }
}
