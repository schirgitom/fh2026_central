using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FreshWaterAquariumController : GenericController<FreshWaterAquarium>
{
    public FreshWaterAquariumController(IFreshWaterAquariumService service) : base(service)
    {
    }
}
