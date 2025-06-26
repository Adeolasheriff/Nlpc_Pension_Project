using Microsoft.AspNetCore.Mvc;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.Services;

namespace Nlpc_Pension_Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class ContributionsController : ControllerBase
{
    private readonly IContributionService _contributionService;

    public ContributionsController(IContributionService contributionService)
    {
        _contributionService = contributionService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<int>> Create([FromBody] ContributionProcessingDto request, CancellationToken cancellationToken)
    {
        var id = await _contributionService.CalculateContribution(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContributionDto>> GetById(int id)
    {
        // Placeholder: Replace with service logic
        return NotFound(); // Implement when needed
    }

    [HttpGet("member/{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContributionDto>>> GetForMember(int memberId)
    {
        // Placeholder: Replace with service logic
        return Ok(new List<ContributionDto>()); // Implement when needed
    }
}
