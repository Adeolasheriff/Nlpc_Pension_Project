using Microsoft.AspNetCore.Mvc;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.Services.Interface;
using Microsoft.AspNetCore.Http;


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
        return Ok(new
        {
            id
        });
    }

    [HttpGet("member/{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContributionStatement(int memberId, CancellationToken cancellationToken)
    {
        var response = await _contributionService.GetContributionStatement(memberId, cancellationToken);
        return  Ok(response);
    }

}
