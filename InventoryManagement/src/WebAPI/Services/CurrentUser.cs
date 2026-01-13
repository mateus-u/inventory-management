using Application.Common.Interfaces;

namespace WebAPI.Services;

public class CurrentUser : ICurrentUser
{
    public CurrentUser()
    {

    }

    public Guid Id => Guid.NewGuid();
    public string Name => "Current User";
    public string Email => "currentuser@example.com";
}
