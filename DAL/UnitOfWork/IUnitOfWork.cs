using DAL.Repository.Impl;

namespace DAL.UnitOfWork
{
    public interface IUnitOfWork
    {


        IAquariumRepository Aquariums { get; }

        IAnimalRepository Animals { get; }
        ICoralRepository Corals { get; }
        IFishRepository Fishes { get; }
        
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
