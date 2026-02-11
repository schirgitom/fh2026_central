using DAL.Entities;

namespace DAL.Repository.Impl
{
    public class AnimalRepository : Repository<Animal>, IAnimalRepository
    {
        public AnimalRepository(AquariumDBContext context) : base(context)
        {
        }
    }
}
