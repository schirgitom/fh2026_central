using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using Services.Interfaces;

namespace Services.Services;

public class FishService : Service<Fish>, IFishService
{
    public FishService(IRepository<Fish> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
    }
}
