using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using Services.Interfaces;
using Services.Security;

namespace Services.Services;

public class UserService : Service<User>, IUserService
{
    public UserService(IRepository<User> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
    }

    public async Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var users = await Repository.FilterAsync(u => u.Email == email, cancellationToken);
        var user = users.FirstOrDefault();
        if (user == null)
        {
            return null;
        }

        return PasswordHasher.Verify(password, user.HashedPassword) ? user : null;
    }

    protected override List<string> OnBeforeSave(User entity, bool isCreate)
    {
        var errors = new List<string>();

        if (isCreate)
        {
            if (string.IsNullOrWhiteSpace(entity.Password))
            {
                errors.Add("Password is required.");
                return errors;
            }

            entity.HashedPassword = PasswordHasher.Hash(entity.Password);
            entity.Password = null;
        }
        else if (!string.IsNullOrWhiteSpace(entity.Password))
        {
            entity.HashedPassword = PasswordHasher.Hash(entity.Password);
            entity.Password = null;
        }

        return errors;
    }
}
