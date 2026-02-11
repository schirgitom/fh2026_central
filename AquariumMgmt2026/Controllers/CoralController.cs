using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoralController : GenericController<Coral>
{
    public CoralController(ICoralService service) : base(service)
    {
    }
}
