using DAL.Repository.Impl;
using Microsoft.EntityFrameworkCore;

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
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (NotSupportedException ex) when (ex.Message.Contains("transaction", StringComparison.OrdinalIgnoreCase))
            {
                // Standalone MongoDB deployments do not support transactions.
                Context.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
                await Context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
