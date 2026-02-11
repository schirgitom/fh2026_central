using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using Services.Interfaces;

namespace Services.Services;

public class CoralService : Service<Coral>, ICoralService
{
    public CoralService(IRepository<Coral> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
    }
}
