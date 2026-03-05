using CoreEngine.Infrastructure.Persistence;
using CoreEngine.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Mvc;

namespace CoreEngine.API.Controllers;

public class SeedController : BaseApiController
{
    private readonly ApplicationDbContext _context;

    public SeedController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Seed demo data for a specific mining module or all modules.
    /// Valid modules: minesites, shifts, registers, safety, inspections, equipment,
    /// personnel, blasting, production, permits, environmental, ventilation, compliance, geotechnical, all
    /// </summary>
    [HttpPost("mining/{module}")]
    public async Task<ActionResult<object>> SeedMiningModule(string module)
    {
        try
        {
            var result = await MiningDemoDataSeeder.SeedModuleAsync(_context, module);
            return Ok(new { module, result });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
