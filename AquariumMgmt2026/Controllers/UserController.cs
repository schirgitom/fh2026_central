using DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace AquariumMgmt2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : GenericController<User>
{
    public UserController(IUserService service, ILogger<UserController> logger) : base(service, logger)
    {
    }
}
