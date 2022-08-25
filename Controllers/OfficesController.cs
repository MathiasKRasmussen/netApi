using Microsoft.AspNetCore.Mvc;

[Route("api/offices")]
[ApiController]
public class OfficesController : ControllerBase
{
    private readonly IOfficeRepository _officeRepo;

    public OfficesController(IOfficeRepository officeRepo)
    {
        _officeRepo = officeRepo;
    }

    // Get all 'Offices'
    [HttpGet]
    public async Task<IActionResult> GetOffices()
    {
        try
        {
            var offices = await _officeRepo.GetOffices();
            return Ok(offices);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Get 'Office' by 'Id'
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOffice(int id)
    {
        try
        {
            var office = await _officeRepo.GetOffice(id);
            if (office == null)
                return NotFound();
            return Ok(office);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}