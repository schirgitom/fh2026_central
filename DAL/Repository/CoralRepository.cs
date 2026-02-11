using DAL.Entities;

namespace DAL.Repository.Impl
{
    public class CoralRepository : Repository<Coral>, ICoralRepository
    {
        public CoralRepository(AquariumDBContext context) : base(context)
        {
        }
    }
}
