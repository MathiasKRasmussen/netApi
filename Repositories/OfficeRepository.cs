using netApi.Models;
using Dapper;
public interface IOfficeRepository
{
    public Task<IEnumerable<Office>> GetOffices();
    public Task<Office> GetOffice(int id);
}

public class OfficeRepository : IOfficeRepository
{
    private readonly DapperContext _context;
    public OfficeRepository(DapperContext context)
    {
        _context = context;
    }

    // Get all 'Offices'
    public async Task<IEnumerable<Office>> GetOffices()
    {
        var query = "SELECT * FROM Offices";
        using (var connection = _context.CreateConnection())
        {
            var offices = await connection.QueryAsync<Office>(query);
            return offices.ToList();
        }
    }

    // Get one 'Office' by 'Id'
    public async Task<Office> GetOffice(int id)
    {
        var query = "SELECT * FROM Offices WHERE Id = @Id";
        using (var connection = _context.CreateConnection())
        {
            var office = await connection.QuerySingleOrDefaultAsync<Office>(query, new { id });
            return office;
        }
    }
}