using netApi.Models;
using Dapper;
using System.Data;

public interface IEmployeeRepository
{
    public Task<IEnumerable<Employee>> GetEmployees();
    public Task<Employee> GetEmployee(int id);
    public Task<int> GetEmployeeCountForOffice(int officeId);
    public Task<Employee> CreateEmployee(EmployeeDto employee);
    public Task UpdateEmployee(int id, EmployeeDto employee);
    public Task DeleteEmployee(int id);
}

public class EmployeeRepository : IEmployeeRepository
{
    private readonly DapperContext _context;
    public EmployeeRepository(DapperContext context)
    {
        _context = context;
    }

    // Get all 'Employees'
    public async Task<IEnumerable<Employee>> GetEmployees()
    {
        var query = "SELECT * FROM Employees";
        using (var connection = _context.CreateConnection())
        {
            var employees = await connection.QueryAsync<Employee>(query);
            return employees.ToList();
        }
    }

    // Get one 'Employee' by 'Id'
    public async Task<Employee> GetEmployee(int id)
    {
        var query = "SELECT * FROM Employees WHERE Id = @Id";
        using (var connection = _context.CreateConnection())
        {
            var employee = await connection.QuerySingleOrDefaultAsync<Employee>(query, new { id });
            return employee;
        }
    }
    // Get the count of 'Employees' at an 'Office'
    public async Task<int> GetEmployeeCountForOffice(int id)
    {
        var query = "SELECT COUNT( * ) FROM Employees WHERE OfficeId = @Id;";
        using (var connection = _context.CreateConnection())
        {
            var employeesAmount = await connection.QuerySingleOrDefaultAsync<int>(query, new { id });
            return employeesAmount;
        }
    }

    // Create one 'Employee'
    public async Task<Employee> CreateEmployee(EmployeeDto employee)
    {
        // Query returns the 'Id' of the newly created 'Employee'
        var query = "INSERT INTO Employees (FirstName, LastName, BirthDate, OfficeId) VALUES (@FirstName, @LastName, @BirthDate, @OfficeId) SELECT CAST(SCOPE_IDENTITY() as int)";
        var parameters = new DynamicParameters();
        parameters.Add("FirstName", employee.FirstName, DbType.String);
        parameters.Add("LastName", employee.LastName, DbType.String);
        parameters.Add("BirthDate", employee.BirthDate, DbType.DateTime);
        parameters.Add("OfficeId", employee.OfficeId, DbType.Int32);

        using (var connection = _context.CreateConnection())
        {
            // 'id' obtained from query
            var id = await connection.QuerySingleAsync<int>(query, parameters);
            // Creates an 'Employee' object and returns it
            var createdEmployee = new Employee
            {
                Id = id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                BirthDate = employee.BirthDate,
                OfficeId = employee.OfficeId
            };
            return createdEmployee;
        }
    }

    // Update an 'Employee' row in the database
    public async Task UpdateEmployee(int id, EmployeeDto employee)
    {
        var query = "UPDATE Employees SET FirstName = @FirstName, LastName = @LastName, BirthDate = @BirthDate, OfficeId = @OfficeId WHERE Id = @Id";
        var parameters = new DynamicParameters();
        parameters.Add("Id", id, DbType.Int32);
        parameters.Add("FirstName", employee.FirstName, DbType.String);
        parameters.Add("LastName", employee.LastName, DbType.String);
        parameters.Add("BirthDate", employee.BirthDate, DbType.DateTime);
        parameters.Add("OfficeId", employee.OfficeId, DbType.Int32);
        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, parameters);
        }
    }

    // Deleting one 'Employee' based on their 'id'
    public async Task DeleteEmployee(int id)
    {
        var query = "DELETE FROM Employees WHERE Id = @Id";
        using (var connection = _context.CreateConnection())
        {
            await connection.ExecuteAsync(query, new { id });
        }
    }
}