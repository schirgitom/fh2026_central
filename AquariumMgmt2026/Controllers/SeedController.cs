using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IFreshWaterAquariumService _freshWaterAquariumService;
    private readonly ISeaWaterAquariumService _seaWaterAquariumService;
    private readonly IFishService _fishService;
    private readonly ICoralService _coralService;
    private readonly ILogger<SeedController> _logger;

    public SeedController(
        IUserService userService,
        IFreshWaterAquariumService freshWaterAquariumService,
        ISeaWaterAquariumService seaWaterAquariumService,
        IFishService fishService,
        ICoralService coralService,
        ILogger<SeedController> logger)
    {
        _userService = userService;
        _freshWaterAquariumService = freshWaterAquariumService;
        _seaWaterAquariumService = seaWaterAquariumService;
        _fishService = fishService;
        _coralService = coralService;
        _logger = logger;
    }

    [HttpPost("dummy-data")]
    public async Task<IActionResult> CreateDummyData(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting dummy data creation");
        var freshwaterUser = new User
        {
            ID = Guid.NewGuid().ToString(),
            Email = "anna.mueller@aquarium-demo.local",
            Firstname = "Anna",
            Lastname = "Mueller",
            Password = "Demo!123",
            Active = true
        };

        var seawaterUser = new User
        {
            ID = Guid.NewGuid().ToString(),
            Email = "lukas.weber@aquarium-demo.local",
            Firstname = "Lukas",
            Lastname = "Weber",
            Password = "Demo!123",
            Active = true
        };

        var createdFreshwaterUser = await _userService.CreateAsync(freshwaterUser, cancellationToken);
        if (!createdFreshwaterUser.IsSuccess)
        {
            _logger.LogWarning("Dummy data creation failed while creating freshwater user");
            return BadRequest(createdFreshwaterUser.Errors);
        }

        var createdSeawaterUser = await _userService.CreateAsync(seawaterUser, cancellationToken);
        if (!createdSeawaterUser.IsSuccess)
        {
            _logger.LogWarning("Dummy data creation failed while creating seawater user");
            return BadRequest(createdSeawaterUser.Errors);
        }

        var freshwaterAquarium = new FreshWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Suedamerika-Becken",
            OwnerId = freshwaterUser.ID,
            Depth = 45,
            Height = 50,
            Length = 120,
            Liters = 270,
            HasFreshAir = true,
            HasCo2System = true
        };

        var seawaterAquarium = new SeaWaterAquarium
        {
            ID = Guid.NewGuid().ToString(),
            Name = "Indopazifik-Riff",
            OwnerId = seawaterUser.ID,
            Depth = 60,
            Height = 60,
            Length = 150,
            Liters = 540,
            IsReefTank = true,
            HasWaveMachine = true
        };

        var createdFreshwaterAquarium =
            await _freshWaterAquariumService.CreateAsync(freshwaterAquarium, cancellationToken);
        if (!createdFreshwaterAquarium.IsSuccess)
        {
            _logger.LogWarning("Dummy data creation failed while creating freshwater aquarium");
            return BadRequest(createdFreshwaterAquarium.Errors);
        }

        var createdSeawaterAquarium = await _seaWaterAquariumService.CreateAsync(seawaterAquarium, cancellationToken);
        if (!createdSeawaterAquarium.IsSuccess)
        {
            _logger.LogWarning("Dummy data creation failed while creating seawater aquarium");
            return BadRequest(createdSeawaterAquarium.Errors);
        }

        var fishToCreate = new[]
        {
            new Fish
            {
                ID = Guid.NewGuid().ToString(),
                AquariumId = freshwaterAquarium.ID,
                Name = "Neonsalmler (Paracheirodon innesi)",
                Inserted = DateTime.UtcNow,
                Amount = 20,
                Description = "Schwarmfisch aus dem Amazonasgebiet."
            },
            new Fish
            {
                ID = Guid.NewGuid().ToString(),
                AquariumId = freshwaterAquarium.ID,
                Name = "Marmorierter Panzerwels (Corydoras paleatus)",
                Inserted = DateTime.UtcNow,
                Amount = 8,
                Description = "Bodenbewohner fuer die untere Wasserzone."
            },
            new Fish
            {
                ID = Guid.NewGuid().ToString(),
                AquariumId = seawaterAquarium.ID,
                Name = "Falscher Clownfisch (Amphiprion ocellaris)",
                Inserted = DateTime.UtcNow,
                Amount = 2,
                Description = "Beliebter Riffbewohner aus dem Indopazifik."
            },
            new Fish
            {
                ID = Guid.NewGuid().ToString(),
                AquariumId = seawaterAquarium.ID,
                Name = "Gelber Segelflossendoktor (Zebrasoma flavescens)",
                Inserted = DateTime.UtcNow,
                Amount = 1,
                Description = "Algenfressender Doktorfisch."
            }
        };

        foreach (var fish in fishToCreate)
        {
            var createdFish = await _fishService.CreateAsync(fish, cancellationToken);
            if (!createdFish.IsSuccess)
            {
                _logger.LogWarning("Dummy data creation failed while creating fish entry for aquarium {AquariumId}",
                    fish.AquariumId);
                return BadRequest(createdFish.Errors);
            }
        }

        var coralsToCreate = new[]
        {
            new Coral
            {
                ID = Guid.NewGuid().ToString(),
                AquariumId = seawaterAquarium.ID,
                Name = "Hammerkoralle (Euphyllia ancora)",
                Inserted = DateTime.UtcNow,
                Amount = 1,
                Description = "Steinkoralle mit langen, hammerfoermigen Tentakeln.",
                CoralTyp = CoralTyp.HardCoral
            },
            new Coral
            {
                ID = Guid.NewGuid().ToString(),
                AquariumId = seawaterAquarium.ID,
                Name = "Lederkoralle (Sarcophyton glaucum)",
                Inserted = DateTime.UtcNow,
                Amount = 1,
                Description = "Robuste Weichkoralle fuer Riffaquarien.",
                CoralTyp = CoralTyp.SoftCoral
            }
        };

        foreach (var coral in coralsToCreate)
        {
            var createdCoral = await _coralService.CreateAsync(coral, cancellationToken);
            if (!createdCoral.IsSuccess)
            {
                _logger.LogWarning("Dummy data creation failed while creating coral entry for aquarium {AquariumId}",
                    coral.AquariumId);
                return BadRequest(createdCoral.Errors);
            }
        }

        _logger.LogInformation(
            "Dummy data creation successful. Users={Users} Aquariums={Aquariums} Fish={Fish} Corals={Corals}",
            2, 2, fishToCreate.Length, coralsToCreate.Length);

        return Ok(new
        {
            message = "Dummydaten wurden erstellt.",
            usersCreated = 2,
            aquariumsCreated = 2,
            fishCreated = fishToCreate.Length,
            coralsCreated = coralsToCreate.Length,
            userIds = new[] { freshwaterUser.ID, seawaterUser.ID },
            aquariumIds = new[] { freshwaterAquarium.ID, seawaterAquarium.ID }
        });
    }
}
