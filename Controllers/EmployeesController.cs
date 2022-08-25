using Microsoft.AspNetCore.Mvc;

[Route("api/employees")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IOfficeRepository _officeRepo;
    private readonly DateTime dateMinLimit = DateTime.Today.AddYears(-75);
    private readonly DateTime dateMaxLimit = DateTime.Today.AddYears(-18);

    public EmployeesController(IEmployeeRepository employeeRepo, IOfficeRepository officeRepo)
    {
        _employeeRepo = employeeRepo;
        _officeRepo = officeRepo;
    }

    // Get all employees
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var employees = await _employeeRepo.GetEmployees();
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Get 'Employee' by 'Id'
    [HttpGet("{id}", Name = "EmployeeById")]
    public async Task<IActionResult> GetEmployee(int id)
    {
        try
        {
            var employee = await _employeeRepo.GetEmployee(id);
            if (employee == null)
                return NotFound();
            return Ok(employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Create a new 'Employee' and return it
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(EmployeeDto employee)
    {
        try
        {
            // The 'LastName' of an employee cannot contain white space
            if (employee.LastName.Contains(" ")) return StatusCode(422, "\'LastName\' cannot contain white space");

            // Is the 'BirthDate' in the valid range
            if (employee.BirthDate < dateMinLimit || dateMaxLimit < employee.BirthDate)
                return StatusCode(422, "\'BirthDate\' has to be between " + dateMinLimit + " and " + dateMaxLimit);

            // Checks if the 'Office' exits
            var dbOffice = await _officeRepo.GetOffice(employee.OfficeId);
            if (dbOffice == null) return NotFound("Office not found");
            // Is the 'MaxOccupancy' reached at the 'Office'
            var employeesAmount = await _employeeRepo.GetEmployeeCountForOffice(employee.OfficeId);
            if (employeesAmount >= dbOffice.MaxOccupancy)
                return StatusCode(409, "Max number of employees at this office (" + employeesAmount + "/" + dbOffice.MaxOccupancy + ")");

            // Create the 'Employee' 
            var createdEmployee = await _employeeRepo.CreateEmployee(employee);
            return CreatedAtRoute("EmployeeById", new { id = createdEmployee.Id }, createdEmployee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Updating an 'Employee'
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeDto employee)
    {
        try
        {
            // Checks if the 'Employee' with the 'id' exists
            var dbEmployee = await _employeeRepo.GetEmployee(id);
            if (dbEmployee == null)
                return NotFound("Employee not found");

            // The 'LastName' of an employee cannot contain white space
            if (employee.LastName.Contains(" ")) return StatusCode(422, "\'LastName\' cannot contain white space");

            // Is the 'BirthDate' in the valid range
            if (employee.BirthDate < dateMinLimit || dateMaxLimit < employee.BirthDate)
                return StatusCode(422, "\'BirthDate\' has to be between " + dateMinLimit + " and " + dateMaxLimit);

            // Checks if the 'Office' exits
            var dbOffice = await _officeRepo.GetOffice(employee.OfficeId);
            if (dbOffice == null) return NotFound("Office not found");
            // Is the 'MaxOccupancy' reached at the 'Office'
            var employeesAmount = await _employeeRepo.GetEmployeeCountForOffice(employee.OfficeId);
            if (employeesAmount >= dbOffice.MaxOccupancy && dbEmployee.OfficeId != employee.OfficeId)
                return StatusCode(409, "Max number of employees at this office (" + employeesAmount + "/" + dbOffice.MaxOccupancy + ")");

            await _employeeRepo.UpdateEmployee(id, employee);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Deleting an 'Employee' based on their 'id'
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            // Checks if the 'Employee' with the 'id' exists
            var dbEmployee = await _employeeRepo.GetEmployee(id);
            if (dbEmployee == null)
                return NotFound();
            // Deletes the 'Employee'
            await _employeeRepo.DeleteEmployee(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}