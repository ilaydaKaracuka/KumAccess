using Dapper;
using System.Data;
using System.Threading.Tasks;

namespace KumAccess.Services
{
    public class ApplicationService
    {
        private readonly IDbConnection _db;

        public ApplicationService(IDbConnection db)
        {
            _db = db;
        }

        public async Task AddApplicationAsync(string applicationName)
        {
            var sql = "INSERT INTO KumAccess.dbo.Application (ApplicationName) VALUES (@ApplicationName)";
            await _db.ExecuteAsync(sql, new { ApplicationName = applicationName });
        }
    }
}
