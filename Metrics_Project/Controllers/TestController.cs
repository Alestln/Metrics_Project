using Metrics_Project.Contexts;
using Metrics_Project.Entities;
using Metrics_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Metrics_Project.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly DataDbContext _context;
    private readonly ILogger<TestController> _logger;

    public TestController(DataDbContext context, ILogger<TestController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // --- READ Operations ---

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<PersonViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonViewModel>>> GetAll(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all persons");
        var data = await _context.Persons
            .AsNoTracking()
            .Select(p => PersonViewModel.FromEntity(p))
            .ToListAsync(cancellationToken);

        return Ok(data);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonViewModel>> GetById(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting person by ID: {PersonId}", id);
        var person = await _context.Persons
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found", id);
            return NotFound();
        }

        return Ok(PersonViewModel.FromEntity(person));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<PersonViewModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonViewModel>>> Search(
        [FromQuery] SearchQueryParameters queryParams,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching persons with params: {@QueryParams}", queryParams);

        var query = _context.Persons.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
        {
            var term = queryParams.SearchTerm.Trim().ToLower();
            query = query.Where(p =>
                (p.FirstName.ToLower().Contains(term)) ||
                (p.LastName.ToLower().Contains(term)) ||
                (p.MiddleName != null && p.MiddleName.ToLower().Contains(term))
            );
        }

        Expression<Func<Person, object>> orderByExpression = queryParams.SortBy?.ToLowerInvariant() switch
        {
            "firstname" => p => p.FirstName!,
            "birthdaydate" => p => p.BirthdayDate,
            _ => p => p.LastName!
        };

        if (queryParams.SortDescending == 1)
        {
            query = query.OrderByDescending(orderByExpression);
        }
        else
        {
            query = query.OrderBy(orderByExpression);
        }
        
        var pageNumber = queryParams.PageNumber > 0 ? queryParams.PageNumber : 1;
        var pageSize = queryParams.PageSize > 0 ? queryParams.PageSize : 20;
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);


        var results = await query
                            .Select(p => PersonViewModel.FromEntity(p))
                            .ToListAsync(cancellationToken);

        return Ok(results);
    }

    // --- CREATE Operation ---

    [HttpPost]
    [ProducesResponseType(typeof(PersonViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] PersonCreateModel createModel, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new person: {@PersonData}", createModel);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var person = new Person
        {
            FirstName = createModel.FirstName,
            LastName = createModel.LastName,
            MiddleName = createModel.MiddleName,
            BirthdayDate = createModel.BirthdayDate.ToUniversalTime()
        };

        _context.Persons.Add(person);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Person created with ID: {PersonId}", person.Id);


        var viewModel = PersonViewModel.FromEntity(person);

        return CreatedAtAction(nameof(GetById), new { id = person.Id }, viewModel);
    }

    // --- UPDATE Operation ---

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] PersonUpdateModel updateModel, CancellationToken cancellationToken)
    {
         _logger.LogInformation("Updating person ID: {PersonId} with data: {@PersonData}", id, updateModel);
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var person = await _context.Persons.FindAsync(new object[] { id }, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found for update", id);
            return NotFound();
        }

        person.FirstName = updateModel.FirstName;
        person.LastName = updateModel.LastName;
        person.MiddleName = updateModel.MiddleName;
        person.BirthdayDate = updateModel.BirthdayDate.ToUniversalTime();

        await _context.SaveChangesAsync(cancellationToken);
         _logger.LogInformation("Person with ID: {PersonId} updated successfully", id);


        return NoContent();
    }

    // --- DELETE Operation ---

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting person with ID: {PersonId}", id);
        var person = await _context.Persons.FindAsync(new object[] { id }, cancellationToken);

        if (person == null)
        {
            _logger.LogWarning("Person with ID: {PersonId} not found for deletion", id);
            return NotFound();
        }

        _context.Persons.Remove(person);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Person with ID: {PersonId} deleted successfully", id);


        return NoContent();
    }
}