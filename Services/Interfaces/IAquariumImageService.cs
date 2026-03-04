using DAL.Entities;

namespace Services.Interfaces;

public interface IAquariumImageService : IService<AquariumImage>
{
    Task<List<AquariumImage>> GetImagesForAquariumAsync(string aquariumId, CancellationToken cancellationToken = default);

    Task<bool> AquariumExistsAsync(string aquariumId, CancellationToken cancellationToken = default);
}
