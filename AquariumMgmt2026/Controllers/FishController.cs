using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FishController : GenericController<Fish>
{
    public FishController(IFishService service) : base(service)
    {
    }
}
