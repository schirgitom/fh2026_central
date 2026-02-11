using DAL.Repository.Impl;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AquariumDBContext Context;

        public UnitOfWork(AquariumDBContext context)
        {
            Context = context;
        }


        public IAquariumRepository Aquariums
        {
            get
            {
                return new AquariumRepository(Context);
            }
        }

        public IAnimalRepository Animals
        {
            get
            {
                return new AnimalRepository(Context);
            }
        }

        public ICoralRepository Corals
        {
            get
            {
                return new CoralRepository(Context);
            }
        }

        public IFishRepository Fishes
        {
            get
            {
                return new FishRepository(Context);
            }
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
    }
}
