using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nlpc_Pension_Project.Application.Dtos;
using Nlpc_Pension_Project.Application.Services.Interface;

namespace Nlpc_Pension_Project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployerController : ControllerBase
{
    private readonly IEmployerService _employerService;

    public EmployerController(IEmployerService employerService)
    {
        _employerService = employerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await _employerService.GetAllAsync();
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployerDto employerDto)
    {
        var response = await _employerService.CreateAsync(employerDto);
        return Ok(response);
    }
}
