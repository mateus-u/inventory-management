namespace Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid Id { get; }
    string Name { get; }
    string Email { get; }
}
