using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using Services.Interfaces;

namespace Services.Services;

public class SeaWaterAquariumService : Service<SeaWaterAquarium>, ISeaWaterAquariumService
{
    public SeaWaterAquariumService(IRepository<SeaWaterAquarium> repository, IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
    }
}
