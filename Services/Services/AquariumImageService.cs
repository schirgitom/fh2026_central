using DAL.Entities;
using DAL.Repository;
using DAL.UnitOfWork;
using Services.Interfaces;

namespace Services.Services;

public class AquariumImageService : Service<AquariumImage>, IAquariumImageService
{
    private readonly IRepository<Aquarium> _aquariumRepository;

    public AquariumImageService(IRepository<AquariumImage> repository, IRepository<Aquarium> aquariumRepository,
        IUnitOfWork unitOfWork)
        : base(repository, unitOfWork)
    {
        _aquariumRepository = aquariumRepository;
    }

    public async Task<List<AquariumImage>> GetImagesForAquariumAsync(string aquariumId,
        CancellationToken cancellationToken = default)
    {
        return await Repository.FilterAsync(i => i.AquariumId == aquariumId, cancellationToken);
    }

    public async Task<bool> AquariumExistsAsync(string aquariumId, CancellationToken cancellationToken = default)
    {
        var aquarium = await _aquariumRepository.GetAsync(aquariumId, cancellationToken);
        return aquarium != null;
    }
}
