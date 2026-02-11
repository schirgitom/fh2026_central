using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeaWaterAquariumController : GenericController<SeaWaterAquarium>
{
    public SeaWaterAquariumController(ISeaWaterAquariumService service) : base(service)
    {
    }
}
