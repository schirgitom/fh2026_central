using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using Services.Interfaces;

namespace Services.Services;

public class FreshWaterAquariumService : Service<FreshWaterAquarium>, IFreshWaterAquariumService
{
    public FreshWaterAquariumService(IRepository<FreshWaterAquarium> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
    }
}
