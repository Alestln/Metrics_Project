using Metrics_Project.Contexts;
using Metrics_Project.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Metrics_Project.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly DataDbContext _context;

    public TestController(DataDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var data = await _context.Persons
            .ToListAsync(cancellationToken);
        
        return Ok(data);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var person = new Person()
        {
            FirstName = "John",
            LastName = "Doe",
            MiddleName = "John",
            BirthdayDate = DateTime.Now,
        };
        
        _context.Persons.Add(person);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Ok();
    }
}