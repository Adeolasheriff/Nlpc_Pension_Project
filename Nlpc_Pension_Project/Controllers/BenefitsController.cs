using Microsoft.AspNetCore.Mvc;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.Services;

namespace Nlpc_Pension_Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersion("1.0")]
public class BenefitsController : ControllerBase
{
    private readonly ICalculateBenefit _benefitService;

    public BenefitsController(ICalculateBenefit benefitService)
    {
        _benefitService = benefitService;
    }

    [HttpGet("{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BenefitDto>> Calculate(int memberId, CancellationToken cancellationToken)
    {
        var benefit = await _benefitService.CalculateBenefit(memberId, cancellationToken);
        return Ok(benefit);
    }
}
