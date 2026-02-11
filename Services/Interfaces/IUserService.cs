using DAL.Entities;

namespace Services.Interfaces;

public interface IUserService : IService<User>
{
    Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}
