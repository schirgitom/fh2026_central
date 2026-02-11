using DAL.Entities;

namespace DAL.Repository.Impl
{
    public class FishRepository : Repository<Fish>, IFishRepository
    {
        public FishRepository(AquariumDBContext context) : base(context)
        {
        }
    }
}
